# Visitor & Client Space - Quick Reference

## URLs Quick Access

### Visitor (Public)
| Page | URL | Description |
|------|-----|-------------|
| Home | `/Visitor/Index` | Welcome page with featured products |
| Catalog | `/Visitor/Catalog` | Browse all products |
| Filter by Category | `/Visitor/Catalog?categoryId=1` | Products in specific category |
| Search | `/Visitor/Catalog?search=keyword` | Search products |
| Product Details | `/Visitor/ProductDetails/1` | Detailed product view |

### Client (Authenticated)
| Page | URL | Description |
|------|-----|-------------|
| Dashboard | `/Client/Index` | Client home page |
| Shopping Cart | `/Client/Cart` | View/manage cart |
| Checkout | `/Client/Checkout` | Enter shipping info |
| Payment | `/Client/Payment` | Simulated payment form |
| Order Confirmation | `/Client/OrderConfirmation/{id}` | Order success page |
| My Orders | `/Client/MyOrders` | Order history |
| Order Details | `/Client/OrderDetails/{id}` | Specific order details |
| Profile | `/Client/Profile` | Manage profile |

## Database Schema Reference

### Tables & Key Columns

```
Article
??? IdArticle (PK)
??? NomArt (Product Name)
??? PrixUnit (Price)
??? ImagePath (Image)
??? ReferenceArt (SKU)
??? DesignationArt (Description)
??? IdCat (FK ? Categorie)

Stock
??? IdSt (PK)
??? QteDispo (Available Quantity) ??
??? Stockmax (Maximum)
??? Stockmin (Minimum)
??? DateModification (Last Update)
??? IdArticle (FK ? Article)

Payment
??? IdPayment (PK)
??? Methode (Payment Method) ??
??? DatePayment (Payment Date)
??? EstPaye (Is Paid - boolean) ??

Vente
??? IdVente (PK)
??? DateVente (Sale Date)
??? TotalV (Total Amount) ??
??? AdresseLiv (Delivery Address) ??
??? Status (Order Status)
??? IdClient (FK ? Client)
??? IdPayment (FK ? Payment)

DetailVente
??? IdDv (PK)
??? QteDv (Quantity) ??
??? MontantDv (Line Total) ??
??? IdVente (FK ? Vente)
??? IdArticle (FK ? Article)

Review
??? IdReview (PK)
??? Comment (Review Text)
??? Rating (Stars 1-5)
??? IdClient (FK ? Client)
??? IdArticle (FK ? Article)
```

?? = **Important**: These property names differ from common naming conventions

## Code Snippets

### Add to Cart (AJAX)
```javascript
fetch('/Client/AddToCart', {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body: `productId=${id}&quantity=${qty}`
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        alert(data.message);
    }
});
```

### Get Cart from Session (C#)
```csharp
var cartJson = HttpContext.Session.GetString("ShoppingCart");
var cart = JsonSerializer.Deserialize<CartViewModel>(cartJson);
```

### Save Cart to Session (C#)
```csharp
var cartJson = JsonSerializer.Serialize(cart);
HttpContext.Session.SetString("ShoppingCart", cartJson);
```

### Query Products with Stock (C#)
```csharp
var products = await _dbContext.Articles
    .Include(a => a.Stocks)
    .Include(a => a.Reviews)
    .Select(a => new ProductCardViewModel
    {
        IdArticle = a.IdArticle,
        NomArt = a.NomArt,
        PrixUnit = a.PrixUnit,
        StockQuantity = a.Stocks.Sum(s => s.QteDispo),
        AverageRating = a.Reviews.Any() ? a.Reviews.Average(r => r.Rating) : null
    })
    .ToListAsync();
```

### Create Order (C#)
```csharp
// 1. Create Payment
var payment = new Payment
{
    Methode = "Carte bancaire",
    DatePayment = DateTime.Now,
    EstPaye = true
};
_dbContext.Payments.Add(payment);
await _dbContext.SaveChangesAsync();

// 2. Create Vente
var vente = new Vente
{
    DateVente = DateTime.Now,
    IdClient = clientId,
    IdPayment = payment.IdPayment,
    TotalV = totalAmount,
    AdresseLiv = shippingAddress,
    Status = "En traitement"
};
_dbContext.Ventes.Add(vente);
await _dbContext.SaveChangesAsync();

// 3. Create DetailVente for each item
foreach (var item in cartItems)
{
    var detail = new DetailVente
    {
        IdVente = vente.IdVente,
        IdArticle = item.IdArticle,
        QteDv = item.Quantity,
        MontantDv = item.TotalPrice
    };
    _dbContext.DetailVentes.Add(detail);
    
    // 4. Update stock
    var stock = await _dbContext.Stocks
        .FirstOrDefaultAsync(s => s.IdArticle == item.IdArticle);
    if (stock != null)
    {
        stock.QteDispo -= item.Quantity;
        stock.DateModification = DateOnly.FromDateTime(DateTime.Now);
    }
}
await _dbContext.SaveChangesAsync();
```

## ViewModels Hierarchy

