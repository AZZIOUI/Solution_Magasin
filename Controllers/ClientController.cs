using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;
using Solution_Magasin.ViewModels;
using Solution_Magasin.Services;
using System.Text.Json;

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
    private readonly InvoiceService _invoiceService;
    private const string CartSessionKey = "ShoppingCart";

    public ClientController(
        UserManager<ApplicationUser> userManager,
        DotnetProjectContext dbContext,
        ILogger<ClientController> logger,
        InvoiceService invoiceService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
        _invoiceService = invoiceService;
    }

    /// <summary>
    /// Page d'accueil de l'espace client
    /// </summary>
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Accès à l'espace client par {User}", User.Identity?.Name);
        
        var user = await _userManager.GetUserAsync(User);
        ViewBag.UserName = $"{user?.FirstName} {user?.LastName}";
        
        // Get recent orders count
        var recentOrdersCount = 0;
        if (user?.ClientId != null)
        {
            recentOrdersCount = await _dbContext.Ventes
                .Where(v => v.IdClient == user.ClientId)
                .CountAsync();
        }
        ViewBag.OrdersCount = recentOrdersCount;
        
        return View();
    }

    #region Shopping Cart

    /// <summary>
    /// Display shopping cart
    /// </summary>
    [HttpGet]
    public IActionResult Cart()
    {
        var cart = GetCartFromSession();
        return View(cart);
    }

    /// <summary>
    /// Add product to cart
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        var article = await _dbContext.Articles
            .Include(a => a.Stocks)
            .FirstOrDefaultAsync(a => a.IdArticle == productId);

        if (article == null)
        {
            return Json(new { success = false, message = "Produit introuvable" });
        }

        var availableStock = article.Stocks.Sum(s => s.QteDispo) ?? 0;
        if (availableStock < quantity)
        {
            return Json(new { success = false, message = "Stock insuffisant" });
        }

        var cart = GetCartFromSession();
        var existingItem = cart.Items.FirstOrDefault(i => i.IdArticle == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            if (existingItem.Quantity > availableStock)
            {
                existingItem.Quantity = availableStock;
            }
        }
        else
        {
            cart.Items.Add(new CartItemViewModel
            {
                IdArticle = article.IdArticle,
                ProductName = article.NomArt,
                ProductReference = article.ReferenceArt,
                ImagePath = article.ImagePath,
                UnitPrice = article.PrixUnit ?? 0,
                Quantity = quantity,
                MaxQuantity = availableStock
            });
        }

        SaveCartToSession(cart);

        return Json(new { success = true, message = "Produit ajouté au panier", cartItemCount = cart.TotalItems });
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    [HttpPost]
    public IActionResult UpdateCartItem(int productId, int quantity)
    {
        var cart = GetCartFromSession();
        var item = cart.Items.FirstOrDefault(i => i.IdArticle == productId);

        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else if (quantity <= item.MaxQuantity)
            {
                item.Quantity = quantity;
            }
            else
            {
                return Json(new { success = false, message = "Quantité demandée non disponible" });
            }

            SaveCartToSession(cart);
            return Json(new { success = true, subtotal = cart.Subtotal, total = cart.Total });
        }

        return Json(new { success = false, message = "Article introuvable dans le panier" });
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCartFromSession();
        var item = cart.Items.FirstOrDefault(i => i.IdArticle == productId);

        if (item != null)
        {
            cart.Items.Remove(item);
            SaveCartToSession(cart);
            return Json(new { success = true, message = "Article retiré du panier", cartItemCount = cart.TotalItems });
        }

        return Json(new { success = false, message = "Article introuvable" });
    }

    /// <summary>
    /// Clear all items from cart
    /// </summary>
    [HttpPost]
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return Json(new { success = true, message = "Panier vidé" });
    }

    #endregion

    #region Checkout & Payment

    /// <summary>
    /// Display checkout page
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCartFromSession();
        
        if (!cart.Items.Any())
        {
            TempData["ErrorMessage"] = "Votre panier est vide";
            return RedirectToAction(nameof(Cart));
        }

        var user = await _userManager.GetUserAsync(User);
        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.IdClient == user.ClientId);

        var viewModel = new CheckoutViewModel
        {
            Cart = cart,
            ShippingAddress = client?.AdresseClient ?? "",
            Phone = client?.TelClient ?? ""
        };

        return View(viewModel);
    }

    /// <summary>
    /// Process checkout
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = GetCartFromSession();
        model.Cart = cart;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!cart.Items.Any())
        {
            TempData["ErrorMessage"] = "Votre panier est vide";
            return RedirectToAction(nameof(Cart));
        }

        var user = await _userManager.GetUserAsync(User);
        if (user?.ClientId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Store order info in TempData for payment page (convert double to string)
        TempData["ShippingAddress"] = model.ShippingAddress;
        TempData["Phone"] = model.Phone;
        TempData["DeliveryInstructions"] = model.DeliveryInstructions;
        TempData["TotalAmount"] = cart.Total.ToString("F2"); // Convert to string to avoid serialization error

        return RedirectToAction(nameof(Payment));
    }

    /// <summary>
    /// Display payment page
    /// </summary>
    [HttpGet]
    public IActionResult Payment()
    {
        var cart = GetCartFromSession();
        
        if (!cart.Items.Any())
        {
            TempData["ErrorMessage"] = "Votre panier est vide";
            return RedirectToAction(nameof(Cart));
        }
        
        var viewModel = new PaymentViewModel
        {
            OrderId = 0, // Will be generated after successful payment
            TotalAmount = cart.Total
        };

        return View(viewModel);
    }

    /// <summary>
    /// Process payment (simulated)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Payment", model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user?.ClientId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = GetCartFromSession();
        if (!cart.Items.Any())
        {
            TempData["ErrorMessage"] = "Votre panier est vide";
            return RedirectToAction(nameof(Cart));
        }

        // Get shipping info from TempData
        var shippingAddress = TempData["ShippingAddress"]?.ToString();
        var phone = TempData["Phone"]?.ToString();
        var totalAmountStr = TempData["TotalAmount"]?.ToString();

        if (string.IsNullOrEmpty(shippingAddress))
        {
            TempData["ErrorMessage"] = "Informations de livraison manquantes";
            return RedirectToAction(nameof(Checkout));
        }

        // Parse total amount from TempData
        double totalAmount = cart.Total;
        if (!string.IsNullOrEmpty(totalAmountStr) && double.TryParse(totalAmountStr, out var parsedAmount))
        {
            totalAmount = parsedAmount;
        }

        // Create Payment record (simulated - always successful)
        var payment = new Payment
        {
            Methode = "Carte bancaire",
            DatePayment = DateTime.Now,
            EstPaye = true // Simulated payment always succeeds
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        // Create Vente record
        var vente = new Vente
        {
            DateVente = DateTime.Now,
            IdClient = user.ClientId.Value,
            IdPayment = payment.IdPayment,
            TotalV = totalAmount,
            AdresseLiv = shippingAddress,
            Status = "En traitement"
        };

        _dbContext.Ventes.Add(vente);
        await _dbContext.SaveChangesAsync();

        // Create DetailVente records and update stock
        foreach (var item in cart.Items)
        {
            var detailVente = new DetailVente
            {
                IdVente = vente.IdVente,
                IdArticle = item.IdArticle,
                QteDv = item.Quantity,
                MontantDv = item.TotalPrice
            };

            _dbContext.DetailVentes.Add(detailVente);

            // Update stock
            var stock = await _dbContext.Stocks
                .FirstOrDefaultAsync(s => s.IdArticle == item.IdArticle);
            
            if (stock != null && stock.QteDispo >= item.Quantity)
            {
                stock.QteDispo -= item.Quantity;
                stock.DateModification = DateOnly.FromDateTime(DateTime.Now);
            }
        }

        await _dbContext.SaveChangesAsync();

        // Clear cart
        HttpContext.Session.Remove(CartSessionKey);

        TempData["SuccessMessage"] = "Commande validée avec succès !";
        return RedirectToAction(nameof(OrderConfirmation), new { orderId = vente.IdVente });
    }

    /// <summary>
    /// Display order confirmation
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> OrderConfirmation(int orderId)
    {
        var user = await _userManager.GetUserAsync(User);
        
        var vente = await _dbContext.Ventes
            .Include(v => v.DetailVentes)
                .ThenInclude(dv => dv.IdArticleNavigation)
            .Include(v => v.IdPaymentNavigation)
            .FirstOrDefaultAsync(v => v.IdVente == orderId && v.IdClient == user.ClientId);

        if (vente == null)
        {
            return NotFound();
        }

        var viewModel = new OrderDetailsViewModel
        {
            IdVente = vente.IdVente,
            DateVente = vente.DateVente,
            TotalAmount = vente.TotalV,
            PaymentMethod = vente.IdPaymentNavigation?.Methode,
            PaymentStatus = vente.IdPaymentNavigation?.EstPaye == true ? "Payé" : "En attente",
            ShippingAddress = vente.AdresseLiv,
            Items = vente.DetailVentes.Select(dv => new OrderItemViewModel
            {
                IdArticle = dv.IdArticle,
                ProductName = dv.IdArticleNavigation?.NomArt,
                ProductReference = dv.IdArticleNavigation?.ReferenceArt,
                ImagePath = dv.IdArticleNavigation?.ImagePath,
                Quantity = dv.QteDv ?? 0,
                UnitPrice = dv.MontantDv ?? 0 / Math.Max(dv.QteDv ?? 1, 1)
            }).ToList()
        };

        return View(viewModel);
    }

    #endregion

    #region Orders

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
            .Select(v => new OrderSummaryViewModel
            {
                IdVente = v.IdVente,
                DateVente = v.DateVente,
                TotalAmount = v.TotalV,
                PaymentStatus = v.IdPaymentNavigation != null && v.IdPaymentNavigation.EstPaye == true ? "Payé" : "En attente",
                ItemCount = v.DetailVentes.Sum(dv => dv.QteDv) ?? 0,
                Items = v.DetailVentes.Select(dv => new OrderItemViewModel
                {
                    IdArticle = dv.IdArticle,
                    ProductName = dv.IdArticleNavigation != null ? dv.IdArticleNavigation.NomArt : null,
                    ProductReference = dv.IdArticleNavigation != null ? dv.IdArticleNavigation.ReferenceArt : null,
                    ImagePath = dv.IdArticleNavigation != null ? dv.IdArticleNavigation.ImagePath : null,
                    Quantity = dv.QteDv ?? 0,
                    UnitPrice = dv.MontantDv ?? 0 / Math.Max(dv.QteDv ?? 1, 1)
                }).ToList()
            })
            .ToListAsync();

        var viewModel = new OrderHistoryViewModel
        {
            Orders = orders
        };

        return View(viewModel);
    }

    /// <summary>
    /// Display order details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        
        var vente = await _dbContext.Ventes
            .Include(v => v.DetailVentes)
                .ThenInclude(dv => dv.IdArticleNavigation)
            .Include(v => v.IdPaymentNavigation)
            .FirstOrDefaultAsync(v => v.IdVente == id && v.IdClient == user.ClientId);

        if (vente == null)
        {
            return NotFound();
        }

        var viewModel = new OrderDetailsViewModel
        {
            IdVente = vente.IdVente,
            DateVente = vente.DateVente,
            TotalAmount = vente.TotalV,
            PaymentMethod = vente.IdPaymentNavigation?.Methode,
            PaymentStatus = vente.IdPaymentNavigation?.EstPaye == true ? "Payé" : "En attente",
            ShippingAddress = vente.AdresseLiv,
            Items = vente.DetailVentes.Select(dv => new OrderItemViewModel
            {
                IdArticle = dv.IdArticle,
                ProductName = dv.IdArticleNavigation?.NomArt,
                ProductReference = dv.IdArticleNavigation?.ReferenceArt,
                ImagePath = dv.IdArticleNavigation?.ImagePath,
                Quantity = dv.QteDv ?? 0,
                UnitPrice = dv.MontantDv ?? 0 / Math.Max(dv.QteDv ?? 1, 1)
            }).ToList()
        };

        return View(viewModel);
    }

    #endregion

    #region Profile

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

    #endregion

    #region Returns Management

    /// <summary>
    /// Display return history
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> MyReturns()
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var returns = await _dbContext.Retours
            .Include(r => r.IdArticleNavigation)
            .Include(r => r.IdVenteNavigation)
            .Where(r => r.IdVenteNavigation.IdClient == user.ClientId)
            .OrderByDescending(r => r.DateRetour)
            .Select(r => new RetourViewModel
            {
                IdRetour = r.IdRetour,
                Motif = r.Motif,
                DateRetour = r.DateRetour,
                IdArticle = r.IdArticle,
                IdVente = r.IdVente,
                ProductName = r.IdArticleNavigation.NomArt,
                ProductReference = r.IdArticleNavigation.ReferenceArt,
                ProductImage = r.IdArticleNavigation.ImagePath,
                ProductPrice = r.IdArticleNavigation.PrixUnit,
                OrderNumber = $"CMD-{r.IdVente:D6}",
                OrderDate = r.IdVenteNavigation.DateVente,
                Status = "En cours de traitement"
            })
            .ToListAsync();

        var viewModel = new ReturnHistoryViewModel
        {
            Returns = returns,
            TotalReturns = returns.Count
        };

        return View(viewModel);
    }

    /// <summary>
    /// Display return request form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> RequestReturn(int orderId, int articleId)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        // Verify order belongs to client
        var vente = await _dbContext.Ventes
            .Include(v => v.DetailVentes)
                .ThenInclude(dv => dv.IdArticleNavigation)
            .FirstOrDefaultAsync(v => v.IdVente == orderId && v.IdClient == user.ClientId);

        if (vente == null)
        {
            return NotFound();
        }

        var detailVente = vente.DetailVentes.FirstOrDefault(dv => dv.IdArticle == articleId);
        if (detailVente == null)
        {
            return NotFound();
        }

        // Check if return already exists
        var existingReturn = await _dbContext.Retours
            .AnyAsync(r => r.IdVente == orderId && r.IdArticle == articleId);

        if (existingReturn)
        {
            TempData["ErrorMessage"] = "Une demande de retour existe déjà pour cet article";
            return RedirectToAction(nameof(OrderDetails), new { id = orderId });
        }

        var article = detailVente.IdArticleNavigation;
        var viewModel = new RequestReturnViewModel
        {
            IdArticle = articleId,
            IdVente = orderId,
            ProductName = article?.NomArt,
            ProductImage = article?.ImagePath,
            ProductPrice = article?.PrixUnit,
            QuantityPurchased = detailVente.QteDv
        };

        return View(viewModel);
    }

    /// <summary>
    /// Process return request
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestReturn(RequestReturnViewModel model)
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

        // Verify order belongs to client
        var vente = await _dbContext.Ventes
            .FirstOrDefaultAsync(v => v.IdVente == model.IdVente && v.IdClient == user.ClientId);

        if (vente == null)
        {
            return NotFound();
        }

        // Create return record
        var retour = new Retour
        {
            IdArticle = model.IdArticle,
            IdVente = model.IdVente,
            Motif = model.Motif,
            DateRetour = DateTime.Now
        };

        _dbContext.Retours.Add(retour);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Votre demande de retour a été enregistrée avec succès";
        return RedirectToAction(nameof(MyReturns));
    }

    /// <summary>
    /// Display return details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ReturnDetails(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var retour = await _dbContext.Retours
            .Include(r => r.IdArticleNavigation)
            .Include(r => r.IdVenteNavigation)
            .FirstOrDefaultAsync(r => r.IdRetour == id && r.IdVenteNavigation.IdClient == user.ClientId);

        if (retour == null)
        {
            return NotFound();
        }

        var viewModel = new RetourViewModel
        {
            IdRetour = retour.IdRetour,
            Motif = retour.Motif,
            DateRetour = retour.DateRetour,
            IdArticle = retour.IdArticle,
            IdVente = retour.IdVente,
            ProductName = retour.IdArticleNavigation?.NomArt,
            ProductReference = retour.IdArticleNavigation?.ReferenceArt,
            ProductImage = retour.IdArticleNavigation?.ImagePath,
            ProductPrice = retour.IdArticleNavigation?.PrixUnit,
            OrderNumber = $"CMD-{retour.IdVente:D6}",
            OrderDate = retour.IdVenteNavigation?.DateVente,
            Status = "En cours de traitement"
        };

        return View(viewModel);
    }

    #endregion

    #region Reviews Management

    /// <summary>
    /// Display client's reviews
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> MyReviews()
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var reviews = await _dbContext.Reviews
            .Include(r => r.IdArticleNavigation)
            .Where(r => r.IdClient == user.ClientId)
            .OrderByDescending(r => r.IdReview)
            .Select(r => new ClientReviewViewModel
            {
                IdReview = r.IdReview,
                IdArticle = r.IdArticle,
                ProductName = r.IdArticleNavigation.NomArt,
                ProductImage = r.IdArticleNavigation.ImagePath,
                ProductReference = r.IdArticleNavigation.ReferenceArt,
                Rating = r.Rating ?? 0,
                Comment = r.Comment,
                DateReview = DateTime.Now // Review model doesn't have DateReview field
            })
            .ToListAsync();

        var viewModel = new ReviewHistoryViewModel
        {
            Reviews = reviews,
            TotalReviews = reviews.Count,
            AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0
        };

        return View(viewModel);
    }

    /// <summary>
    /// Add review for a product
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(AddReviewViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Données invalides" });
        }

        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return Json(new { success = false, message = "Utilisateur non authentifié" });
        }

        // Check if client has purchased this product
        var hasPurchased = await _dbContext.DetailVentes
            .Include(dv => dv.IdVenteNavigation)
            .AnyAsync(dv => dv.IdArticle == model.IdArticle && 
                           dv.IdVenteNavigation.IdClient == user.ClientId);

        if (!hasPurchased)
        {
            return Json(new { success = false, message = "Vous devez acheter ce produit avant de laisser un avis" });
        }

        // Check if review already exists
        var existingReview = await _dbContext.Reviews
            .AnyAsync(r => r.IdArticle == model.IdArticle && r.IdClient == user.ClientId);

        if (existingReview)
        {
            return Json(new { success = false, message = "Vous avez déjà laissé un avis pour ce produit" });
        }

        // Create review
        var review = new Review
        {
            IdArticle = model.IdArticle,
            IdClient = user.ClientId.Value,
            Rating = model.Rating,
            Comment = model.Comment
        };

        _dbContext.Reviews.Add(review);
        await _dbContext.SaveChangesAsync();

        return Json(new { success = true, message = "Votre avis a été enregistré avec succès" });
    }

    /// <summary>
    /// Delete a review
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return Json(new { success = false, message = "Utilisateur non authentifié" });
        }

        var review = await _dbContext.Reviews
            .FirstOrDefaultAsync(r => r.IdReview == reviewId && r.IdClient == user.ClientId);

        if (review == null)
        {
            return Json(new { success = false, message = "Avis introuvable" });
        }

        _dbContext.Reviews.Remove(review);
        await _dbContext.SaveChangesAsync();

        return Json(new { success = true, message = "Votre avis a été supprimé" });
    }

    #endregion

    #region Invoice Management

    /// <summary>
    /// Generate and download invoice
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DownloadInvoice(int orderId)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.ClientId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        // Verify order belongs to client
        var vente = await _dbContext.Ventes
            .FirstOrDefaultAsync(v => v.IdVente == orderId && v.IdClient == user.ClientId);

        if (vente == null)
        {
            return NotFound();
        }

        // Generate or get existing invoice
        var facture = await _invoiceService.GetInvoiceByVenteIdAsync(orderId);
        if (facture == null)
        {
            facture = await _invoiceService.CreateInvoiceAsync(orderId);
        }

        // Return invoice HTML
        var htmlContent = await _invoiceService.GenerateInvoiceHtmlAsync(orderId);
        return Content(htmlContent, "text/html");
    }

    /// <summary>
    /// View invoice
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ViewInvoice(int orderId)
    {
        return await DownloadInvoice(orderId);
    }

    #endregion

    #region Helper Methods

    private CartViewModel GetCartFromSession()
    {
        var cartJson = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
        {
            return new CartViewModel();
        }

        return JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
    }

    private void SaveCartToSession(CartViewModel cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString(CartSessionKey, cartJson);
    }

    #endregion
}
