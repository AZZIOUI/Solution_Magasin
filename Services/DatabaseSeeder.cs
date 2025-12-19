using Microsoft.AspNetCore.Identity;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;

namespace Solution_Magasin.Services;

/// <summary>
/// Service pour initialiser la base de données avec les rôles et l'administrateur par défaut
/// </summary>
public class DatabaseSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DotnetProjectContext _dbContext;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        DotnetProjectContext dbContext,
        ILogger<DatabaseSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Initialise les rôles et crée un compte administrateur par défaut
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Créer les rôles s'ils n'existent pas
            await CreateRolesAsync();

            // Créer un administrateur par défaut
            await CreateDefaultAdminAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données");
        }
    }

    /// <summary>
    /// Crée tous les rôles de l'application
    /// </summary>
    private async Task CreateRolesAsync()
    {
        var roles = RoleConstants.GetAllRoles();

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    _logger.LogInformation("Rôle créé: {RoleName}", roleName);
                }
                else
                {
                    _logger.LogError("Erreur lors de la création du rôle {RoleName}: {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    /// <summary>
    /// Crée un compte administrateur par défaut si aucun n'existe
    /// </summary>
    private async Task CreateDefaultAdminAsync()
    {
        const string adminEmail = "admin@magasin.com";
        const string adminPassword = "Admin123!";

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            // 1. Créer d'abord un enregistrement Employe
            var employe = new Employe
            {
                NomEmp = "Système",
                PrenomEmp = "Admin",
                Cin = "ADMIN000",
                DateEmbauche = DateOnly.FromDateTime(DateTime.Now)
            };

            _dbContext.Employes.Add(employe);
            await _dbContext.SaveChangesAsync();

            // 2. Créer l'utilisateur Identity et le lier à l'Employe
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "Système",
                UserType = "Employe",
                EmployeId = employe.IdUtilisateur,  // Lier à l'enregistrement Employe
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, RoleConstants.Administrateur);
                _logger.LogInformation("Administrateur par défaut créé: {Email}, EmployeId: {EmployeId}", 
                    adminEmail, employe.IdUtilisateur);
                _logger.LogWarning("IMPORTANT: Changez le mot de passe de l'administrateur par défaut!");
            }
            else
            {
                // Si la création de l'utilisateur Identity échoue, supprimer l'Employe créé
                _dbContext.Employes.Remove(employe);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogError("Erreur lors de la création de l'administrateur: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
