using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for submitting a product review
/// </summary>
public class AddReviewViewModel
{
    [Required(ErrorMessage = "L'ID de l'article est requis")]
    public int IdArticle { get; set; }
    
    [Required(ErrorMessage = "La note est obligatoire")]
    [Range(1, 5, ErrorMessage = "La note doit être entre 1 et 5")]
    public int Rating { get; set; }
    
    [Required(ErrorMessage = "Le commentaire est obligatoire")]
    [StringLength(300, MinimumLength = 10, ErrorMessage = "Le commentaire doit contenir entre 10 et 300 caractères")]
    public string Comment { get; set; } = string.Empty;
    
    // For display purposes
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
}

/// <summary>
/// ViewModel for displaying client's reviews
/// </summary>
public class ClientReviewViewModel
{
    public int IdReview { get; set; }
    public int IdArticle { get; set; }
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
    public string? ProductReference { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime DateReview { get; set; }
}

/// <summary>
/// ViewModel for review history
/// </summary>
public class ReviewHistoryViewModel
{
    public List<ClientReviewViewModel> Reviews { get; set; } = new();
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
}
