using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution_Magasin.Constants;

namespace Solution_Magasin.Controllers
{
    /// <summary>
    /// Contrôleur pour l'espace Employé
    /// Accessible à tous les employés (Administrateur, ResponsableAchat, Magasinier)
    /// </summary>
    [Authorize(Policy = RoleConstants.EmployePolicy)]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        }

        // GET: /Employee/Index - Accessible à tous les employés
        public IActionResult Index()
        {
            ViewData["Message"] = "Tableau de bord employé";
            return View();
        }

        // GET: /Employee/Sales - Accessible à tous les employés
        public IActionResult Sales()
        {
            ViewData["Message"] = "Gestion des ventes";
            return View();
        }

        // GET: /Employee/Purchases - Accessible uniquement aux Administrateurs et Responsables Achat
        [Authorize(Policy = RoleConstants.ResponsableAchatPolicy)]
        public IActionResult Purchases()
        {
            ViewData["Message"] = "Gestion des achats";
            return View();
        }

        // GET: /Employee/Stock - Accessible uniquement aux Administrateurs et Magasiniers
        [Authorize(Policy = RoleConstants.MagasinierPolicy)]
        public IActionResult Stock()
        {
            ViewData["Message"] = "Gestion du stock";
            return View();
        }

        // GET: /Employee/Users - Accessible uniquement aux Administrateurs
        [Authorize(Policy = RoleConstants.AdminPolicy)]
        public IActionResult Users()
        {
            ViewData["Message"] = "Gestion des utilisateurs";
            return View();
        }

        // GET: /Employee/Reports - Accessible uniquement aux Administrateurs
        [Authorize(Policy = RoleConstants.AdminPolicy)]
        public IActionResult Reports()
        {
            ViewData["Message"] = "Rapports et statistiques";
            return View();
        }

        // GET: /Employee/Presence - Accessible à tous les employés
        public IActionResult Presence()
        {
            ViewData["Message"] = "Pointage des présences";
            return View();
        }
    }
}
