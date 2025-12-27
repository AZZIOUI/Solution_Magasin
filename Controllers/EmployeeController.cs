using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;
using Solution_Magasin.ViewModels;

namespace Solution_Magasin.Controllers;

/// <summary>
/// Contrôleur pour l'espace employé
/// Accessible aux utilisateurs avec les rôles: Administrateur, ResponsableAchat, ou Magasinier
/// </summary>
[Authorize(Policy = RoleConstants.EmployePolicy)]
public class EmployeeController : Controller
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly DotnetProjectContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EmployeeController(
        ILogger<EmployeeController> logger,
        DotnetProjectContext context,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Page d'accueil de l'espace employé avec tableau de bord
    /// </summary>
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Accès à l'espace employé par {User}", User.Identity?.Name);
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !user.EmployeId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var employe = await _context.Employes
            .FirstOrDefaultAsync(e => e.IdUtilisateur == user.EmployeId.Value);

        var viewModel = new EmployeeDashboardViewModel
        {
            NomEmploye = employe != null ? $"{employe.PrenomEmp} {employe.NomEmp}" : user.UserName,
            Cin = employe?.Cin,
            DateEmbauche = employe?.DateEmbauche,
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
        };

        // Statistiques de présence
        if (employe != null)
        {
            var debutMois = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
            var presences = await _context.Presences
                .Where(p => p.IdUtilisateur == employe.IdUtilisateur && p.DateP >= debutMois)
                .ToListAsync();

            viewModel.PresencesCeMois = presences.Count;
            viewModel.HeuresTotalesCeMois = presences
                .Where(p => p.HeureArrive.HasValue && p.HeureDepart.HasValue)
                .Sum(p => (p.HeureDepart!.Value.ToTimeSpan() - p.HeureArrive!.Value.ToTimeSpan()).TotalHours);

            // Présence aujourd'hui
            var aujourdhui = DateOnly.FromDateTime(DateTime.Now);
            var presenceAujourdhui = await _context.Presences
                .FirstOrDefaultAsync(p => p.IdUtilisateur == employe.IdUtilisateur && p.DateP == aujourdhui);

            if (presenceAujourdhui != null)
            {
                viewModel.PresenceAujourdhui = new PresenceViewModel
                {
                    IdPr = presenceAujourdhui.IdPr,
                    Date = presenceAujourdhui.DateP ?? aujourdhui,
                    HeureArrive = presenceAujourdhui.HeureArrive,
                    HeureDepart = presenceAujourdhui.HeureDepart
                };
            }
        }

        // Statistiques de stock (pour Magasinier et Admin)
        if (User.IsInRole(RoleConstants.Magasinier) || User.IsInRole(RoleConstants.Administrateur))
        {
            var stocks = await _context.Stocks.Include(s => s.IdArticleNavigation).ToListAsync();
            viewModel.AlertesStock = stocks.Count(s => s.QteDispo <= s.Stockmin);
            viewModel.ArticlesEnRupture = stocks.Count(s => s.QteDispo == 0);

            // Notifications non lues
            viewModel.NotificationsNonLues = await _context.NotificationStocks
                .CountAsync(n => n.Vu == false || n.Vu == null);

            // Dernières notifications
            var notifications = await _context.NotificationStocks
                .Include(n => n.IdArticleNavigation)
                .OrderByDescending(n => n.DateNot)
                .Take(5)
                .ToListAsync();

            viewModel.DernieresNotifications = notifications.Select(n => new StockNotificationViewModel
            {
                IdNot = n.IdNot,
                IdArticle = n.IdArticle,
                NomArticle = n.IdArticleNavigation.NomArt,
                ReferenceArticle = n.IdArticleNavigation.ReferenceArt,
                Message = n.Msg,
                DateNotification = n.DateNot,
                Vu = n.Vu ?? false
            }).ToList();

            // Statistiques des commandes (pour Magasinier et Admin)
            var aujourdhui = DateTime.Today;
            viewModel.CommandesEnTraitement = await _context.Ventes
                .CountAsync(v => v.Status == "En traitement");
            viewModel.CommandesEnPreparation = await _context.Ventes
                .CountAsync(v => v.Status == "En préparation");
            viewModel.CommandesPretesAujourdhui = await _context.Ventes
                .CountAsync(v => v.Status == "Prête" && v.DateVente.HasValue && v.DateVente.Value.Date == aujourdhui);
        }

        // Statistiques d'achats (pour Responsable d'Achat et Admin)
        if (User.IsInRole(RoleConstants.ResponsableAchat) || User.IsInRole(RoleConstants.Administrateur))
        {
            var debutMois = DateTime.Now.AddMonths(-1);
            var achats = await _context.Achats
                .Where(a => a.DateAchat >= debutMois)
                .ToListAsync();

            viewModel.AchatsCeMois = achats.Count;
            viewModel.TotalAchatsCeMois = achats.Sum(a => a.TotalAch ?? 0);
        }

        return View(viewModel);
    }

    #region Gestion des Stocks

    /// <summary>
    /// Gestion des stocks - Accessible uniquement aux Administrateurs et Magasiniers
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public async Task<IActionResult> StockManagement(string searchTerm, string filter)
    {
        var query = _context.Stocks
            .Include(s => s.IdArticleNavigation)
            .ThenInclude(a => a.IdCatNavigation)
            .AsQueryable();

        // Filtres
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(s => 
                s.IdArticleNavigation.NomArt!.Contains(searchTerm) ||
                s.IdArticleNavigation.ReferenceArt!.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(filter))
        {
            query = filter switch
            {
                "lowstock" => query.Where(s => s.QteDispo <= s.Stockmin),
                "outofstock" => query.Where(s => s.QteDispo == 0),
                _ => query
            };
        }

        var stocks = await query
            .OrderBy(s => s.QteDispo)
            .ToListAsync();

        var viewModel = stocks.Select(s => new EmployeeStockViewModel
        {
            IdSt = s.IdSt,
            IdArticle = s.IdArticle,
            NomArticle = s.IdArticleNavigation.NomArt,
            ReferenceArticle = s.IdArticleNavigation.ReferenceArt,
            Categorie = s.IdArticleNavigation.IdCatNavigation?.NomCat,
            QteDispo = s.QteDispo ?? 0,
            Stockmin = s.Stockmin ?? 0,
            Stockmax = s.Stockmax ?? 100,
            DateModification = s.DateModification,
            PrixUnit = s.IdArticleNavigation.PrixUnit
        }).ToList();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.Filter = filter;

        return View(viewModel);
    }

    /// <summary>
    /// Affiche le formulaire de mise à jour du stock
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public async Task<IActionResult> UpdateStock(int id)
    {
        var stock = await _context.Stocks
            .Include(s => s.IdArticleNavigation)
            .FirstOrDefaultAsync(s => s.IdSt == id);

        if (stock == null)
        {
            return NotFound();
        }

        var viewModel = new UpdateStockViewModel
        {
            IdSt = stock.IdSt,
            NomArticle = stock.IdArticleNavigation.NomArt,
            CurrentQty = stock.QteDispo ?? 0,
            NewQty = stock.QteDispo ?? 0
        };

        return View(viewModel);
    }

    /// <summary>
    /// Met à jour la quantité en stock
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public async Task<IActionResult> UpdateStock(UpdateStockViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var stock = await _context.Stocks
            .Include(s => s.IdArticleNavigation)
            .FirstOrDefaultAsync(s => s.IdSt == model.IdSt);

        if (stock == null)
        {
            return NotFound();
        }

        stock.QteDispo = model.NewQty;
        stock.DateModification = DateOnly.FromDateTime(DateTime.Now);

        // Créer une notification si le stock est bas
        if (model.NewQty <= stock.Stockmin)
        {
            var notification = new NotificationStock
            {
                IdArticle = stock.IdArticle,
                Msg = $"Stock faible pour {stock.IdArticleNavigation.NomArt}: {model.NewQty} unités restantes",
                DateNot = DateTime.Now,
                Vu = false
            };
            _context.NotificationStocks.Add(notification);
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Stock mis à jour avec succès";
        return RedirectToAction(nameof(StockManagement));
    }

    /// <summary>
    /// Affiche les notifications de stock
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public async Task<IActionResult> StockNotifications(bool showRead = false)
    {
        var query = _context.NotificationStocks
            .Include(n => n.IdArticleNavigation)
            .ThenInclude(a => a.Stocks)
            .AsQueryable();

        if (!showRead)
        {
            query = query.Where(n => n.Vu == false || n.Vu == null);
        }

        var notifications = await query
            .OrderByDescending(n => n.DateNot)
            .ToListAsync();

        var viewModel = notifications.Select(n => new StockNotificationViewModel
        {
            IdNot = n.IdNot,
            IdArticle = n.IdArticle,
            NomArticle = n.IdArticleNavigation.NomArt,
            ReferenceArticle = n.IdArticleNavigation.ReferenceArt,
            Message = n.Msg,
            DateNotification = n.DateNot,
            Vu = n.Vu ?? false,
            QteDispo = n.IdArticleNavigation.Stocks.FirstOrDefault()?.QteDispo,
            Stockmin = n.IdArticleNavigation.Stocks.FirstOrDefault()?.Stockmin
        }).ToList();

        ViewBag.ShowRead = showRead;

        return View(viewModel);
    }

    /// <summary>
    /// Marque une notification comme lue
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public async Task<IActionResult> MarkNotificationAsRead(int id)
    {
        var notification = await _context.NotificationStocks.FindAsync(id);
        if (notification != null)
        {
            notification.Vu = true;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(StockNotifications));
    }

    #endregion

    #region Gestion des Achats

    /// <summary>
    /// Gestion des achats - Accessible uniquement aux Administrateurs et Responsables d'achat
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.ResponsableAchat}")]
    public async Task<IActionResult> PurchaseManagement(DateTime? startDate, DateTime? endDate, int? supplierId)
    {
        var query = _context.Achats
            .Include(a => a.IdFourniNavigation)
            .Include(a => a.DetailAchats)
            .AsQueryable();

        // Filtres
        if (startDate.HasValue)
        {
            query = query.Where(a => a.DateAchat >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.DateAchat <= endDate.Value);
        }

        if (supplierId.HasValue)
        {
            query = query.Where(a => a.IdFourni == supplierId.Value);
        }

        var achats = await query
            .OrderByDescending(a => a.DateAchat)
            .ToListAsync();

        var viewModel = achats.Select(a => new EmployeePurchaseViewModel
        {
            IdAchat = a.IdAchat,
            DateAchat = a.DateAchat,
            NomFournisseur = $"{a.IdFourniNavigation.PrenomFourni} {a.IdFourniNavigation.NomFourni}",
            TelFournisseur = a.IdFourniNavigation.TelFourni,
            Total = a.TotalAch,
            NombreArticles = a.DetailAchats.Count
        }).ToList();

        // Liste des fournisseurs pour le filtre
        ViewBag.Suppliers = await _context.Fournisseurs
            .Select(f => new SelectListItem
            {
                Value = f.IdFourni.ToString(),
                Text = $"{f.PrenomFourni} {f.NomFourni}"
            })
            .ToListAsync();

        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;
        ViewBag.SupplierId = supplierId;

        return View(viewModel);
    }

    /// <summary>
    /// Affiche les détails d'un achat
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.ResponsableAchat}")]
    public async Task<IActionResult> PurchaseDetails(int id)
    {
        var achat = await _context.Achats
            .Include(a => a.IdFourniNavigation)
            .Include(a => a.DetailAchats)
            .ThenInclude(d => d.IdArticleNavigation)
            .FirstOrDefaultAsync(a => a.IdAchat == id);

        if (achat == null)
        {
            return NotFound();
        }

        var viewModel = new EmployeePurchaseViewModel
        {
            IdAchat = achat.IdAchat,
            DateAchat = achat.DateAchat,
            NomFournisseur = $"{achat.IdFourniNavigation.PrenomFourni} {achat.IdFourniNavigation.NomFourni}",
            TelFournisseur = achat.IdFourniNavigation.TelFourni,
            Total = achat.TotalAch,
            NombreArticles = achat.DetailAchats.Count,
            Details = achat.DetailAchats.Select(d => new EmployeePurchaseDetailViewModel
            {
                IdDa = d.IdDa,
                NomArticle = d.IdArticleNavigation.NomArt,
                ReferenceArticle = d.IdArticleNavigation.ReferenceArt,
                Quantite = d.QteDa,
                PrixUnitaire = d.QteDa > 0 ? d.MontantDa / d.QteDa : 0,
                Montant = d.MontantDa
            }).ToList()
        };

        return View(viewModel);
    }

    /// <summary>
    /// Affiche le formulaire de création d'achat
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.ResponsableAchat}")]
    public async Task<IActionResult> CreatePurchase()
    {
        ViewBag.Suppliers = await _context.Fournisseurs
            .Select(f => new SelectListItem
            {
                Value = f.IdFourni.ToString(),
                Text = $"{f.PrenomFourni} {f.NomFourni}"
            })
            .ToListAsync();

        ViewBag.Articles = await _context.Articles
            .Where(a => a.IdCat != null)
            .Select(a => new SelectListItem
            {
                Value = a.IdArticle.ToString(),
                Text = $"{a.NomArt} ({a.ReferenceArt})"
            })
            .ToListAsync();

        return View(new CreatePurchaseViewModel());
    }

    /// <summary>
    /// Crée un nouvel achat
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.ResponsableAchat}")]
    public async Task<IActionResult> CreatePurchase(CreatePurchaseViewModel model)
    {
        if (!ModelState.IsValid || model.Items == null || !model.Items.Any())
        {
            if (!model.Items?.Any() == true)
            {
                ModelState.AddModelError("", "Veuillez ajouter au moins un article");
            }

            ViewBag.Suppliers = await _context.Fournisseurs
                .Select(f => new SelectListItem
                {
                    Value = f.IdFourni.ToString(),
                    Text = $"{f.PrenomFourni} {f.NomFourni}"
                })
                .ToListAsync();

            ViewBag.Articles = await _context.Articles
                .Select(a => new SelectListItem
                {
                    Value = a.IdArticle.ToString(),
                    Text = $"{a.NomArt} ({a.ReferenceArt})"
                })
                .ToListAsync();

            return View(model);
        }

        // Créer l'achat
        var achat = new Achat
        {
            DateAchat = model.DateAchat,
            IdFourni = model.IdFournisseur,
            TotalAch = model.Items.Sum(i => i.Montant)
        };

        _context.Achats.Add(achat);
        await _context.SaveChangesAsync();

        // Créer les détails de l'achat et mettre à jour le stock
        foreach (var item in model.Items)
        {
            var detail = new DetailAchat
            {
                IdAchat = achat.IdAchat,
                IdArticle = item.IdArticle,
                QteDa = item.Quantite,
                MontantDa = item.Montant
            };
            _context.DetailAchats.Add(detail);

            // Mettre à jour le stock
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.IdArticle == item.IdArticle);
            if (stock != null)
            {
                stock.QteDispo = (stock.QteDispo ?? 0) + item.Quantite;
                stock.DateModification = DateOnly.FromDateTime(DateTime.Now);
            }
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Achat créé avec succès";
        return RedirectToAction(nameof(PurchaseManagement));
    }

    #endregion

    #region Gestion des Commandes (Pour Magasinier)

    /// <summary>
    /// Affiche toutes les commandes - Pour le magasinier qui prépare les commandes
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public async Task<IActionResult> Orders(string status, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Ventes
            .Include(v => v.IdClientNavigation)
            .Include(v => v.IdPaymentNavigation)
            .Include(v => v.DetailVentes)
            .AsQueryable();

        // Filtres
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(v => v.Status == status);
        }

        if (startDate.HasValue)
        {
            query = query.Where(v => v.DateVente >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(v => v.DateVente <= endDate.Value);
        }

        var orders = await query
            .OrderByDescending(v => v.DateVente)
            .ToListAsync();

        var viewModel = orders.Select(v => new EmployeeOrderViewModel
        {
            IdVente = v.IdVente,
            DateVente = v.DateVente,
            ClientName = $"{v.IdClientNavigation.PrenomClient} {v.IdClientNavigation.NomClient}",
            ClientPhone = v.IdClientNavigation.TelClient,
            AdresseLiv = v.AdresseLiv,
            TotalV = v.TotalV,
            Status = v.Status ?? "En traitement",
            PaymentMethod = v.IdPaymentNavigation?.Methode,
            PaymentStatus = v.IdPaymentNavigation?.EstPaye == true ? "Payé" : "En attente",
            ItemCount = v.DetailVentes.Count
        }).ToList();

        // Liste des statuts pour le filtre
        ViewBag.Statuses = new SelectList(new[]
        {
            new { Value = "En traitement", Text = "En traitement" },
            new { Value = "En préparation", Text = "En préparation" },
            new { Value = "Prête", Text = "Prête" },
            new { Value = "Expédiée", Text = "Expédiée" },
            new { Value = "Livrée", Text = "Livrée" }
        }, "Value", "Text");

        ViewBag.Status = status;
        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;

        return View(viewModel);
    }

    /// <summary>
    /// Affiche les détails d'une commande
    /// </summary>
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var vente = await _context.Ventes
            .Include(v => v.IdClientNavigation)
            .Include(v => v.IdPaymentNavigation)
            .Include(v => v.DetailVentes)
                .ThenInclude(dv => dv.IdArticleNavigation)
                    .ThenInclude(a => a.Stocks)
            .FirstOrDefaultAsync(v => v.IdVente == id);

        if (vente == null)
        {
            return NotFound();
        }

        var viewModel = new EmployeeOrderViewModel
        {
            IdVente = vente.IdVente,
            DateVente = vente.DateVente,
            ClientName = $"{vente.IdClientNavigation.PrenomClient} {vente.IdClientNavigation.NomClient}",
            ClientEmail = vente.IdClientNavigation.MailClient,
            ClientPhone = vente.IdClientNavigation.TelClient,
            ClientAddress = vente.IdClientNavigation.AdresseClient,
            AdresseLiv = vente.AdresseLiv,
            TotalV = vente.TotalV,
            Status = vente.Status ?? "En traitement",
            PaymentMethod = vente.IdPaymentNavigation?.Methode,
            PaymentStatus = vente.IdPaymentNavigation?.EstPaye == true ? "Payé" : "En attente",
            ItemCount = vente.DetailVentes.Count,
            Items = vente.DetailVentes.Select(dv => new EmployeeOrderItemViewModel
            {
                IdArticle = dv.IdArticle,
                ProductName = dv.IdArticleNavigation?.NomArt,
                ProductReference = dv.IdArticleNavigation?.ReferenceArt,
                ImagePath = dv.IdArticleNavigation?.ImagePath,
                Quantity = dv.QteDv ?? 0,
                UnitPrice = dv.QteDv > 0 ? (dv.MontantDv ?? 0) / dv.QteDv.Value : 0,
                Subtotal = dv.MontantDv ?? 0,
                StockAvailable = dv.IdArticleNavigation?.Stocks.FirstOrDefault()?.QteDispo ?? 0
            }).ToList()
        };

        return View(viewModel);
    }

    /// <summary>
    /// Met à jour le statut d'une commande
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
    public async Task<IActionResult> UpdateOrderStatus(int id, string newStatus)
    {
        var vente = await _context.Ventes.FindAsync(id);
        if (vente == null)
        {
            return Json(new { success = false, message = "Commande introuvable" });
        }

        vente.Status = newStatus;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} status updated to {Status} by {User}", 
            id, newStatus, User.Identity?.Name);

        return Json(new { success = true, message = $"Statut mis à jour: {newStatus}" });
    }

    #endregion

    #region Gestion de la Présence

    /// <summary>
    /// Affiche les présences
    /// </summary>
    public async Task<IActionResult> Presence(DateOnly? date)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !user.EmployeId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var employe = await _context.Employes
            .FirstOrDefaultAsync(e => e.IdUtilisateur == user.EmployeId.Value);

        if (employe == null)
        {
            TempData["ErrorMessage"] = "Aucune fiche employé trouvée";
            return RedirectToAction(nameof(Index));
        }

        var selectedDate = date ?? DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
        var endDate = DateOnly.FromDateTime(DateTime.Now);

        var presences = await _context.Presences
            .Where(p => p.IdUtilisateur == employe.IdUtilisateur && p.DateP >= selectedDate && p.DateP <= endDate)
            .OrderByDescending(p => p.DateP)
            .ToListAsync();

        var viewModel = presences.Select(p => new PresenceViewModel
        {
            IdPr = p.IdPr,
            NomEmploye = $"{employe.PrenomEmp} {employe.NomEmp}",
            Cin = employe.Cin,
            Date = p.DateP ?? DateOnly.FromDateTime(DateTime.Now),
            HeureArrive = p.HeureArrive,
            HeureDepart = p.HeureDepart
        }).ToList();

        ViewBag.SelectedDate = selectedDate;
        ViewBag.EmployeId = employe.IdUtilisateur;

        return View(viewModel);
    }

    /// <summary>
    /// Enregistre l'arrivée
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !user.EmployeId.HasValue)
        {
            TempData["ErrorMessage"] = "Utilisateur non trouvé ou non lié à un employé";
            _logger.LogWarning("CheckIn attempt failed - User: {User}, EmployeId: {EmployeId}", 
                User.Identity?.Name, user?.EmployeId);
            return RedirectToAction(nameof(Index));
        }

        var employe = await _context.Employes
            .FirstOrDefaultAsync(e => e.IdUtilisateur == user.EmployeId.Value);

        if (employe == null)
        {
            TempData["ErrorMessage"] = "Aucune fiche employé trouvée";
            _logger.LogError("CheckIn failed - No employee found for EmployeId: {EmployeId}", user.EmployeId.Value);
            return RedirectToAction(nameof(Index));
        }

        var aujourdhui = DateOnly.FromDateTime(DateTime.Now);
        var presenceExistante = await _context.Presences
            .FirstOrDefaultAsync(p => p.IdUtilisateur == employe.IdUtilisateur && p.DateP == aujourdhui);

        if (presenceExistante != null)
        {
            TempData["ErrorMessage"] = "Vous avez déjà enregistré votre arrivée aujourd'hui";
            return RedirectToAction(nameof(Index));
        }

        var presence = new Presence
        {
            IdUtilisateur = employe.IdUtilisateur,
            DateP = aujourdhui,
            HeureArrive = TimeOnly.FromDateTime(DateTime.Now)
        };

        _context.Presences.Add(presence);
        await _context.SaveChangesAsync();

        _logger.LogInformation("CheckIn successful for employee {EmployeId} at {Time}", 
            employe.IdUtilisateur, presence.HeureArrive);

        TempData["SuccessMessage"] = "Arrivée enregistrée avec succès";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Enregistre le départ
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(int idPr)
    {
        var presence = await _context.Presences.FindAsync(idPr);
        if (presence == null)
        {
            TempData["ErrorMessage"] = "Présence introuvable";
            return RedirectToAction(nameof(Index));
        }

        if (presence.HeureDepart.HasValue)
        {
            TempData["ErrorMessage"] = "Vous avez déjà enregistré votre départ";
            return RedirectToAction(nameof(Index));
        }

        presence.HeureDepart = TimeOnly.FromDateTime(DateTime.Now);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Départ enregistré avec succès";
        return RedirectToAction(nameof(Index));
    }

    #endregion

    /// <summary>
    /// Administration - Accessible uniquement aux Administrateurs
    /// </summary>
    [Authorize(Policy = RoleConstants.AdminPolicy)]
    public IActionResult Administration()
    {
        return RedirectToAction("Index", "Admin");
    }
}
