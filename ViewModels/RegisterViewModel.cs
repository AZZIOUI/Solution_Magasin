using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [StringLength(30, ErrorMessage = "Le CIN ne peut pas dépasser 30 caractères")]
        [Display(Name = "CIN")]
        public string? Cin { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Format de téléphone invalide")]
        [Display(Name = "Numéro de téléphone")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, ErrorMessage = "Le mot de passe doit contenir au moins {2} caractères", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "S'inscrire en tant que client")]
        public bool RegisterAsClient { get; set; } = true;
    }
}
