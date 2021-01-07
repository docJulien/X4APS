using APS.Helpers;
using APS.Model;
using Microsoft.AspNetCore.Identity;
using System;

namespace APS.Data
{
    public class SeedModule
    {
        public static void SeedData
               (UserManager<ApplicationUser> userManager,
               RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedRoles
                (RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync
                (ConstanteRole.Administrateur).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = ConstanteRole.Administrateur;
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync
                (ConstanteRole.Externe).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = ConstanteRole.Externe;
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }

        public async static void SeedUsers
            (UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = userManager.FindByNameAsync("admin").Result;
            if (user == null)
            {
                user = new ApplicationUser();
                user.UserName = "admin";
                user.Email = "your@email.com";
                user.FirstName = "Julien";
                user.LastName = "Doc";
                user.Langue = "EN";
                user.DateHeureMAJ = DateTime.Now;

                IdentityResult result = userManager.CreateAsync
                (user, "Secret1234!").Result;

                if (!result.Succeeded)
                {
                    throw new Exception("oops while seeding User");
                }
            }

            if (!userManager.IsInRoleAsync(user,
                                ConstanteRole.Administrateur).Result)
                userManager.AddToRoleAsync(user,
                               ConstanteRole.Administrateur).Wait();
            
        }

    }
}
