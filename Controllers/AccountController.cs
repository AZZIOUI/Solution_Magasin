using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;
using Solution_Magasin.ViewModels;

namespace Solution_Magasin.Controllers;

/// <summary>
/// Contrôleur pour la gestion de l'authentification et des comptes utilisateur
/// </summary>
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly DotnetProjectContext _dbContext;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        DotnetProjectContext dbContext,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Affiche la page de connexion
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Traite la connexion de l'utilisateur
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Utilisateur connecté: {Email}", model.Email);
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                return RedirectToAction("Index", "Home");
            }
            
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Compte verrouillé: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Votre compte a été verrouillé suite à plusieurs tentatives échouées.");
                return View(model);
            }
            
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
        }

        return View(model);
    }

    /// <summary>
    /// Affiche la page d'inscription
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Traite l'inscription d'un nouveau client
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            // 1. Créer d'abord un enregistrement Client
            var client = new Client
            {
                NomClient = model.LastName,
                PrenomClient = model.FirstName,
                MailClient = model.Email
            };

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();

            // 2. Créer l'utilisateur Identity et le lier au Client
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserType = "Client",
                ClientId = client.IdClient,  // Lier à l'enregistrement Client
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assigner le rôle Client par défaut
                await _userManager.AddToRoleAsync(user, RoleConstants.Client);
                
                _logger.LogInformation("Nouveau compte client créé: {Email}, ClientId: {ClientId}", model.Email, client.IdClient);

                await _signInManager.SignInAsync(user, isPersistent: false);
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                return RedirectToAction("Index", "Home");
            }

            // Si la création de l'utilisateur Identity échoue, supprimer le Client créé
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    /// <summary>
    /// Déconnecte l'utilisateur
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Utilisateur déconnecté");
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Page d'accès refusé
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
