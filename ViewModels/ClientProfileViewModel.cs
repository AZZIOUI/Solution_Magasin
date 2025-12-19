using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour afficher et modifier le profil client
/// </summary>
public class ClientProfileViewModel
{
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Le prénom est requis")]
    [StringLength(100)]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100)]
    [Display(Name = "Nom")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est requise")]
    [StringLength(200)]
    [Display(Name = "Adresse")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le téléphone est requis")]
    [StringLength(50)]
    [Phone]
    [Display(Name = "Téléphone")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Date de création du compte")]
    public DateTime DateCreated { get; set; }
}
