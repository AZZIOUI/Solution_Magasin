using Microsoft.AspNetCore.Identity;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;

namespace Solution_Magasin.Services
{
    /// <summary>
    /// Service pour initialiser les rôles et les utilisateurs par défaut
    /// </summary>
    public class DbInitializer
    {
        /// <summary>
        /// Initialise les rôles dans la base de données
        /// </summary>
        public static async Task SeedRolesAsync(RoleManager<AspNetRole> roleManager)
        {
            var roles = RoleConstants.GetAllRoles();
            
            foreach (var roleName in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new AspNetRole 
                    { 
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }
        }

        /// <summary>
        /// Crée un administrateur par défaut
        /// </summary>
        public static async Task SeedAdminAsync(UserManager<AspNetUser> userManager)
        {
            var adminEmail = "admin@magasin.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new AspNetUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Nom = "Admin",
                    Prenom = "System",
                    Cin = "ADMIN001",
                    PhoneNumber = "0600000000"
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, RoleConstants.Administrateur);
                }
            }
        }

        /// <summary>
        /// Crée des utilisateurs de test pour chaque rôle
        /// </summary>
        public static async Task SeedTestUsersAsync(UserManager<AspNetUser> userManager)
        {
            // Client de test
            await CreateUserIfNotExists(userManager, 
                "client@test.com", 
                "Client@123", 
                "Client", 
                "Test", 
                "CL001",
                RoleConstants.Client);

            // Responsable Achat de test
            await CreateUserIfNotExists(userManager,
                "achat@magasin.com",
                "Achat@123",
                "Achat",
                "Responsable",
                "RA001",
                RoleConstants.ResponsableAchat);

            // Magasinier de test
            await CreateUserIfNotExists(userManager,
                "magasin@magasin.com",
                "Magasin@123",
                "Magasin",
                "Stock",
                "MG001",
                RoleConstants.Magasinier);
        }

        private static async Task CreateUserIfNotExists(
            UserManager<AspNetUser> userManager,
            string email,
            string password,
            string nom,
            string prenom,
            string cin,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                var newUser = new AspNetUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Nom = nom,
                    Prenom = prenom,
                    Cin = cin,
                    PhoneNumber = "0600000000"
                };

                var result = await userManager.CreateAsync(newUser, password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
            }
        }
    }
}
