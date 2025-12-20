using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Solution_Magasin.Models;

namespace Solution_Magasin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Si l'utilisateur est connecté, rediriger vers son espace approprié
            if (User.Identity?.IsAuthenticated == true)
            {
                // Administrateur -> Tableau de bord admin
                if (User.IsInRole("Administrateur"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                
                // Client -> Espace client
                if (User.IsInRole("Client"))
                {
                    return RedirectToAction("Index", "Client");
                }
                
                // Employé (ResponsableAchat ou Magasinier) -> Espace employé
                if (User.IsInRole("ResponsableAchat") || User.IsInRole("Magasinier"))
                {
                    return RedirectToAction("Index", "Employee");
                }
            }

            // Si non connecté ou aucun rôle reconnu, afficher la page d'accueil publique
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
