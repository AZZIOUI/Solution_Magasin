using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for checkout process
/// </summary>
public class CheckoutViewModel
{
    public CartViewModel Cart { get; set; } = new();
    
    [Required(ErrorMessage = "L'adresse de livraison est obligatoire")]
    [Display(Name = "Adresse de livraison")]
    public string ShippingAddress { get; set; } = "";
    
    [Required(ErrorMessage = "Le numéro de téléphone est obligatoire")]
    [Display(Name = "Téléphone")]
    [Phone(ErrorMessage = "Format de téléphone invalide")]
    public string Phone { get; set; } = "";
    
    [Display(Name = "Instructions de livraison")]
    public string? DeliveryInstructions { get; set; }
}

/// <summary>
/// ViewModel for payment information (simulated)
/// </summary>
public class PaymentViewModel
{
    public int OrderId { get; set; }
    public double TotalAmount { get; set; }
    
    [Required(ErrorMessage = "Le nom du titulaire est obligatoire")]
    [Display(Name = "Nom du titulaire de la carte")]
    public string CardHolderName { get; set; } = "";
    
    [Required(ErrorMessage = "Le numéro de carte est obligatoire")]
    [Display(Name = "Numéro de carte")]
    [CreditCard(ErrorMessage = "Format de carte invalide")]
    public string CardNumber { get; set; } = "";
    
    [Required(ErrorMessage = "La date d'expiration est obligatoire")]
    [Display(Name = "Date d'expiration (MM/AA)")]
    public string ExpiryDate { get; set; } = "";
    
    [Required(ErrorMessage = "Le CVV est obligatoire")]
    [Display(Name = "CVV")]
    [StringLength(4, MinimumLength = 3, ErrorMessage = "Le CVV doit contenir 3 ou 4 chiffres")]
    public string CVV { get; set; } = "";
}
