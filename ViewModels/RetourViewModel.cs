using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for product return management
/// </summary>
public class RetourViewModel
{
    public int IdRetour { get; set; }
    public string? Motif { get; set; }
    public DateTime? DateRetour { get; set; }
    public int IdArticle { get; set; }
    public int IdVente { get; set; }
    
    // Product information
    public string? ProductName { get; set; }
    public string? ProductReference { get; set; }
    public string? ProductImage { get; set; }
    public double? ProductPrice { get; set; }
    
    // Order information
    public string? OrderNumber { get; set; }
    public DateTime? OrderDate { get; set; }
    
    // Status
    public string? Status { get; set; }
}

/// <summary>
/// ViewModel for requesting a product return
/// </summary>
public class RequestReturnViewModel
{
    [Required(ErrorMessage = "Veuillez sélectionner un article")]
    public int IdArticle { get; set; }
    
    [Required(ErrorMessage = "Veuillez sélectionner une commande")]
    public int IdVente { get; set; }
    
    [Required(ErrorMessage = "Le motif du retour est obligatoire")]
    [StringLength(200, ErrorMessage = "Le motif ne peut pas dépasser 200 caractères")]
    public string Motif { get; set; } = string.Empty;
    
    // For display purposes
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
    public double? ProductPrice { get; set; }
    public int? QuantityPurchased { get; set; }
}

/// <summary>
/// ViewModel for return history
/// </summary>
public class ReturnHistoryViewModel
{
    public List<RetourViewModel> Returns { get; set; } = new();
    public int TotalReturns { get; set; }
}
