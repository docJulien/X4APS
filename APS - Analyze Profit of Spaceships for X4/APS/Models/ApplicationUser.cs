using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using APS.Extensions;
using APS.Resources;

namespace APS.Model
{
    public class ApplicationUser : IdentityUser
    {
        [CustomRequired]
        [Display(Name = "Prenom", ResourceType = typeof(Label))]
        [StringLength(40)]
        public string FirstName { get; set; }

        [CustomRequired]
        [Display(Name = "Nom", ResourceType = typeof(Label))]
        [StringLength(40)]
        public string LastName { get; set; }
        
        [Required]
        [DefaultValue("FR")]
        public string Langue { get; set; }
        
        [CustomRequired]
        public DateTime DateHeureMAJ { get; set; }

        public bool AccountDisabled { get; set; }
    }
}
