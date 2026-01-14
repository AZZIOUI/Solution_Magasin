# ?? Quick Reference: Retour, Review & Facture

## ?? URL Quick Access

| Feature | URL | Description |
|---------|-----|-------------|
| **Mes Retours** | `/Client/MyReturns` | View all returns |
| **Demander un retour** | `/Client/RequestReturn?orderId={id}&articleId={id}` | Request return form |
| **Détails du retour** | `/Client/ReturnDetails/{id}` | Return details |
| **Mes Avis** | `/Client/MyReviews` | View all reviews |
| **Télécharger facture** | `/Client/ViewInvoice?orderId={id}` | Download invoice |

---

## ?? Key Features at a Glance

### Returns (Retour)
- ? Request return for purchased products
- ? View return history with status
- ? See product info and return motif
- ? Protected: can't return same product twice

### Reviews (Avis)
- ? Rate products 1-5 stars
- ? Write comments (10-300 chars)
- ? View all your reviews with stats
- ? Delete your reviews
- ? Protected: must purchase to review, one review per product

### Invoices (Facture)
- ? HTML invoice with professional layout
- ? Download from order list or details
- ? Print-friendly format
- ? Includes TVA calculation (20%)
- ? Auto-generated on first access

---

## ??? Database Schema Quick Reference

### Retour Table
```sql
- IdRetour (int, PK)
- Motif (varchar(200))
- DateRetour (datetime)
- IdArticle (int, FK)
- IdVente (int, FK)
```

### Review Table
```sql
- IdReview (int, PK)
- Comment (varchar(300))
- Rating (int, 1-5)
- IdClient (int, FK)
- IdArticle (int, FK)
```

### Facture Table
```sql
- IdFacture (int, PK)
- CodeFacture (varchar(100))
- DateFacture (date)
- MontantTotal (float)
- FilePath (varchar(300))
- IdVente (int, FK)
```

---

## ?? Code Snippets

### Request a Return
```csharp
// From Order Details view
<a asp-action="RequestReturn" 
   asp-route-orderId="@Model.IdVente" 
   asp-route-articleId="@item.IdArticle"
   class="btn btn-outline-warning">
    <i class="bi bi-arrow-return-left"></i> Retour
</a>
```

### Submit a Review (AJAX)
```javascript
$.post('@Url.Action("AddReview", "Client")', {
    IdArticle: articleId,
    Rating: rating,
    Comment: comment,
    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
})
```

### Download Invoice
```csharp
// From Order List or Details
<a asp-action="ViewInvoice" 
   asp-route-orderId="@order.IdVente" 
   class="btn btn-success" 
   target="_blank">
    <i class="bi bi-file-earmark-pdf"></i> Facture
</a>
```

---

## ?? Security Rules

| Feature | Security Check |
|---------|----------------|
| **Returns** | User can only return products from their own orders |
| **Reviews** | User must have purchased the product |
| **Reviews** | One review per product per user |
| **Invoices** | User can only download invoices for their own orders |
| **All** | `[Authorize(Policy = RoleConstants.ClientPolicy)]` required |

---

## ?? UI Components

### Return Card
- Product image
- Product name & reference
- Order number & date
- Return date & motif
- Status badge
- Action button (View details)

### Review Card
- Product image
- Product name & reference
- Star rating (filled/empty stars)
- Comment text
- Date
- Delete button

### Invoice Layout
- Company header
- Invoice number & date
- Client information
- Products table (ref, name, qty, price, total)
- Subtotal, TVA, Grand Total
- Payment & shipping info
- Print button

---

## ?? Testing Quick Checks

### Returns
```bash
1. View /Client/MyReturns ? Should show empty state or list
2. Go to order details ? Click "Retour" on a product
3. Fill motif ? Submit
4. Check /Client/MyReturns ? Return should appear
5. Try requesting same return again ? Should be blocked
```

### Reviews
```bash
1. View /Client/MyReviews ? Should show empty state or list
2. Go to order details ? Click "Avis" on a product
3. Select stars (1-5) ? Write comment ? Submit
4. Check /Client/MyReviews ? Review should appear
5. Click delete ? Confirm ? Review should be removed
6. Try reviewing same product again ? Should be blocked
```

### Invoices
```bash
1. View /Client/MyOrders ? Click "Facture" on an order
2. Invoice opens in new tab ? Check all sections
3. Print preview ? Should be print-friendly
4. Download again ? Same invoice (check by invoice number)
5. Check database ? Facture record should exist
6. Check wwwroot/invoices ? HTML file should exist
```

