namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for admin return management
/// </summary>
public class AdminReturnViewModel
{
    public int IdRetour { get; set; }
    public string? Motif { get; set; }
    public DateTime? DateRetour { get; set; }
    public int IdArticle { get; set; }
    public int IdVente { get; set; }
    
    // Product information
    public string? ProductName { get; set; }
    public string? ProductReference { get; set; }
    public string? ProductImage { get; set; }
    public double? ProductPrice { get; set; }
    
    // Order information
    public string? OrderNumber { get; set; }
    public DateTime? OrderDate { get; set; }
    
    // Client information
    public string? ClientName { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientPhone { get; set; }
    
    // Status
    public string? Status { get; set; }
}

/// <summary>
/// ViewModel for admin review management
/// </summary>
public class AdminReviewViewModel
{
    public int IdReview { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    
    // Product information
    public string? ProductName { get; set; }
    public string? ProductReference { get; set; }
    public string? ProductImage { get; set; }
    
    // Client information
    public string? ClientName { get; set; }
    public string? ClientEmail { get; set; }
}

/// <summary>
/// ViewModel for admin invoice management
/// </summary>
public class AdminInvoiceViewModel
{
    public int IdFacture { get; set; }
    public string? CodeFacture { get; set; }
    public DateOnly? DateFacture { get; set; }
    public double? MontantTotal { get; set; }
    public string? FilePath { get; set; }
    public int IdVente { get; set; }
    
    // Order information
    public DateTime? OrderDate { get; set; }
    
    // Client information
    public string? ClientName { get; set; }
    public string? ClientEmail { get; set; }
}
