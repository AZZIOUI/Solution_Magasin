using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour la gestion du stock
/// </summary>
public class StockViewModel
{
    public int Id { get; set; }

    [Display(Name = "Article")]
    public int ArticleId { get; set; }

    [Display(Name = "Nom du produit")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Référence")]
    public string Reference { get; set; } = string.Empty;

    [Required(ErrorMessage = "La quantité est requise")]
    [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être positive")]
    [Display(Name = "Quantité")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "La quantité minimale est requise")]
    [Range(0, int.MaxValue, ErrorMessage = "La quantité minimale doit être positive")]
    [Display(Name = "Quantité minimale")]
    public int MinQuantity { get; set; }

    [Display(Name = "Date de dernière mise à jour")]
    public DateTime? LastUpdated { get; set; }

    [Display(Name = "Statut")]
    public string Status
    {
        get
        {
            if (Quantity == 0) return "Rupture de stock";
            if (Quantity <= MinQuantity) return "Stock faible";
            return "Stock normal";
        }
    }

    [Display(Name = "Catégorie")]
    public string? CategoryName { get; set; }
}
