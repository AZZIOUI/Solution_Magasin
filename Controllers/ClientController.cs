using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;
using Solution_Magasin.ViewModels;
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
    private const string CartSessionKey = "ShoppingCart";

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

        // Create Payment record
        var payment = new Payment
        {
            Methode = "Carte bancaire",
            DatePayment = DateTime.Now,
            EstPaye = false
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        // Store order info in TempData for payment page
        TempData["PendingOrderId"] = payment.IdPayment;
        TempData["ShippingAddress"] = model.ShippingAddress;
        TempData["Phone"] = model.Phone;
        TempData["DeliveryInstructions"] = model.DeliveryInstructions;
        TempData["TotalAmount"] = cart.Total;

        return RedirectToAction(nameof(Payment), new { paymentId = payment.IdPayment });
    }

    /// <summary>
    /// Display payment page
    /// </summary>
    [HttpGet]
    public IActionResult Payment(int paymentId)
    {
        var cart = GetCartFromSession();
        
        var viewModel = new PaymentViewModel
        {
            OrderId = paymentId,
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

        // Get payment record
        var payment = await _dbContext.Payments.FindAsync(model.OrderId);
        if (payment == null)
        {
            TempData["ErrorMessage"] = "Paiement introuvable";
            return RedirectToAction(nameof(Cart));
        }

        // Update payment status (simulated success)
        payment.EstPaye = true;
        payment.DatePayment = DateTime.Now;

        // Get shipping info from TempData
        var shippingAddress = TempData["ShippingAddress"]?.ToString();
        var phone = TempData["Phone"]?.ToString();
        var totalAmount = TempData["TotalAmount"] as double? ?? cart.Total;

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
