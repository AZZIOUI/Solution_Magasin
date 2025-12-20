namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel for shopping cart
/// </summary>
public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public double Subtotal => Items.Sum(i => i.TotalPrice);
    public double ShippingCost { get; set; } = 50.0; // Fixed shipping cost
    public double Total => Subtotal + ShippingCost;
    public int TotalItems => Items.Sum(i => i.Quantity);
}

/// <summary>
/// ViewModel for shopping cart item
/// </summary>
public class CartItemViewModel
{
    public int IdArticle { get; set; }
    public string? ProductName { get; set; }
    public string? ProductReference { get; set; }
    public string? ImagePath { get; set; }
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice => UnitPrice * Quantity;
    public int MaxQuantity { get; set; } // Based on stock
}
