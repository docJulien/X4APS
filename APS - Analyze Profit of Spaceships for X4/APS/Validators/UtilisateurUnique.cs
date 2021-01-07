using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using APS.Model;
using APS.Areas.Gestion.Models;

namespace APS.Validators
{
    public class UtilisateurUnique : ValidationAttribute
    {
        DBContext db = new DBContext();

        /// <summary>
        /// Vérifie si le nom d'utilisateur n'est pas déjà utilisé
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var utilisateur = (InformationUtilisateurViewModel) validationContext.ObjectInstance;

            if (!db.Users.Any(x => x.UserName == utilisateur.UserName && x.Id != utilisateur.Id))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(String.Format("Le nom d'utilisateur \"{0}\" est déjà utilisé\nVeuillez modifier la nouvelle entrée\nAutrement, toute modification ne sera enregistrée", utilisateur.UserName));
        }
    }
}