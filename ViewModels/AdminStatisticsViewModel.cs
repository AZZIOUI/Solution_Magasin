using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour les statistiques du tableau de bord administrateur
/// </summary>
public class AdminStatisticsViewModel
{
    [Display(Name = "Utilisateurs totaux")]
    public int TotalUsers { get; set; }

    [Display(Name = "Clients")]
    public int TotalClients { get; set; }

    [Display(Name = "Employés")]
    public int TotalEmployees { get; set; }

    [Display(Name = "Produits")]
    public int TotalProducts { get; set; }

    [Display(Name = "Catégories")]
    public int TotalCategories { get; set; }

    [Display(Name = "Fournisseurs")]
    public int TotalSuppliers { get; set; }

    [Display(Name = "Ventes aujourd'hui")]
    public int TodaySales { get; set; }

    [Display(Name = "Ventes ce mois")]
    public int MonthSales { get; set; }

    [Display(Name = "Revenus aujourd'hui")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public double TodayRevenue { get; set; }

    [Display(Name = "Revenus ce mois")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public double MonthRevenue { get; set; }

    [Display(Name = "Articles en rupture de stock")]
    public int OutOfStockProducts { get; set; }

    [Display(Name = "Articles en stock faible")]
    public int LowStockProducts { get; set; }

    [Display(Name = "Dernières ventes")]
    public List<SalesViewModel> RecentSales { get; set; } = new();

    [Display(Name = "Produits populaires")]
    public List<ProductViewModel> PopularProducts { get; set; } = new();

    [Display(Name = "Alertes stock")]
    public List<StockViewModel> StockAlerts { get; set; } = new();
}
