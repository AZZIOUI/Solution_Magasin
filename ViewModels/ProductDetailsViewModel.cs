using Solution_Magasin.Models;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for detailed product view
/// </summary>
public class ProductDetailsViewModel
{
    public int IdArticle { get; set; }
    public string? ReferenceArt { get; set; }
    public string? NomArt { get; set; }
    public string? DesignationArt { get; set; }
    public double? PrixUnit { get; set; }
    public string? ImagePath { get; set; }
    public string? CategoryName { get; set; }
    public int? StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
    
    // Reviews
    public List<ProductReviewViewModel> Reviews { get; set; } = new();
    public double? AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    // Related products
    public List<ProductCardViewModel> RelatedProducts { get; set; } = new();
}

/// <summary>
/// ViewModel for product review
/// </summary>
public class ProductReviewViewModel
{
    public int IdReview { get; set; }
    public string? ClientName { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? DateReview { get; set; }
}
