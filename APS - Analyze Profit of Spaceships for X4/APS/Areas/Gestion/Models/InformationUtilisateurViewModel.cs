using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using APS.Model;
using APS.Validators;
using APS.Extensions;
using Microsoft.AspNetCore.Identity;
using APS.Resources;

namespace APS.Areas.Gestion.Models
{
    public sealed class InformationUtilisateurViewModel
    {
        /// <summary>
        /// Constructeur vide
        /// </summary>
        public InformationUtilisateurViewModel()
        {

        }

        /// <summary>
        /// Constructeur (Fabrique)
        /// </summary>
        /// <param name="utilisateur"></param>
        public InformationUtilisateurViewModel(ApplicationUser utilisateur, List<IdentityRole> roles)
        {
            Id = utilisateur.Id;
            UserName = utilisateur.UserName;
            LastName = utilisateur.LastName;
            FirstName = utilisateur.FirstName;
            Email = utilisateur.Email;

            Groupes = roles;

            Actif = !utilisateur.AccountDisabled;
            Langue = utilisateur.Langue;
        }

        public string Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "Requis")]
        [CustomDisplayName("Prenom")]
        [StringLength(40, ErrorMessage = "Le champ {0} doit être une chaîne d'une longueur maximale de {1} et minimale de {2}.", MinimumLength = 2)]
        [RegularExpression(@"^[A-ZÀ-ÿa-zÀ-ÿ](?:[A-ZÀ-ÿa-zÀ-ÿ]|-(?=[A-ZÀ-ÿa-zÀ-ÿ])|[ ](?=[A-ZÀ-ÿa-zÀ-ÿ])|['](?=[A-ZÀ-ÿa-zÀ-ÿ]))*", ErrorMessage = "Le prénom doit uniquement contenir des lettres, des apostrophes, des tirets et des espaces")] // TODO Label
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "Requis")]
        [StringLength(40, MinimumLength = 2)]
        [RegularExpression(@"^[A-ZÀ-ÿa-zÀ-ÿ](?:[A-ZÀ-ÿa-zÀ-ÿ]|-(?=[A-ZÀ-ÿa-zÀ-ÿ])|[ ](?=[A-ZÀ-ÿa-zÀ-ÿ])|['](?=[A-ZÀ-ÿa-zÀ-ÿ]))*", ErrorMessage = "Le nom doit uniquement contenir des lettres, des apostrophes, des tirets et des espaces")] // TODO Label
        [CustomDisplayName("Nom")]
        public string LastName { get; set; }

        [CustomDisplayName("Nom")]
        public string NomComplet => FirstName + " " + LastName;

        [DefaultValue("FR")]
        public string Langue { get; set; }

        [CustomDisplayName("Actif")]
        public bool Actif { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "Requis")]
        [StringLength(256)]
        [EmailAddress(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "CourrielInvalide")]
        [EmailUnique]
        [CustomDisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "Requis")]
        [UtilisateurUnique]
        [RegularExpression(@"^[A-Za-z0-9](?:[A-Za-z0-9])*",
            ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "FormatUsername")]
        [StringLength(256)]
        [CustomDisplayName("Username")]
        public string UserName { get; set; }

        [CustomDisplayName("Groupes")]
        public List<IdentityRole> Groupes { get; set; }

        [DataType(DataType.Password)]
        [StringLength(12, ErrorMessage = "Le champ {0} doit être une chaîne d'une longueur maximale de {1} et minimale de {2}.", MinimumLength = 5)]
        [CustomDisplayName("MotPasse")]
        public string MotDePasse { get; set; }

        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne concordent pas.")]
        [DataType(DataType.Password)]
        [CustomDisplayName("MotPasseConfirmer")]
        public string ConfirmationMotDePasse { get; set; }
    }

}