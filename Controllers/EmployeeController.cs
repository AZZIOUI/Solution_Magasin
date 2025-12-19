using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution_Magasin.Constants;

namespace Solution_Magasin.Controllers;

/// <summary>
/// Contrôleur pour l'espace employé
/// Accessible aux utilisateurs avec les rôles: Administrateur, ResponsableAchat, ou Magasinier
/// </summary>
[Authorize(Policy = RoleConstants.EmployePolicy)]
public class EmployeeController : Controller
{
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(ILogger<EmployeeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Page d'accueil de l'espace employé
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Accès à l'espace employé par {User}", User.Identity?.Name);
        return View();
    }

    /// <summary>
    /// Gestion des stocks - Accessible uniquement aux Administrateurs et Magasiniers
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public IActionResult StockManagement()
    {
        return View();
    }

    /// <summary>
    /// Gestion des achats - Accessible uniquement aux Administrateurs et Responsables d'achat
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.ResponsableAchat}")]
    public IActionResult PurchaseManagement()
    {
        return View();
    }

    /// <summary>
    /// Administration - Accessible uniquement aux Administrateurs
    /// </summary>
    [Authorize(Policy = RoleConstants.AdminPolicy)]
    public IActionResult Administration()
    {
        return View();
    }
}
