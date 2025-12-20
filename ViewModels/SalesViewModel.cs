using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour l'affichage des ventes
/// </summary>
public class SalesViewModel
{
    [Display(Name = "ID Vente")]
    public int Id { get; set; }

    [Display(Name = "Date de vente")]
    public DateTime? SaleDate { get; set; }

    [Display(Name = "Client")]
    public string ClientName { get; set; } = string.Empty;

    [Display(Name = "Total")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public double Total { get; set; }

    [Display(Name = "Statut")]
    public string Status { get; set; } = string.Empty;

    [Display(Name = "Adresse de livraison")]
    public string? DeliveryAddress { get; set; }

    [Display(Name = "Nombre d'articles")]
    public int ItemCount { get; set; }

    [Display(Name = "Méthode de paiement")]
    public string? PaymentMethod { get; set; }

    [Display(Name = "Détails")]
    public List<SaleDetailViewModel> Details { get; set; } = new();
}

/// <summary>
/// Détail d'une vente
/// </summary>
public class SaleDetailViewModel
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
