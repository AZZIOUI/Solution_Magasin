using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution_Magasin.Constants;

namespace Solution_Magasin.Controllers;

/// <summary>
/// Contrôleur pour l'espace client
/// Accessible uniquement aux utilisateurs avec le rôle Client
/// </summary>
[Authorize(Policy = RoleConstants.ClientPolicy)]
public class ClientController : Controller
{
    private readonly ILogger<ClientController> _logger;

    public ClientController(ILogger<ClientController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Page d'accueil de l'espace client
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Accès à l'espace client par {User}", User.Identity?.Name);
        return View();
    }

    /// <summary>
    /// Page des commandes du client
    /// </summary>
    public IActionResult MyOrders()
    {
        return View();
    }

    /// <summary>
    /// Page du profil du client
    /// </summary>
    public IActionResult Profile()
    {
        return View();
    }
}
