using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour l'inscription
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Le prénom est requis")]
    [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    [Display(Name = "Nom")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est requise")]
    [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères")]
    [Display(Name = "Adresse")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le téléphone est requis")]
    [StringLength(50, ErrorMessage = "Le téléphone ne peut pas dépasser 50 caractères")]
    [Phone(ErrorMessage = "Format de téléphone invalide")]
    [Display(Name = "Téléphone")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [StringLength(100, ErrorMessage = "Le mot de passe doit contenir au moins {2} caractères et au maximum {1} caractères.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmez le mot de passe")]
    [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
