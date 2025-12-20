using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel pour créer un nouvel achat
/// </summary>
public class CreatePurchaseViewModel
{
    [Required(ErrorMessage = "Le fournisseur est requis")]
    [Display(Name = "Fournisseur")]
    public int IdFournisseur { get; set; }
    
    [Display(Name = "Date d'Achat")]
    public DateTime DateAchat { get; set; } = DateTime.Now;
    
    [Display(Name = "Articles")]
    public List<PurchaseItemViewModel> Items { get; set; } = new();
    
    [Display(Name = "Total")]
    public double Total { get; set; }
}

/// <summary>
/// ViewModel pour les articles d'un achat
/// </summary>
public class PurchaseItemViewModel
{
    [Required(ErrorMessage = "L'article est requis")]
    [Display(Name = "Article")]
    public int IdArticle { get; set; }
    
    [Display(Name = "Nom de l'Article")]
    public string? NomArticle { get; set; }
    
    [Required(ErrorMessage = "La quantité est requise")]
    [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0")]
    [Display(Name = "Quantité")]
    public int Quantite { get; set; }
    
    [Required(ErrorMessage = "Le prix unitaire est requis")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
    [Display(Name = "Prix Unitaire")]
    public double PrixUnitaire { get; set; }
    
    [Display(Name = "Montant")]
    public double Montant => Quantite * PrixUnitaire;
}

/// <summary>
/// ViewModel pour afficher les achats
/// </summary>
public class EmployeePurchaseViewModel
{
    public int IdAchat { get; set; }
    
    [Display(Name = "Date d'Achat")]
    public DateTime? DateAchat { get; set; }
    
    [Display(Name = "Fournisseur")]
    public string? NomFournisseur { get; set; }
    
    [Display(Name = "Contact Fournisseur")]
    public string? TelFournisseur { get; set; }
    
    [Display(Name = "Total")]
    [DisplayFormat(DataFormatString = "{0:N2} MAD")]
    public double? Total { get; set; }
    
    [Display(Name = "Nombre d'Articles")]
    public int NombreArticles { get; set; }
    
    public List<EmployeePurchaseDetailViewModel> Details { get; set; } = new();
}

/// <summary>
/// ViewModel pour les détails d'un achat (version employé)
/// </summary>
public class EmployeePurchaseDetailViewModel
{
    public int IdDa { get; set; }
    
    [Display(Name = "Article")]
    public string? NomArticle { get; set; }
    
    [Display(Name = "Référence")]
    public string? ReferenceArticle { get; set; }
    
    [Display(Name = "Quantité")]
    public int? Quantite { get; set; }
    
    [Display(Name = "Prix Unitaire")]
    [DisplayFormat(DataFormatString = "{0:N2} MAD")]
    public double? PrixUnitaire { get; set; }
    
    [Display(Name = "Montant")]
    [DisplayFormat(DataFormatString = "{0:N2} MAD")]
    public double? Montant { get; set; }
}

/// <summary>
/// ViewModel pour les fournisseurs (liste simplifiée)
/// </summary>
public class SupplierListItemViewModel
{
    public int IdFourni { get; set; }
    
    [Display(Name = "Fournisseur")]
    public string? NomComplet { get; set; }
    
    [Display(Name = "Téléphone")]
    public string? Telephone { get; set; }
    
    [Display(Name = "Email")]
    public string? Email { get; set; }
}
