using System;
using System.Collections.Generic;
using System.Linq;
using APS.Areas.Gestion.Models;
using APS.Model;

namespace APS.Methods.Gestion
{
    public static class Utilisateurs
    {
        /// <summary>
        /// Met à jour les informations d'un utilisateur
        /// </summary>
        /// <param name="model"></param>
        /// <param name="motDePasseModel"></param>
        /// <param name="passwordHash"></param>
        public static void UpdateUtilisateur(InformationUtilisateurViewModel model, string passwordHash, string UsagerMAJID, bool resetToken = false)
        {
            using (DBContext db = new DBContext())
            {
                ApplicationUser utilisateurExistant = db.Users.Single(u => u.Id == model.Id);

                utilisateurExistant.UserName = model.UserName;
                utilisateurExistant.NormalizedUserName = model.UserName.ToUpperInvariant();
                utilisateurExistant.AccountDisabled = !model.Actif;
                utilisateurExistant.Email = model.Email;
                utilisateurExistant.NormalizedEmail = model.Email.ToUpperInvariant();
                utilisateurExistant.DateHeureMAJ = DateTime.Now;
                utilisateurExistant.FirstName = model.FirstName;
                utilisateurExistant.LastName = model.LastName;
                if (resetToken)
                    utilisateurExistant.SecurityStamp = string.Empty;


                if (!string.IsNullOrWhiteSpace(passwordHash))
                {
                    utilisateurExistant.PasswordHash = passwordHash;
                }

                db.SaveChanges();
            }
        }

        public static List<string> GetFastSwitchUsers()
        {
            using (DBContext db = new DBContext())
            {
                return (from u in db.Users
                        join r in db.UserRoles on u.Id equals r.UserId
                        join n in db.Roles on r.RoleId equals n.Id
                        where n.Name.Equals(Helpers.ConstanteRole.Externe)
                        select u.UserName
                        ).ToList();                    
            }
        }
    }
}