---

## ?? Common Queries

### Get All Returns for a Client
```csharp
var returns = await _dbContext.Retours
    .Include(r => r.IdArticleNavigation)
    .Include(r => r.IdVenteNavigation)
    .Where(r => r.IdVenteNavigation.IdClient == clientId)
    .ToListAsync();
```

### Get All Reviews for a Client
```csharp
var reviews = await _dbContext.Reviews
    .Include(r => r.IdArticleNavigation)
    .Where(r => r.IdClient == clientId)
    .ToListAsync();
```

### Generate Invoice
```csharp
var invoice = await _invoiceService.CreateInvoiceAsync(venteId);
var html = await _invoiceService.GenerateInvoiceHtmlAsync(venteId);
```

---

## ?? Troubleshooting

### Returns Not Showing
- ? Check if user has made any orders
- ? Verify `IdVente` and `IdArticle` are correct
- ? Check database: `SELECT * FROM Retour WHERE id_vente IN (SELECT id_vente FROM Vente WHERE id_client = ?)`

### Review Submit Fails
- ? Ensure user purchased the product
- ? Check if review already exists
- ? Validate rating (1-5) and comment length (10-300)
- ? Check anti-forgery token is present

### Invoice Not Generating
- ? Check `InvoiceService` is registered in `Program.cs`
- ? Verify `wwwroot/invoices` directory exists
- ? Check write permissions on invoices folder
- ? Verify `Vente` record exists with proper relationships

---

## ?? Configuration

### Invoice Settings (InvoiceService.cs)
```csharp
// Company name
const string COMPANY_NAME = "AZZIOUI MAGASIN";

// TVA rate
const double TVA_RATE = 0.2; // 20%

// Invoice file location
var invoicesDir = Path.Combine(_environment.WebRootPath, "invoices");
```

### Invoice Number Format
```csharp
// Format: INV-XXXXXX (6 digits, zero-padded)
var invoiceCode = $"INV-{venteId:D6}";
// Example: INV-000001, INV-000042, INV-001234
```

---

## ?? Bootstrap Classes Used

| Component | Classes |
|-----------|---------|
| Cards | `card`, `card-body`, `shadow-sm` |
| Badges | `badge bg-success/warning/primary` |
| Buttons | `btn btn-primary/success/warning` |
| Icons | `bi bi-arrow-return-left`, `bi-star`, `bi-file-earmark-pdf` |
| Modals | `modal`, `modal-dialog`, `modal-content` |
| Forms | `form-control`, `form-label`, `mb-3` |

---

## ? Performance Tips

### Returns
- Returns are filtered by client on database side
- Uses eager loading (`.Include()`) to avoid N+1 queries
- Pagination recommended if return count grows

### Reviews
- Reviews use projection (`.Select()`) for efficiency
- Average rating calculated in memory (few records)
- Consider caching if review count is high

### Invoices
- Invoice HTML generated once, cached in file system
- Database lookup first to check if invoice exists
- File-based caching avoids regeneration

---

## ?? Integration Points

### From Order Details
```csharp
// Return button
<a asp-action="RequestReturn" 
   asp-route-orderId="@Model.IdVente" 
   asp-route-articleId="@item.IdArticle">

// Review button (opens modal)
<button class="btn-review" 
        data-article-id="@item.IdArticle">

// Invoice button
<a asp-action="ViewInvoice" 
   asp-route-orderId="@Model.IdVente">
```

### From Navigation Menu
```html
<li><a asp-action="MyReturns">Mes Retours</a></li>
<li><a asp-action="MyReviews">Mes Avis</a></li>
```

### From Dashboard
```html
<a asp-action="MyReturns" class="btn btn-warning">
    Voir mes retours
</a>
```

---

## ?? Related Documentation

- **Full Implementation**: See `RETOUR_REVIEW_FACTURE_COMPLETE.md`
- **Original Specs**: See `VISITOR_CLIENT_IMPLEMENTATION_SUMMARY.md`
- **Database Schema**: See `Models/` folder for entity definitions
- **Service Layer**: See `Services/InvoiceService.cs`

---

## ?? Summary

**3 Features Implemented:**
1. ?? **Retour** - Product returns
2. ? **Review** - Product reviews
3. ?? **Facture** - Invoice generation

**Build Status**: ? SUCCESS  
**Ready**: ? YES  
**Tested**: ?? MANUAL TESTING RECOMMENDED

---

**Need help?** Check the full documentation in `RETOUR_REVIEW_FACTURE_COMPLETE.md`
