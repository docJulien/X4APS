using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using APS.Model;
using System.Threading.Tasks;
using APS.Methods.Common;
using APS.Methods.Gestion;
using APS.Areas.Gestion.Models;
using APS.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using APS.Methods;
using Microsoft.AspNetCore.Authorization;
using APS.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace APS.Areas.Gestion.Controllers
{
    [Area("Gestion")]
    [Authorize(Roles = Helpers.ConstanteRole.Administrateur)]
    public class UtilisateursController : Controller
    {
        private readonly ILogger _logger;
        private UserManager<ApplicationUser> _userManager;
        private string _userId;

        public UtilisateursController(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager,
            ILogger<UtilisateursController> logger)
        {
            _userManager = userManager;
            _userId = userManager.GetUserId(httpContextAccessor.HttpContext.User);
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Appel Ajax de grille kendo principale de ce controlleur
        /// </summary>
        /// <param name="request"></param>        
        [ExceptionMessages(ResourceKey = "Read")]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<InformationUtilisateurViewModel> result = CommonMethods.GetUserList();
            
            return Json(result.ToDataSourceResult(request)); //, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sauvegarde les informations du profil utilisateur entrées dans le popup pour la création
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [ExceptionMessages(ResourceKey = "Create")]
        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request,
            InformationUtilisateurViewModel model)
        {
            //if (ModelState.ContainsKey("UserName"))
            //{
            //    ModelState["UserName"].Errors.Clear();
            //    ModelState["UserName"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            //}
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Langue = "FR",
                    DateHeureMAJ = DateTime.Now,
                    AccountDisabled = !model.Actif
                };
                if (string.IsNullOrWhiteSpace(model.MotDePasse))
                {
                    model.MotDePasse = Guid.NewGuid().ToString().Replace("-", string.Empty).Remove(12) + "A!";
                }

                IdentityResult result = await _userManager.CreateAsync(user, model.MotDePasse);
                model.ConfirmationMotDePasse = "**********";
                model.MotDePasse = "**********";
                _logger.LogWarning("création de User." + JsonConvert.SerializeObject(model) + " par " + User.Identity.Name);

                if (result.Succeeded)
                {
                    model.Id = user.Id; // Affecter le Id nouvellement créé par le UserManager au model pour que l'ajout des rôles fonctionne
                    AddRoles(model, false);
                    return Json(new[] { user }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    string error = "Erreur de création de l'usager. Vérifier si l'information respecte les règles de sécurité: " + string.Join(", ", result.Errors.Select(x => x.Description));
                    ModelState.AddModelError("UserName", error);
                    return this.Json(new DataSourceResult
                    {
                        Errors = error,
                        Data = ModelState,
                        Total = 1
                    });
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Sauvegarde les informations du profil utilisateur entrées dans le formulaire pour la modification
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [ExceptionMessages(ResourceKey = "Update")]
        public async Task<IActionResult> Update([DataSourceRequest]DataSourceRequest request, InformationUtilisateurViewModel model)
        {
            if (model.MotDePasse != model.ConfirmationMotDePasse)
                throw new Exception("Mot de passe de confirmation différent.");
            string passwordHash = string.Empty;
            if (!string.IsNullOrWhiteSpace(model.MotDePasse))
                 passwordHash = _userManager.PasswordHasher.HashPassword(new ApplicationUser(), model.MotDePasse);

            Utilisateurs.UpdateUtilisateur(model, passwordHash, _userId);
            model.ConfirmationMotDePasse = "**********";
            model.MotDePasse = "**********";
            _logger.LogWarning("update de User." + JsonConvert.SerializeObject(model) + " par " + User.Identity.Name);
            AddRoles(model);
                
            return Json(new[] { model }.ToDataSourceResult(request));
        }

        /// <summary>
        /// Sauvegarde les informations du profil utilisateur entrées dans le formulaire pour la modification
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [ExceptionMessages(ResourceKey = "Delete")]
        public async Task<IActionResult> Delete([DataSourceRequest]DataSourceRequest request, InformationUtilisateurViewModel model)
        {
            if (model.Id == null)
            {
                return this.Json(new
                {
                    success = false,
                    message = "Erreur l'effacement de l'utilisateur! " + "Code invalide. Contacter le support et indiquer les étapes pour reproduire ce problème."
                });
            }


            var user = await _userManager.FindByIdAsync(model.Id);

            IdentityResult rc = new IdentityResult();
            rc = await  _userManager.DeleteAsync(user);
            if (!rc.Succeeded)
                throw new Exception("Une erreur est survenue: " + rc.Errors.First().Description);
            //     transaction.commit();
            //}

            model.ConfirmationMotDePasse = "**********";
            model.MotDePasse = "**********";
            _logger.LogWarning("delete de User." + JsonConvert.SerializeObject(model) + " par " + User.Identity.Name);

            return Json(new[] { model }.ToDataSourceResult(request));
        }

        /// <summary>
        /// Enlève tous les rôles avant d'ajouter les nouveaux rôles sélectionnés
        /// </summary>
        /// <param name="model"></param>
        /// <param name="removeRoles"></param>
        private void AddRoles(InformationUtilisateurViewModel model, bool removeRoles = true)
        {
            if (!_userManager.SupportsUserRole)
                return;
            
            ApplicationUser user = _userManager.FindByIdAsync(model.Id).Result;
            IList<string> rls = _userManager.GetRolesAsync(user).Result;
            _userManager.RemoveFromRolesAsync(user, rls).Wait();
            rls = _userManager.GetRolesAsync(user).Result;
            if (rls.Count > 1)
                throw new Exception("Oups");

            // S'il y a des groupes de sélectionnés...
            if (model.Groupes != null)
            {
                IEnumerable<string> groupeIds = model.Groupes.Select(x => x.Id);

                // Ajouter des groupes dans AspNetRoles à l'aide du UserManager (qui s'occupe de faire les liens entre les tables ASP
                IdentityResult result =
                    _userManager.AddToRolesAsync(user, CommonMethods.GetRole(model.Groupes)).Result;

                if (!result.Succeeded)
                {
                    throw new Exception(Error.AjoutRoles);
                }
            }
        }

        [HttpGet]
        public JsonResult GetRoles()
        {
            var resultat = GlobalMethods.GetRoles().ToList();

            return Json(resultat);
        }
    }
}