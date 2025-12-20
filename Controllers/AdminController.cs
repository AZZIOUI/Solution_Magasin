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
/// Contrôleur pour l'administration - gestion complète du système
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
    /// Page d'accueil de l'administration avec statistiques complètes
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        var statistics = new AdminStatisticsViewModel
        {
            TotalUsers = _userManager.Users.Count(),
            TotalClients = _userManager.Users.Count(u => u.UserType == "Client"),
            TotalEmployees = _userManager.Users.Count(u => u.UserType == "Employe"),
            TotalProducts = await _dbContext.Articles.CountAsync(),
            TotalCategories = await _dbContext.Categories.CountAsync(),
            TotalSuppliers = await _dbContext.Fournisseurs.CountAsync(),
            TodaySales = await _dbContext.Ventes.CountAsync(v => v.DateVente.HasValue && v.DateVente.Value.Date == today),
            MonthSales = await _dbContext.Ventes.CountAsync(v => v.DateVente.HasValue && v.DateVente.Value >= firstDayOfMonth),
            TodayRevenue = await _dbContext.Ventes.Where(v => v.DateVente.HasValue && v.DateVente.Value.Date == today).SumAsync(v => v.TotalV ?? 0),
            MonthRevenue = await _dbContext.Ventes.Where(v => v.DateVente.HasValue && v.DateVente.Value >= firstDayOfMonth).SumAsync(v => v.TotalV ?? 0),
            OutOfStockProducts = await _dbContext.Stocks.CountAsync(s => s.QteDispo == 0),
            LowStockProducts = await _dbContext.Stocks.CountAsync(s => s.QteDispo > 0 && s.QteDispo <= s.Stockmin)
        };

        // Dernières ventes
        statistics.RecentSales = await _dbContext.Ventes
            .Include(v => v.IdClientNavigation)
            .Include(v => v.DetailVentes)
            .OrderByDescending(v => v.DateVente)
            .Take(5)
            .Select(v => new SalesViewModel
            {
                Id = v.IdVente,
                SaleDate = v.DateVente,
                ClientName = $"{v.IdClientNavigation.PrenomClient} {v.IdClientNavigation.NomClient}",
                Total = v.TotalV ?? 0,
                Status = v.Status ?? "En cours",
                ItemCount = v.DetailVentes.Count
            })
            .ToListAsync();

        // Alertes stock
        statistics.StockAlerts = await _dbContext.Stocks
            .Include(s => s.IdArticleNavigation)
            .ThenInclude(a => a.IdCatNavigation)
            .Where(s => s.QteDispo <= s.Stockmin)
            .OrderBy(s => s.QteDispo)
            .Take(10)
            .Select(s => new StockViewModel
            {
                Id = s.IdSt,
                ArticleId = s.IdArticle,
                ProductName = s.IdArticleNavigation.NomArt ?? "",
                Reference = s.IdArticleNavigation.ReferenceArt ?? "",
                Quantity = s.QteDispo ?? 0,
                MinQuantity = s.Stockmin ?? 0,
                CategoryName = s.IdArticleNavigation.IdCatNavigation != null ? s.IdArticleNavigation.IdCatNavigation.NomCat : ""
            })
            .ToListAsync();

        return View(statistics);
    }

    #region Gestion des Employés

    /// <summary>
    /// Affiche le formulaire de création d'un employé
    /// </summary>
    [HttpGet]
    public IActionResult CreateEmployee()
    {
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
            var employe = new Employe
            {
                NomEmp = model.LastName,
                PrenomEmp = model.FirstName,
                Cin = model.CIN,
                DateEmbauche = DateOnly.FromDateTime(DateTime.Now)
            };

            _dbContext.Employes.Add(employe);
            await _dbContext.SaveChangesAsync();

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserType = "Employe",
                EmployeId = employe.IdUtilisateur,
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                
                _logger.LogInformation("Nouveau compte employé créé: {Email}, EmployeId: {EmployeId}, Role: {Role}", 
                    model.Email, employe.IdUtilisateur, model.Role);

                TempData["SuccessMessage"] = $"Employé {model.FirstName} {model.LastName} créé avec succès (Rôle: {model.Role})";
                return RedirectToAction(nameof(Index));
            }

            _dbContext.Employes.Remove(employe);
            await _dbContext.SaveChangesAsync();

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

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

    #endregion

    #region Gestion des Produits

    /// <summary>
    /// Liste tous les produits
    /// </summary>
    public async Task<IActionResult> Products()
    {
        var products = await _dbContext.Articles
            .Include(a => a.IdCatNavigation)
            .Include(a => a.Stocks)
            .Select(a => new ProductViewModel
            {
                Id = a.IdArticle,
                Reference = a.ReferenceArt ?? "",
                Name = a.NomArt ?? "",
                Designation = a.DesignationArt,
                UnitPrice = a.PrixUnit ?? 0,
                DateAdded = a.DateAjout,
                CategoryId = a.IdCat ?? 0,
                CategoryName = a.IdCatNavigation != null ? a.IdCatNavigation.NomCat : "",
                StockQuantity = a.Stocks.FirstOrDefault() != null ? a.Stocks.FirstOrDefault()!.QteDispo ?? 0 : 0
            })
            .ToListAsync();

        return View(products);
    }

    /// <summary>
    /// Affiche le formulaire de création d'un produit
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        ViewBag.Categories = new SelectList(await _dbContext.Categories.ToListAsync(), "IdCat", "NomCat");
        return View();
    }

    /// <summary>
    /// Traite la création d'un nouveau produit
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var article = new Article
            {
                ReferenceArt = model.Reference,
                NomArt = model.Name,
                DesignationArt = model.Designation,
                PrixUnit = model.UnitPrice,
                DateAjout = DateOnly.FromDateTime(DateTime.Now),
                IdCat = model.CategoryId
            };

            _dbContext.Articles.Add(article);
            await _dbContext.SaveChangesAsync();

            // Créer une entrée de stock initiale
            var stock = new Stock
            {
                IdArticle = article.IdArticle,
                QteDispo = 0,
                Stockmin = 10,
                Stockmax = 1000,
                DateModification = DateOnly.FromDateTime(DateTime.Now)
            };

            _dbContext.Stocks.Add(stock);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Produit '{model.Name}' créé avec succès";
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = new SelectList(await _dbContext.Categories.ToListAsync(), "IdCat", "NomCat");
        return View(model);
    }

    /// <summary>
    /// Affiche le formulaire d'édition d'un produit
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        var article = await _dbContext.Articles
            .Include(a => a.IdCatNavigation)
            .FirstOrDefaultAsync(a => a.IdArticle == id);

        if (article == null)
        {
            TempData["ErrorMessage"] = "Produit non trouvé";
            return RedirectToAction(nameof(Products));
        }

        var model = new ProductViewModel
        {
            Id = article.IdArticle,
            Reference = article.ReferenceArt ?? "",
            Name = article.NomArt ?? "",
            Designation = article.DesignationArt,
            UnitPrice = article.PrixUnit ?? 0,
            DateAdded = article.DateAjout,
            CategoryId = article.IdCat ?? 0
        };

        ViewBag.Categories = new SelectList(await _dbContext.Categories.ToListAsync(), "IdCat", "NomCat", model.CategoryId);
        return View(model);
    }

    /// <summary>
    /// Traite la mise à jour d'un produit
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var article = await _dbContext.Articles.FindAsync(model.Id);
            if (article == null)
            {
                TempData["ErrorMessage"] = "Produit non trouvé";
                return RedirectToAction(nameof(Products));
            }

            article.ReferenceArt = model.Reference;
            article.NomArt = model.Name;
            article.DesignationArt = model.Designation;
            article.PrixUnit = model.UnitPrice;
            article.IdCat = model.CategoryId;

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Produit '{model.Name}' mis à jour avec succès";
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = new SelectList(await _dbContext.Categories.ToListAsync(), "IdCat", "NomCat", model.CategoryId);
        return View(model);
    }

    /// <summary>
    /// Supprime un produit
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var article = await _dbContext.Articles
            .Include(a => a.DetailVentes)
            .Include(a => a.DetailAchats)
            .FirstOrDefaultAsync(a => a.IdArticle == id);

        if (article == null)
        {
            TempData["ErrorMessage"] = "Produit non trouvé";
            return RedirectToAction(nameof(Products));
        }

        if (article.DetailVentes.Any() || article.DetailAchats.Any())
        {
            TempData["ErrorMessage"] = "Impossible de supprimer ce produit car il est référencé dans des ventes ou achats";
            return RedirectToAction(nameof(Products));
        }

        _dbContext.Articles.Remove(article);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Produit supprimé avec succès";
        return RedirectToAction(nameof(Products));
    }

    #endregion

    #region Gestion des Fournisseurs

    /// <summary>
    /// Liste tous les fournisseurs
    /// </summary>
    public async Task<IActionResult> Suppliers()
    {
        var suppliers = await _dbContext.Fournisseurs
            .Include(f => f.Achats)
            .Select(f => new SupplierViewModel
            {
                Id = f.IdFourni,
                CIN = f.Cin ?? "",
                LastName = f.NomFourni ?? "",
                FirstName = f.PrenomFourni ?? "",
                Address = f.AdresseFourni ?? "",
                Phone = f.TelFourni ?? "",
                Email = f.MailFourni ?? "",
                PurchaseCount = f.Achats.Count
            })
            .ToListAsync();

        return View(suppliers);
    }

    /// <summary>
    /// Affiche le formulaire de création d'un fournisseur
    /// </summary>
    [HttpGet]
    public IActionResult CreateSupplier()
    {
        return View();
    }

    /// <summary>
    /// Traite la création d'un nouveau fournisseur
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSupplier(SupplierViewModel model)
    {
        if (ModelState.IsValid)
        {
            var fournisseur = new Fournisseur
            {
                Cin = model.CIN,
                NomFourni = model.LastName,
                PrenomFourni = model.FirstName,
                AdresseFourni = model.Address,
                TelFourni = model.Phone,
                MailFourni = model.Email
            };

            _dbContext.Fournisseurs.Add(fournisseur);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Fournisseur '{model.FirstName} {model.LastName}' créé avec succès";
            return RedirectToAction(nameof(Suppliers));
        }

        return View(model);
    }

    /// <summary>
    /// Affiche le formulaire d'édition d'un fournisseur
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditSupplier(int id)
    {
        var fournisseur = await _dbContext.Fournisseurs.FindAsync(id);

        if (fournisseur == null)
        {
            TempData["ErrorMessage"] = "Fournisseur non trouvé";
            return RedirectToAction(nameof(Suppliers));
        }

        var model = new SupplierViewModel
        {
            Id = fournisseur.IdFourni,
            CIN = fournisseur.Cin ?? "",
            LastName = fournisseur.NomFourni ?? "",
            FirstName = fournisseur.PrenomFourni ?? "",
            Address = fournisseur.AdresseFourni ?? "",
            Phone = fournisseur.TelFourni ?? "",
            Email = fournisseur.MailFourni ?? ""
        };

        return View(model);
    }

    /// <summary>
    /// Traite la mise à jour d'un fournisseur
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSupplier(SupplierViewModel model)
    {
        if (ModelState.IsValid)
        {
            var fournisseur = await _dbContext.Fournisseurs.FindAsync(model.Id);
            if (fournisseur == null)
            {
                TempData["ErrorMessage"] = "Fournisseur non trouvé";
                return RedirectToAction(nameof(Suppliers));
            }

            fournisseur.Cin = model.CIN;
            fournisseur.NomFourni = model.LastName;
            fournisseur.PrenomFourni = model.FirstName;
            fournisseur.AdresseFourni = model.Address;
            fournisseur.TelFourni = model.Phone;
            fournisseur.MailFourni = model.Email;

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Fournisseur '{model.FirstName} {model.LastName}' mis à jour avec succès";
            return RedirectToAction(nameof(Suppliers));
        }

        return View(model);
    }

    /// <summary>
    /// Supprime un fournisseur
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var fournisseur = await _dbContext.Fournisseurs
            .Include(f => f.Achats)
            .FirstOrDefaultAsync(f => f.IdFourni == id);

        if (fournisseur == null)
        {
            TempData["ErrorMessage"] = "Fournisseur non trouvé";
            return RedirectToAction(nameof(Suppliers));
        }

        if (fournisseur.Achats.Any())
        {
            TempData["ErrorMessage"] = "Impossible de supprimer ce fournisseur car il a des achats associés";
            return RedirectToAction(nameof(Suppliers));
        }

        _dbContext.Fournisseurs.Remove(fournisseur);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Fournisseur supprimé avec succès";
        return RedirectToAction(nameof(Suppliers));
    }

    #endregion

    #region Gestion des Catégories

    /// <summary>
    /// Liste toutes les catégories
    /// </summary>
    public async Task<IActionResult> Categories()
    {
        var categories = await _dbContext.Categories
            .Include(c => c.Articles)
            .Select(c => new CategoryViewModel
            {
                Id = c.IdCat,
                Name = c.NomCat ?? "",
                Description = c.DescriptionCat,
                ProductCount = c.Articles.Count
            })
            .ToListAsync();

        return View(categories);
    }

    /// <summary>
    /// Affiche le formulaire de création d'une catégorie
    /// </summary>
    [HttpGet]
    public IActionResult CreateCategory()
    {
        return View();
    }

    /// <summary>
    /// Traite la création d'une nouvelle catégorie
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var categorie = new Categorie
            {
                NomCat = model.Name,
                DescriptionCat = model.Description
            };

            _dbContext.Categories.Add(categorie);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Catégorie '{model.Name}' créée avec succès";
            return RedirectToAction(nameof(Categories));
        }

        return View(model);
    }

    /// <summary>
    /// Affiche le formulaire d'édition d'une catégorie
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        var categorie = await _dbContext.Categories.FindAsync(id);

        if (categorie == null)
        {
            TempData["ErrorMessage"] = "Catégorie non trouvée";
            return RedirectToAction(nameof(Categories));
        }

        var model = new CategoryViewModel
        {
            Id = categorie.IdCat,
            Name = categorie.NomCat ?? "",
            Description = categorie.DescriptionCat
        };

        return View(model);
    }

    /// <summary>
    /// Traite la mise à jour d'une catégorie
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(CategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var categorie = await _dbContext.Categories.FindAsync(model.Id);
            if (categorie == null)
            {
                TempData["ErrorMessage"] = "Catégorie non trouvée";
                return RedirectToAction(nameof(Categories));
            }

            categorie.NomCat = model.Name;
            categorie.DescriptionCat = model.Description;

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Catégorie '{model.Name}' mise à jour avec succès";
            return RedirectToAction(nameof(Categories));
        }

        return View(model);
    }

    /// <summary>
    /// Supprime une catégorie
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var categorie = await _dbContext.Categories
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.IdCat == id);

        if (categorie == null)
        {
            TempData["ErrorMessage"] = "Catégorie non trouvée";
            return RedirectToAction(nameof(Categories));
        }

        if (categorie.Articles.Any())
        {
            TempData["ErrorMessage"] = "Impossible de supprimer cette catégorie car elle contient des produits";
            return RedirectToAction(nameof(Categories));
        }

        _dbContext.Categories.Remove(categorie);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Catégorie supprimée avec succès";
        return RedirectToAction(nameof(Categories));
    }

    #endregion

    #region Gestion du Stock

    /// <summary>
    /// Liste tous les stocks
    /// </summary>
    public async Task<IActionResult> Stock()
    {
        var stocks = await _dbContext.Stocks
            .Include(s => s.IdArticleNavigation)
            .ThenInclude(a => a.IdCatNavigation)
            .Select(s => new StockViewModel
            {
                Id = s.IdSt,
                ArticleId = s.IdArticle,
                ProductName = s.IdArticleNavigation.NomArt ?? "",
                Reference = s.IdArticleNavigation.ReferenceArt ?? "",
                Quantity = s.QteDispo ?? 0,
                MinQuantity = s.Stockmin ?? 0,
                LastUpdated = s.DateModification.HasValue ? s.DateModification.Value.ToDateTime(TimeOnly.MinValue) : null,
                CategoryName = s.IdArticleNavigation.IdCatNavigation != null ? s.IdArticleNavigation.IdCatNavigation.NomCat : ""
            })
            .OrderBy(s => s.Quantity)
            .ToListAsync();

        return View(stocks);
    }

    /// <summary>
    /// Affiche le formulaire de mise à jour du stock
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditStock(int id)
    {
        var stock = await _dbContext.Stocks
            .Include(s => s.IdArticleNavigation)
            .FirstOrDefaultAsync(s => s.IdSt == id);

        if (stock == null)
        {
            TempData["ErrorMessage"] = "Stock non trouvé";
            return RedirectToAction(nameof(Stock));
        }

        var model = new StockViewModel
        {
            Id = stock.IdSt,
            ArticleId = stock.IdArticle,
            ProductName = stock.IdArticleNavigation.NomArt ?? "",
            Reference = stock.IdArticleNavigation.ReferenceArt ?? "",
            Quantity = stock.QteDispo ?? 0,
            MinQuantity = stock.Stockmin ?? 0
        };

        return View(model);
    }

    /// <summary>
    /// Traite la mise à jour du stock
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStock(StockViewModel model)
    {
        if (ModelState.IsValid)
        {
            var stock = await _dbContext.Stocks.FindAsync(model.Id);
            if (stock == null)
            {
                TempData["ErrorMessage"] = "Stock non trouvé";
                return RedirectToAction(nameof(Stock));
            }

            stock.QteDispo = model.Quantity;
            stock.Stockmin = model.MinQuantity;
            stock.DateModification = DateOnly.FromDateTime(DateTime.Now);

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Stock mis à jour avec succès";
            return RedirectToAction(nameof(Stock));
        }

        return View(model);
    }

    #endregion

    #region Consultation des Achats

    /// <summary>
    /// Liste tous les achats
    /// </summary>
    public async Task<IActionResult> Purchases()
    {
        var purchases = await _dbContext.Achats
            .Include(a => a.IdFourniNavigation)
            .Include(a => a.DetailAchats)
            .OrderByDescending(a => a.DateAchat)
            .Select(a => new PurchaseViewModel
            {
                Id = a.IdAchat,
                PurchaseDate = a.DateAchat,
                SupplierName = $"{a.IdFourniNavigation.PrenomFourni} {a.IdFourniNavigation.NomFourni}",
                Total = a.TotalAch ?? 0,
                ItemCount = a.DetailAchats.Count
            })
            .ToListAsync();

        return View(purchases);
    }

    /// <summary>
    /// Détails d'un achat
    /// </summary>
    public async Task<IActionResult> PurchaseDetails(int id)
    {
        var achat = await _dbContext.Achats
            .Include(a => a.IdFourniNavigation)
            .Include(a => a.DetailAchats)
            .ThenInclude(d => d.IdArticleNavigation)
            .FirstOrDefaultAsync(a => a.IdAchat == id);

        if (achat == null)
        {
            TempData["ErrorMessage"] = "Achat non trouvé";
            return RedirectToAction(nameof(Purchases));
        }

        var model = new PurchaseViewModel
        {
            Id = achat.IdAchat,
            PurchaseDate = achat.DateAchat,
            SupplierName = $"{achat.IdFourniNavigation.PrenomFourni} {achat.IdFourniNavigation.NomFourni}",
            Total = achat.TotalAch ?? 0,
            ItemCount = achat.DetailAchats.Count,
            Details = achat.DetailAchats.Select(d => new PurchaseDetailViewModel
            {
                ProductName = d.IdArticleNavigation.NomArt ?? "",
                Reference = d.IdArticleNavigation.ReferenceArt ?? "",
                Quantity = d.QteDa ?? 0,
                UnitPrice = d.QteDa > 0 ? (d.MontantDa ?? 0) / d.QteDa.Value : 0,
                Subtotal = d.MontantDa ?? 0
            }).ToList()
        };

        return View(model);
    }

    #endregion

    #region Consultation des Ventes

    /// <summary>
    /// Liste toutes les ventes
    /// </summary>
    public async Task<IActionResult> Sales()
    {
        var sales = await _dbContext.Ventes
            .Include(v => v.IdClientNavigation)
            .Include(v => v.IdPaymentNavigation)
            .Include(v => v.DetailVentes)
            .OrderByDescending(v => v.DateVente)
            .Select(v => new SalesViewModel
            {
                Id = v.IdVente,
                SaleDate = v.DateVente,
                ClientName = $"{v.IdClientNavigation.PrenomClient} {v.IdClientNavigation.NomClient}",
                Total = v.TotalV ?? 0,
                Status = v.Status ?? "En cours",
                DeliveryAddress = v.AdresseLiv,
                ItemCount = v.DetailVentes.Count,
                PaymentMethod = v.IdPaymentNavigation != null ? v.IdPaymentNavigation.Methode : "Non spécifié"
            })
            .ToListAsync();

        return View(sales);
    }

    /// <summary>
    /// Détails d'une vente
    /// </summary>
    public async Task<IActionResult> SaleDetails(int id)
    {
        var vente = await _dbContext.Ventes
            .Include(v => v.IdClientNavigation)
            .Include(v => v.IdPaymentNavigation)
            .Include(v => v.DetailVentes)
            .ThenInclude(d => d.IdArticleNavigation)
            .FirstOrDefaultAsync(v => v.IdVente == id);

        if (vente == null)
        {
            TempData["ErrorMessage"] = "Vente non trouvée";
            return RedirectToAction(nameof(Sales));
        }

        var model = new SalesViewModel
        {
            Id = vente.IdVente,
            SaleDate = vente.DateVente,
            ClientName = $"{vente.IdClientNavigation.PrenomClient} {vente.IdClientNavigation.NomClient}",
            Total = vente.TotalV ?? 0,
            Status = vente.Status ?? "En cours",
            DeliveryAddress = vente.AdresseLiv,
            ItemCount = vente.DetailVentes.Count,
            PaymentMethod = vente.IdPaymentNavigation != null ? vente.IdPaymentNavigation.Methode : "Non spécifié",
            Details = vente.DetailVentes.Select(d => new SaleDetailViewModel
            {
                ProductName = d.IdArticleNavigation.NomArt ?? "",
                Reference = d.IdArticleNavigation.ReferenceArt ?? "",
                Quantity = d.QteDv ?? 0,
                UnitPrice = d.QteDv > 0 ? (d.MontantDv ?? 0) / d.QteDv.Value : 0,
                Subtotal = d.MontantDv ?? 0
            }).ToList()
        };

        return View(model);
    }

    /// <summary>
    /// Met à jour le statut d'une vente
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSaleStatus(int id, string status)
    {
        var vente = await _dbContext.Ventes.FindAsync(id);
        if (vente == null)
        {
            TempData["ErrorMessage"] = "Vente non trouvée";
            return RedirectToAction(nameof(Sales));
        }

        vente.Status = status;
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Statut de la vente mis à jour: {status}";
        return RedirectToAction(nameof(SaleDetails), new { id });
    }

    #endregion
}
