using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using APS.Model;
using APS.Areas.Gestion.Models;

namespace APS.Validators
{
    public class EmailUnique : ValidationAttribute
    {
        DBContext db = new DBContext();

        /// <summary>
        /// Vérifie si l'email n'est pas déjà utilisé
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var utilisateur = (InformationUtilisateurViewModel) validationContext.ObjectInstance;

            if (!db.Users.Any(x => x.Email == utilisateur.Email && x.Id != utilisateur.Id))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(String.Format("L'adresse courriel \"{0}\" est déjà utilisée\nVeuillez modifier la nouvelle entrée\nAutrement, toute modification ne sera enregistrée", utilisateur.Email));
        }
    }
}