using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;
using Solution_Magasin.ViewModels;

namespace Solution_Magasin.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<AspNetUser> userManager,
            SignInManager<AspNetUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Utilisateur {model.Email} connecté.");
                    
                    // Rediriger selon le rôle
                    var roles = await _userManager.GetRolesAsync(user);
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    
                    if (roles.Contains(RoleConstants.Client))
                    {
                        return RedirectToAction("Index", "Client");
                    }
                    else if (roles.Any(r => RoleConstants.GetEmployeeRoles().Contains(r)))
                    {
                        return RedirectToAction("Index", "Employee");
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"Compte utilisateur {model.Email} verrouillé.");
                    return View("Lockout");
                }
                
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            }

            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AspNetUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Prenom = model.Prenom,
                    Nom = model.Nom,
                    Cin = model.Cin,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Nouvel utilisateur créé.");

                    // Assigner le rôle Client par défaut pour les inscriptions publiques
                    if (model.RegisterAsClient)
                    {
                        await _userManager.AddToRoleAsync(user, RoleConstants.Client);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    return RedirectToAction("Index", model.RegisterAsClient ? "Client" : "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Utilisateur déconnecté.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account/Lockout
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }
    }
}
