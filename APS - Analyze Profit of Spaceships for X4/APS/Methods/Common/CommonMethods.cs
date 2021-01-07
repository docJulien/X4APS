using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APS.Areas.Gestion.Models;
using APS.Methods.QueriesExtensions;
using APS.Model;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace APS.Methods.Common
{
    public static class CommonMethods
    {
        //Gérer erreurs AJAX
        public static DataSourceResult ReturnError(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary ModelState, 
            ILogger _logger, LogLevel l, string error, string field, string user)
        {
            _logger.Log(l, user + ": " + error);
            ModelState.AddModelError(field, error);
            return (new DataSourceResult
            {
                Errors = error,
                Data = ModelState,
                Total = 1
            });
        }

        // On obtient la liste de Tous les éléments du type passé en parametre.
        public static List<T> GetAllData<T>(bool includeAll) where T : class
        {
            using (DBContext db = new DBContext())
            {
                DbSet<T> dbSet = db.Set<T>();

                List<T> resultats = includeAll ? dbSet.IncludeAll().ToList() : dbSet.ToList();

                return resultats;
            }
        }

        // On bénéficie du filtre devancé du DataSourceRequest mais retourne un ViewModel de type VM qui sera Mappé à partir de T. Usage: return Json(CommonMethods.GetDataResult<tblVille, VillesVM>(request));
        public static DataSourceResult GetDataResult<T,VM>(DataSourceRequest request, bool includeAll = false) where T : class where VM : new()
        {
            DataSourceResult ds = includeAll ? CommonMethods.GetQuery<T>().IncludeAll().ToDataSourceResult(request) :
                                CommonMethods.GetQuery<T>().ToDataSourceResult(request);
            ds.Data = ((List<T>)ds.Data)
                            .Select(x=> new VM().Map(x));
            return ds;
        }
                
        public static IQueryable<T> GetQuery<T>() where T : class
        {
            DBContext db = new DBContext();
            
            DbSet<T> dbSet = db.Set<T>();

            return dbSet;            
        }

        // On obtient un seul élément en fonction de l'expression passé en paramètre.
        public static T GetSingle<T>(Func<T, bool> expression, bool IncludeAll) where T : class
        {
            using (DBContext db = new DBContext())
            {
                DbSet<T> dbSet = db.Set<T>();

                T entity = IncludeAll ? dbSet.IncludeAll().Single(expression) : dbSet.Single(expression);

                return entity;
            }
        }

        // On doit passer l'entité que l'on veut créer. Ex: Create(new AspNetUsers { Id = id, Nom = nom, Actif = actif });
        public static T Create<T>(T entity) where T : class
        {
            using (DBContext db = new DBContext())
            {
                DbSet<T> dbSet = db.Set<T>();


                EntityEntry<T> newEntry = dbSet.Add(entity);

                db.SaveChanges();

                return newEntry.Entity;
            }
        }
        
        // On update l'élément spécifié par l'expression. On ignore les valeurs non spécifiées dans l'objet
        // Ex: Update(x => x.Id == data.Id,  new AspNetUsers { Nom = data.Nom })
        public static T Update<T>(Func<T, bool> expression, T newData) where T : class
        {
            using (DBContext db = new DBContext())
            {
                DbSet<T> dbSet = db.Set<T>();
                T entity = dbSet.Single(expression);
                
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    object value = property.GetValue(newData);
                    if (value != null && property.CanWrite
                        && property.GetValue(entity) != value)
                    {
                        property.SetValue(entity, value);
                    }
                }

                db.SaveChanges();

                return entity;
            }
        }
        
        internal static List<string> GetRole(List<IdentityRole> groupes)
        {
            return groupes.Select(x => x.NormalizedName).ToList();
        }
        private static List<IdentityRole> GetRole(ApplicationUser x)
        {
            using (DBContext db = new DBContext())
            {
                return (from u in db.UserRoles
                        join r in db.Roles on u.RoleId equals r.Id
                        where u.UserId == x.Id
                        select r).ToList();
            }
        }
        internal static List<InformationUtilisateurViewModel> GetUserList()
        {
            using (DBContext db = new DBContext())
            {
                var resultat = db.Users.Select(x =>
                     new InformationUtilisateurViewModel(x, GetRole(x))).ToList();
                return resultat;
            }
        }
    }
}