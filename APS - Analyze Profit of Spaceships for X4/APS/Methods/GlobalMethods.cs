using APS.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace APS.Methods
{
    /// <summary>
    /// Classe de méthode globale d'accès à la base de données pouvant servir partout dans le code (liste, etc).
    /// </summary>
    public static class GlobalMethods
    {
        /// <summary>
        /// Retourne la liste de tous les rôles (groupes) de la base de données
        /// </summary>
        /// <param name="langueId"></param>
        /// <returns></returns>
        public static IList<IdentityRole> GetRoles()
        {
            using (DBContext db = new DBContext())
            {
                var data = db.Roles.ToList();

                return data;
            }
        }

        public static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                    .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
    }
}