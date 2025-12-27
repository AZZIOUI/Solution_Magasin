namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for employee order management (Magasinier)
/// </summary>
public class EmployeeOrderViewModel
{
    public int IdVente { get; set; }
    public DateTime? DateVente { get; set; }
    
    // Client information
    public string? ClientName { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientPhone { get; set; }
    public string? ClientAddress { get; set; }
    
    // Delivery information
    public string? AdresseLiv { get; set; }
    
    // Order information
    public double? TotalV { get; set; }
    public string? Status { get; set; }
    public int ItemCount { get; set; }
    
    // Payment information
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    
    // Order items (for details view)
    public List<EmployeeOrderItemViewModel> Items { get; set; } = new();
}

/// <summary>
/// ViewModel for order item in employee view
/// </summary>
public class EmployeeOrderItemViewModel
{
    public int IdArticle { get; set; }
    public string? ProductName { get; set; }
    public string? ProductReference { get; set; }
    public string? ImagePath { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Subtotal { get; set; }
    public int StockAvailable { get; set; }
}
