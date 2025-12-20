# ?? RÉFÉRENCE RAPIDE - ESPACE ADMINISTRATEUR

## ?? Routes Principales

### Tableau de Bord
```
GET /Admin/Index - Tableau de bord avec statistiques
```

### Produits
```
GET  /Admin/Products              - Liste des produits
GET  /Admin/CreateProduct         - Formulaire création
POST /Admin/CreateProduct         - Traitement création
GET  /Admin/EditProduct/{id}      - Formulaire modification
POST /Admin/EditProduct           - Traitement modification
POST /Admin/DeleteProduct/{id}    - Suppression
```

### Catégories
```
GET  /Admin/Categories            - Liste des catégories
GET  /Admin/CreateCategory        - Formulaire création
POST /Admin/CreateCategory        - Traitement création
GET  /Admin/EditCategory/{id}     - Formulaire modification
POST /Admin/EditCategory          - Traitement modification
POST /Admin/DeleteCategory/{id}   - Suppression
```

### Fournisseurs
```
GET  /Admin/Suppliers             - Liste des fournisseurs
GET  /Admin/CreateSupplier        - Formulaire création
POST /Admin/CreateSupplier        - Traitement création
GET  /Admin/EditSupplier/{id}     - Formulaire modification
POST /Admin/EditSupplier          - Traitement modification
POST /Admin/DeleteSupplier/{id}   - Suppression
```

### Stock
```
GET  /Admin/Stock                 - Liste du stock
GET  /Admin/EditStock/{id}        - Formulaire modification
POST /Admin/EditStock             - Traitement modification
```

### Achats
```
GET /Admin/Purchases              - Liste des achats
GET /Admin/PurchaseDetails/{id}   - Détails d'un achat
```

### Ventes
```
GET  /Admin/Sales                 - Liste des ventes
GET  /Admin/SaleDetails/{id}      - Détails d'une vente
POST /Admin/UpdateSaleStatus      - Modifier statut vente
```

### Utilisateurs
```
GET  /Admin/Users                 - Liste des utilisateurs
GET  /Admin/CreateEmployee        - Formulaire création employé
POST /Admin/CreateEmployee        - Traitement création employé
```

## ?? ViewModels

### ProductViewModel
```csharp
- Id: int
- Reference: string
- Name: string
- Designation: string?
- UnitPrice: double
- DateAdded: DateOnly?
- CategoryId: int
- CategoryName: string?
- StockQuantity: int
```

### SupplierViewModel
```csharp
- Id: int
- CIN: string
- LastName: string
- FirstName: string
- Address: string
- Phone: string
- Email: string
- PurchaseCount: int
```

### CategoryViewModel
```csharp
- Id: int
- Name: string
- Description: string?
- ProductCount: int
```

### StockViewModel
```csharp
- Id: int
- ArticleId: int
- ProductName: string
- Reference: string
- Quantity: int
- MinQuantity: int
- LastUpdated: DateTime?
- Status: string (calculé)
- CategoryName: string?
```

### AdminStatisticsViewModel
```csharp
- TotalUsers: int
- TotalClients: int
- TotalEmployees: int
- TotalProducts: int
- TotalCategories: int
- TotalSuppliers: int
- TodaySales: int
- MonthSales: int
- TodayRevenue: double
- MonthRevenue: double
- OutOfStockProducts: int
- LowStockProducts: int
- RecentSales: List<SalesViewModel>
- PopularProducts: List<ProductViewModel>
- StockAlerts: List<StockViewModel>
```

## ??? Propriétés Base de Données

### Stock (Table)
```csharp
- IdSt: int (PK)
- QteDispo: int?       // Quantité disponible
- Stockmin: int?       // Stock minimum
- Stockmax: int?       // Stock maximum
- DateModification: DateOnly?
- IdArticle: int (FK)
```

### DetailAchat (Table)
```csharp
- IdDa: int (PK)
- QteDa: int?          // Quantité
- MontantDa: double?   // Montant total
- IdAchat: int (FK)
- IdArticle: int (FK)
```

### DetailVente (Table)
```csharp
- IdDv: int (PK)
- QteDv: int?          // Quantité
- MontantDv: double?   // Montant total
- IdVente: int (FK)
- IdArticle: int (FK)
```

