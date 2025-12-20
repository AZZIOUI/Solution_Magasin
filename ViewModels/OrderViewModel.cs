using Solution_Magasin.Models;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for order history
/// </summary>
public class OrderHistoryViewModel
{
    public List<OrderSummaryViewModel> Orders { get; set; } = new();
}

/// <summary>
/// ViewModel for order summary
/// </summary>
public class OrderSummaryViewModel
{
    public int IdVente { get; set; }
    public DateTime? DateVente { get; set; }
    public double? TotalAmount { get; set; }
    public string? PaymentStatus { get; set; }
    public int ItemCount { get; set; }
    public List<OrderItemViewModel> Items { get; set; } = new();
}

/// <summary>
/// ViewModel for order item
/// </summary>
public class OrderItemViewModel
{
    public int IdArticle { get; set; }
    public string? ProductName { get; set; }
    public string? ProductReference { get; set; }
    public string? ImagePath { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double TotalPrice => Quantity * UnitPrice;
}

/// <summary>
/// ViewModel for detailed order view
/// </summary>
public class OrderDetailsViewModel
{
    public int IdVente { get; set; }
    public DateTime? DateVente { get; set; }
    public double? TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Phone { get; set; }
    public List<OrderItemViewModel> Items { get; set; } = new();
}
