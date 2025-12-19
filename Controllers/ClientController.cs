using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution_Magasin.Constants;

namespace Solution_Magasin.Controllers
{
    /// <summary>
    /// Contrôleur pour l'espace Client
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

        // GET: /Client/Index
        public IActionResult Index()
        {
            ViewData["Message"] = "Bienvenue dans votre espace client";
            return View();
        }

        // GET: /Client/MyOrders
        public IActionResult MyOrders()
        {
            ViewData["Message"] = "Mes commandes";
            return View();
        }

        // GET: /Client/Profile
        public IActionResult Profile()
        {
            ViewData["Message"] = "Mon profil";
            return View();
        }

        // GET: /Client/Reviews
        public IActionResult Reviews()
        {
            ViewData["Message"] = "Mes avis";
            return View();
        }
    }
}