### Payment (Table)
```csharp
- IdPayment: int (PK)
- Methode: string?     // Méthode de paiement
- DatePayment: DateTime?
- EstPaye: bool?
```

## ?? Classes CSS Bootstrap Utilisées

### Badges de Statut
```html
<!-- Stock -->
<span class="badge bg-success">Stock normal</span>
<span class="badge bg-warning text-dark">Stock faible</span>
<span class="badge bg-danger">Rupture de stock</span>

<!-- Vente -->
<span class="badge bg-success">Livrée</span>
<span class="badge bg-warning">En cours</span>
<span class="badge bg-danger">Annulée</span>
```

### Boutons
```html
<!-- Actions CRUD -->
<a class="btn btn-primary">Créer</a>
<a class="btn btn-outline-primary btn-sm">Modifier</a>
<button class="btn btn-outline-danger btn-sm">Supprimer</button>
<a class="btn btn-secondary">Retour</a>
```

### Cartes
```html
<div class="card shadow-sm">
  <div class="card-header bg-primary text-white">
    <h5 class="mb-0">Titre</h5>
  </div>
  <div class="card-body">
    Contenu
  </div>
</div>
```

## ?? JavaScript/jQuery

### DataTables Configuration
```javascript
$('#tableId').DataTable({
    language: {
        url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/fr-FR.json'
    },
    order: [[0, 'asc']],
    pageLength: 25
});
```

## ??? Sécurité

### Contrôleur
```csharp
[Authorize(Policy = RoleConstants.AdminPolicy)]
public class AdminController : Controller
```

### Formulaires
```html
<form asp-action="ActionName" method="post">
    @Html.AntiForgeryToken()
    <!-- Champs du formulaire -->
</form>
```

## ?? Icons Bootstrap

```html
<!-- Navigation -->
<i class="bi bi-speedometer2"></i>    <!-- Dashboard -->
<i class="bi bi-box-seam"></i>        <!-- Produits -->
<i class="bi bi-tags"></i>            <!-- Catégories -->
<i class="bi bi-boxes"></i>           <!-- Stock -->
<i class="bi bi-truck"></i>           <!-- Fournisseurs -->
<i class="bi bi-cart-check"></i>      <!-- Achats -->
<i class="bi bi-bag-check"></i>       <!-- Ventes -->
<i class="bi bi-people"></i>          <!-- Utilisateurs -->

<!-- Actions -->
<i class="bi bi-plus-circle"></i>     <!-- Créer -->
<i class="bi bi-pencil"></i>          <!-- Modifier -->
<i class="bi bi-trash"></i>           <!-- Supprimer -->
<i class="bi bi-eye"></i>             <!-- Voir -->
<i class="bi bi-arrow-left"></i>      <!-- Retour -->
<i class="bi bi-check-circle"></i>    <!-- Confirmer -->
```

## ? Exemples de Code

### Créer un Produit (Controller)
```csharp
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
            // ...
        };
        _dbContext.Articles.Add(article);
        await _dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Produit créé avec succès";
        return RedirectToAction(nameof(Products));
    }
    return View(model);
}
```

### Afficher les Alertes
```html
@if (TempData["SuccessMessage"] != null)
{
    <div class="alert alert-success alert-dismissible fade show">
        <i class="bi bi-check-circle"></i> @TempData["SuccessMessage"]
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
}
```

## ?? Calculs Importants

### Prix Unitaire (depuis détails)
```csharp
UnitPrice = quantity > 0 ? montant / quantity : 0
```

### Statut Stock
```csharp
Status = quantity == 0 ? "Rupture de stock" 
       : quantity <= minQuantity ? "Stock faible" 
       : "Stock normal"
```

### Conversion Date
```csharp
// DateOnly ? DateTime
LastUpdated = dateOnly.HasValue ? dateOnly.Value.ToDateTime(TimeOnly.MinValue) : null

// DateTime ? DateOnly
DateModification = DateOnly.FromDateTime(DateTime.Now)
```

## ?? Support

Pour toute question sur l'implémentation :
1. Consulter **ADMIN_SPACE_COMPLETE.md** pour la documentation complète
2. Vérifier les ViewModels dans `ViewModels/`
3. Consulter le contrôleur `Controllers/AdminController.cs`
4. Examiner les vues dans `Views/Admin/`

---
**Version:** 1.0  
**Date:** 2025  
**Status:** ? Complet et Opérationnel
