using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour la gestion des catégories
/// </summary>
public class CategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom de la catégorie est requis")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    [Display(Name = "Nom de la catégorie")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La description ne peut pas dépasser 200 caractères")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Nombre de produits")]
    public int ProductCount { get; set; }
}
