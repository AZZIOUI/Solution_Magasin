using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel pour la gestion des stocks par les employés
/// </summary>
public class EmployeeStockViewModel
{
    public int IdSt { get; set; }
    
    public int IdArticle { get; set; }
    
    [Display(Name = "Article")]
    public string? NomArticle { get; set; }
    
    [Display(Name = "Référence")]
    public string? ReferenceArticle { get; set; }
    
    [Display(Name = "Catégorie")]
    public string? Categorie { get; set; }
    
    [Required(ErrorMessage = "La quantité disponible est requise")]
    [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être positive")]
    [Display(Name = "Quantité Disponible")]
    public int QteDispo { get; set; }
    
    [Required(ErrorMessage = "Le stock minimum est requis")]
    [Range(0, int.MaxValue, ErrorMessage = "Le stock minimum doit être positif")]
    [Display(Name = "Stock Minimum")]
    public int Stockmin { get; set; }
    
    [Required(ErrorMessage = "Le stock maximum est requis")]
    [Range(1, int.MaxValue, ErrorMessage = "Le stock maximum doit être supérieur à 0")]
    [Display(Name = "Stock Maximum")]
    public int Stockmax { get; set; }
    
    [Display(Name = "Dernière Modification")]
    public DateOnly? DateModification { get; set; }
    
    [Display(Name = "Prix Unitaire")]
    public double? PrixUnit { get; set; }
    
    public bool IsLowStock => QteDispo <= Stockmin;
    public bool IsOutOfStock => QteDispo == 0;
    public int StockPercentage => Stockmax > 0 ? (QteDispo * 100 / Stockmax) : 0;
}

/// <summary>
/// ViewModel pour mettre à jour les quantités en stock
/// </summary>
public class UpdateStockViewModel
{
    public int IdSt { get; set; }
    
    [Display(Name = "Article")]
    public string? NomArticle { get; set; }
    
    [Display(Name = "Quantité Actuelle")]
    public int CurrentQty { get; set; }
    
    [Required(ErrorMessage = "La nouvelle quantité est requise")]
    [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être positive")]
    [Display(Name = "Nouvelle Quantité")]
    public int NewQty { get; set; }
    
    [Display(Name = "Motif de modification")]
    [StringLength(200, ErrorMessage = "Le motif ne peut pas dépasser 200 caractères")]
    public string? Motif { get; set; }
}

/// <summary>
/// ViewModel pour les notifications de stock
/// </summary>
public class StockNotificationViewModel
{
    public int IdNot { get; set; }
    
    public int IdArticle { get; set; }
    
    [Display(Name = "Article")]
    public string? NomArticle { get; set; }
    
    [Display(Name = "Référence")]
    public string? ReferenceArticle { get; set; }
    
    [Display(Name = "Message")]
    public string? Message { get; set; }
    
    [Display(Name = "Date")]
    public DateTime? DateNotification { get; set; }
    
    [Display(Name = "Lu")]
    public bool Vu { get; set; }
    
    [Display(Name = "Quantité Disponible")]
    public int? QteDispo { get; set; }
    
    [Display(Name = "Stock Minimum")]
    public int? Stockmin { get; set; }
}
