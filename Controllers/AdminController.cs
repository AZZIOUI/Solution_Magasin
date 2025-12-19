using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;
using Solution_Magasin.ViewModels;

namespace Solution_Magasin.Controllers;

/// <summary>
/// Contrôleur pour l'administration - gestion des utilisateurs et employés
/// Accessible uniquement aux administrateurs
/// </summary>
[Authorize(Policy = RoleConstants.AdminPolicy)]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DotnetProjectContext _dbContext;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        DotnetProjectContext dbContext,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Page d'accueil de l'administration
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Affiche le formulaire de création d'un employé
    /// </summary>
    [HttpGet]
    public IActionResult CreateEmployee()
    {
        // Liste des rôles employés disponibles
        ViewBag.EmployeeRoles = new SelectList(new[]
        {
            new { Value = RoleConstants.Administrateur, Text = "Administrateur" },
            new { Value = RoleConstants.ResponsableAchat, Text = "Responsable d'Achat" },
            new { Value = RoleConstants.Magasinier, Text = "Magasinier" }
        }, "Value", "Text");

        return View();
    }

    /// <summary>
    /// Traite la création d'un nouveau compte employé
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeViewModel model)
    {
        if (ModelState.IsValid)
        {
            // 1. Créer d'abord un enregistrement Employe
            var employe = new Employe
            {
                NomEmp = model.LastName,
                PrenomEmp = model.FirstName,
                Cin = model.CIN,
                DateEmbauche = DateOnly.FromDateTime(DateTime.Now)
            };

            _dbContext.Employes.Add(employe);
            await _dbContext.SaveChangesAsync();

            // 2. Créer l'utilisateur Identity et le lier à l'Employe
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserType = "Employe",
                EmployeId = employe.IdUtilisateur,  // Lier à l'enregistrement Employe
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assigner le rôle sélectionné
                await _userManager.AddToRoleAsync(user, model.Role);
                
                _logger.LogInformation("Nouveau compte employé créé: {Email}, EmployeId: {EmployeId}, Role: {Role}", 
                    model.Email, employe.IdUtilisateur, model.Role);

                TempData["SuccessMessage"] = $"Employé {model.FirstName} {model.LastName} créé avec succès (Rôle: {model.Role})";
                return RedirectToAction(nameof(Index));
            }

            // Si la création de l'utilisateur Identity échoue, supprimer l'Employe créé
            _dbContext.Employes.Remove(employe);
            await _dbContext.SaveChangesAsync();

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Recharger la liste des rôles en cas d'erreur
        ViewBag.EmployeeRoles = new SelectList(new[]
        {
            new { Value = RoleConstants.Administrateur, Text = "Administrateur" },
            new { Value = RoleConstants.ResponsableAchat, Text = "Responsable d'Achat" },
            new { Value = RoleConstants.Magasinier, Text = "Magasinier" }
        }, "Value", "Text");

        return View(model);
    }

    /// <summary>
    /// Liste tous les utilisateurs du système
    /// </summary>
    public async Task<IActionResult> Users()
    {
        var users = _userManager.Users.ToList();
        var userViewModels = new List<UserListViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userViewModels.Add(new UserListViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                UserType = user.UserType ?? "",
                IsActive = user.IsActive,
                Roles = string.Join(", ", roles)
            });
        }

        return View(userViewModels);
    }
}
