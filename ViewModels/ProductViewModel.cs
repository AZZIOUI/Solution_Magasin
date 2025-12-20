using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour la gestion des produits/articles
/// </summary>
public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La référence est requise")]
    [StringLength(100, ErrorMessage = "La référence ne peut pas dépasser 100 caractères")]
    [Display(Name = "Référence")]
    public string Reference { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    [Display(Name = "Nom du produit")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La désignation ne peut pas dépasser 200 caractères")]
    [Display(Name = "Désignation")]
    public string? Designation { get; set; }

    [Required(ErrorMessage = "Le prix unitaire est requis")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
    [Display(Name = "Prix unitaire")]
    public double UnitPrice { get; set; }

    [Display(Name = "Date d'ajout")]
    public DateOnly? DateAdded { get; set; }

    [Required(ErrorMessage = "La catégorie est requise")]
    [Display(Name = "Catégorie")]
    public int CategoryId { get; set; }

    [Display(Name = "Catégorie")]
    public string? CategoryName { get; set; }

    [Display(Name = "Quantité en stock")]
    public int StockQuantity { get; set; }
}
