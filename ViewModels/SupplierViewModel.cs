using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour la gestion des fournisseurs
/// </summary>
public class SupplierViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le CIN est requis")]
    [StringLength(30, ErrorMessage = "Le CIN ne peut pas dépasser 30 caractères")]
    [Display(Name = "CIN")]
    public string CIN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    [Display(Name = "Nom")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est requis")]
    [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est requise")]
    [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères")]
    [Display(Name = "Adresse")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le téléphone est requis")]
    [StringLength(50, ErrorMessage = "Le téléphone ne peut pas dépasser 50 caractères")]
    [Phone(ErrorMessage = "Format de téléphone invalide")]
    [Display(Name = "Téléphone")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [StringLength(200, ErrorMessage = "L'email ne peut pas dépasser 200 caractères")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Nombre d'achats")]
    public int PurchaseCount { get; set; }
}
