using Solution_Magasin.Models;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for product catalog display
/// </summary>
public class ProductCatalogViewModel
{
    public List<ProductCardViewModel> Products { get; set; } = new();
    public List<Categorie> Categories { get; set; } = new();
    public int? SelectedCategoryId { get; set; }
    public string? SearchQuery { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 12;
}

/// <summary>
/// ViewModel for individual product card in catalog
/// </summary>
public class ProductCardViewModel
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
    public double? AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