```
ProductCatalogViewModel
??? List<ProductCardViewModel> Products
??? List<Categorie> Categories

ProductDetailsViewModel
??? Basic product info
??? List<ProductReviewViewModel> Reviews
??? List<ProductCardViewModel> RelatedProducts

CartViewModel
??? List<CartItemViewModel> Items
??? Subtotal (calculated)
??? ShippingCost (50 DH)
??? Total (calculated)

CheckoutViewModel
??? CartViewModel Cart
??? ShippingAddress
??? Phone
??? DeliveryInstructions

PaymentViewModel
??? OrderId
??? TotalAmount
??? CardHolderName
??? CardNumber
??? ExpiryDate
??? CVV

OrderHistoryViewModel
??? List<OrderSummaryViewModel> Orders

OrderDetailsViewModel
??? Order info (ID, date, status)
??? Payment info
??? Shipping address
??? List<OrderItemViewModel> Items
```

## Authorization Policies

```csharp
[Authorize(Policy = RoleConstants.ClientPolicy)]
public class ClientController : Controller
{
    // Requires "Client" role
}

// VisitorController - No authorization needed
public class VisitorController : Controller
{
    // Public access
}
```

## Session Configuration

```csharp
// Program.cs
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// In pipeline
app.UseSession(); // Before UseAuthentication()
app.UseAuthentication();
app.UseAuthorization();
```

## Bootstrap Classes Used

### Layout
- `container` / `container-fluid`: Main containers
- `row` / `col-*`: Grid system
- `g-3` / `g-4`: Gap spacing

### Cards
- `card`: Card container
- `card-header` / `card-body` / `card-footer`: Card sections
- `border-0`: Remove borders
- `shadow-sm`: Subtle shadow

### Badges
- `badge bg-primary`: Blue badge
- `badge bg-success`: Green (in stock, paid)
- `badge bg-danger`: Red (out of stock)
- `badge bg-warning`: Yellow (pending)

### Buttons
- `btn btn-primary`: Primary action
- `btn btn-outline-primary`: Secondary action
- `btn-lg`: Large button
- `w-100`: Full width

### Forms
- `form-control`: Input styling
- `form-label`: Label styling
- `form-select`: Dropdown styling

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Cart empty after adding | Session not configured | Add session middleware in Program.cs |
| Can't add to cart | Not authenticated | Ensure user logged in with Client role |
| Stock not updating | Missing SaveChanges | Call `await _dbContext.SaveChangesAsync()` |
| Wrong property name error | Using incorrect column | Use: QteDispo, EstPaye, TotalV, AdresseLiv, QteDv, MontantDv |
| Orders not showing | Client filter issue | Verify `user.ClientId` is populated |

## Key Differences from Standard Naming

| Standard | This Project | Context |
|----------|--------------|---------|
| `Quantity` | `QteDispo` | Stock available quantity |
| `QuantityStock` | `QteDispo` | Stock quantity |
| `Amount` / `TotalAmount` | `Montant` / `TotalV` | Payment/Vente total |
| `PaymentMethod` | `Methode` | Payment method |
| `IsPaid` / `PaymentStatus` | `EstPaye` | Payment status (boolean) |
| `ShippingAddress` | `AdresseLiv` | Delivery address |
| `Quantity` | `QteDv` | DetailVente quantity |
| `LineTotal` | `MontantDv` | DetailVente amount |
| `DateReview` | *(not exists)* | Use IdReview for ordering |

## Testing Workflow

### As Visitor
1. Go to `/Visitor/Index`
2. Click "Voir le catalogue"
3. Browse products
4. Filter by category
5. Search for products
6. View product details
7. Try to add to cart ? Redirect to login

### As Client
1. Register at `/Account/Register`
2. Login at `/Account/Login`
3. Browse catalog
4. Add items to cart
5. Go to cart (`/Client/Cart`)
6. Update quantities
7. Proceed to checkout
8. Enter shipping info
9. Submit payment form
10. View confirmation
11. Check "My Orders"
12. View order details

## Performance Tips

### Database
- Use `.Include()` for related data (avoid N+1 queries)
- Use `.Select()` to project only needed fields
- Add pagination for large result sets

### Session
- Keep cart items minimal (only essential data)
- Clear session after order completion
- Set appropriate timeout (30 minutes)

### Images
- Optimize image sizes
- Use placeholders for missing images
- Consider lazy loading

## File Locations

```
Controllers/
??? VisitorController.cs    # Public catalog
??? ClientController.cs      # Authenticated shopping

ViewModels/
??? ProductCatalogViewModel.cs
??? ProductDetailsViewModel.cs
??? CartViewModel.cs
??? CheckoutViewModel.cs
??? OrderViewModel.cs

Views/
??? Visitor/
?   ??? Index.cshtml        # Home
?   ??? Catalog.cshtml      # Product grid
?   ??? ProductDetails.cshtml
??? Client/
    ??? Index.cshtml        # Dashboard
    ??? Cart.cshtml         # Shopping cart
    ??? Checkout.cshtml     # Checkout form
    ??? Payment.cshtml      # Payment form
    ??? OrderConfirmation.cshtml
    ??? MyOrders.cshtml     # Order list
    ??? OrderDetails.cshtml
```

---

**Quick Help**: For detailed explanations, see `VISITOR_CLIENT_SPACE_COMPLETE.md`
