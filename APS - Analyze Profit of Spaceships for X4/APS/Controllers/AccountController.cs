using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APS.Model;
using APS.Model.AccountViewModels;
using APS.Services;
using APS.Resources;
using Microsoft.AspNetCore.Http;
using APS.Methods.Gestion;
using APS.Areas.Gestion.Models;
using APS.Methods.QueriesExtensions;
using APS.Methods.Common;

namespace APS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private IHttpContextAccessor _accessor;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IHttpContextAccessor accessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _accessor = accessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser u;
                using (DBContext db = new DBContext())
                {
                    u = db.Users.Where(x => x.UserName == model.Username).FirstOrDefault();
                }
                if (u!=null && u.AccountDisabled)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Error.ConnectionIncorrect);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = Helpers.ConstanteRole.Administrateur)]
        public IActionResult LoginAs(string id)
        {
            _logger.LogWarning(User.Identity.Name + ":LoginAs:" + id + ">" + _accessor.HttpContext.Connection.RemoteIpAddress.ToString());
            if (Methods.Gestion.Utilisateurs.GetFastSwitchUsers().Contains(id))
            {
                ApplicationUser u = Methods.Common.CommonMethods.GetSingle<ApplicationUser>(x => x.UserName == id, false);
                
                _signInManager.SignInAsync(u, false).Wait();
                return Redirect("/ERP/Ticket");
            }
            else
            {
                return Json(CommonMethods.ReturnError(ModelState, _logger, LogLevel.Error, "Accès Refusé", "", User.Identity.Name));
            }
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogError($"Unable to load two-factor authentication user.");
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogError($"Unable to load two-factor authentication user.");
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.UserName);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.UserName);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.UserName);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }
                
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string ReturnURL = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "" });
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"Unable to load user with ID '{userId}'.");
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
                _logger.LogError($"Failed to load user with ID '{userId}'.");
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    _logger.LogError($"ForgotPassword failed attempt using." + model.Email);
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //bypass car pas fonctionnel sur linux ou autre raison (fonctionnel en debug local)
                code = Guid.NewGuid().ToString();
                user.SecurityStamp = code;
                Methods.Common.CommonMethods.Update<ApplicationUser>(x => x.Id == user.Id, user);

                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                Service.SendMail(model.Email, Message.CourrielReinitialiserMotPasseSubject, string.Format(Message.CourrielReinitialiserMotPasseBody, callbackUrl), string.Empty, string.Empty, true);

                _logger.LogWarning($"ForgotPassword initiated attempt." + model.Email);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                _logger.LogWarning("ResetPassword: A code must be supplied for password reset.");
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                _logger.LogWarning("ResetPassword: A code must be supplied for password reset.");
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            //julien: bypass de aspnet car non fonctionnel sur linux ou autre
            if (user.SecurityStamp != model.Code)
            {
                _logger.LogWarning("ResetPassword: SecurityStamp invalide.");
                ModelState.AddModelError(string.Empty, "Token invalide. Valider la procédure...");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Mot de passe invalide. Valider la procédure...");
                }
                else
                {
                    string passwordHash = _userManager.PasswordHasher.HashPassword(new ApplicationUser(), model.Password);
                    InformationUtilisateurViewModel iusr = new InformationUtilisateurViewModel().Map(user);
                    iusr.Actif = !user.AccountDisabled;
                    Utilisateurs.UpdateUtilisateur(iusr, passwordHash, user.Id, true);
                    _logger.LogWarning("ResetPassword: successful pour " + user.UserName);
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = true; //todo Utilisateurs.Register(model);
                if (success)
                    return View("RegisterConfirmation");
            }
            return View(model);
        }

        #region Helpers
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
