using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour l'affichage des achats
/// </summary>
public class PurchaseViewModel
{
    [Display(Name = "ID Achat")]
    public int Id { get; set; }

    [Display(Name = "Date d'achat")]
    public DateTime? PurchaseDate { get; set; }

    [Display(Name = "Fournisseur")]
    public string SupplierName { get; set; } = string.Empty;

    [Display(Name = "Total")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public double Total { get; set; }

    [Display(Name = "Nombre d'articles")]
    public int ItemCount { get; set; }

    [Display(Name = "Détails")]
    public List<PurchaseDetailViewModel> Details { get; set; } = new();
}

/// <summary>
/// Détail d'un achat
/// </summary>
public class PurchaseDetailViewModel
{
    [Display(Name = "Article")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Référence")]
    public string Reference { get; set; } = string.Empty;

    [Display(Name = "Quantité")]
    public int Quantity { get; set; }

    [Display(Name = "Prix unitaire")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public double UnitPrice { get; set; }

    [Display(Name = "Sous-total")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public double Subtotal { get; set; }
}
