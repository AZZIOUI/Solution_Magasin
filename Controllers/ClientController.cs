using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;
using Solution_Magasin.ViewModels;

namespace Solution_Magasin.Controllers;

/// <summary>
/// Contrôleur pour l'espace client
/// Accessible uniquement aux utilisateurs avec le rôle Client
/// </summary>
[Authorize(Policy = RoleConstants.ClientPolicy)]
public class ClientController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DotnetProjectContext _dbContext;
    private readonly ILogger<ClientController> _logger;

    public ClientController(
        UserManager<ApplicationUser> userManager,
        DotnetProjectContext dbContext,
        ILogger<ClientController> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
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
    public async Task<IActionResult> MyOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var orders = await _dbContext.Ventes
            .Include(v => v.DetailVentes)
                .ThenInclude(dv => dv.IdArticleNavigation)
            .Include(v => v.IdPaymentNavigation)
            .Where(v => v.IdClient == user.ClientId)
            .OrderByDescending(v => v.DateVente)
            .ToListAsync();

        return View(orders);
    }

    /// <summary>
    /// Affiche le profil du client
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.IdClient == user.ClientId);

        if (client == null)
        {
            return NotFound();
        }

        var viewModel = new ClientProfileViewModel
        {
            ClientId = client.IdClient,
            FirstName = client.PrenomClient ?? "",
            LastName = client.NomClient ?? "",
            Email = client.MailClient ?? "",
            Address = client.AdresseClient ?? "",
            Phone = client.TelClient ?? "",
            DateCreated = user.DateCreated
        };

        return View(viewModel);
    }

    /// <summary>
    /// Met à jour le profil du client
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ClientProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.IdClient == user.ClientId);

        if (client == null)
        {
            return NotFound();
        }

        // Mettre à jour les informations du client
        client.PrenomClient = model.FirstName;
        client.NomClient = model.LastName;
        client.MailClient = model.Email;
        client.AdresseClient = model.Address;
        client.TelClient = model.Phone;

        // Mettre à jour aussi l'utilisateur Identity
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.UserName = model.Email;
        user.PhoneNumber = model.Phone;

        await _dbContext.SaveChangesAsync();
        await _userManager.UpdateAsync(user);

        TempData["SuccessMessage"] = "Votre profil a été mis à jour avec succès.";
        return RedirectToAction(nameof(Profile));
    }
}
