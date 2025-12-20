using System.ComponentModel.DataAnnotations;

namespace Solution_Magasin.ViewModels;

/// <summary>
/// ViewModel pour enregistrer la présence
/// </summary>
public class PresenceViewModel
{
    public int IdPr { get; set; }
    
    [Display(Name = "Employé")]
    public string? NomEmploye { get; set; }
    
    [Display(Name = "CIN")]
    public string? Cin { get; set; }
    
    [Display(Name = "Date")]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
    [Display(Name = "Heure d'Arrivée")]
    public TimeOnly? HeureArrive { get; set; }
    
    [Display(Name = "Heure de Départ")]
    public TimeOnly? HeureDepart { get; set; }
    
    [Display(Name = "Durée")]
    public string? Duree
    {
        get
        {
            if (HeureArrive.HasValue && HeureDepart.HasValue)
            {
                var duree = HeureDepart.Value.ToTimeSpan() - HeureArrive.Value.ToTimeSpan();
                return $"{(int)duree.TotalHours}h {duree.Minutes}min";
            }
            return "En cours";
        }
    }
    
    public bool IsPresent => HeureArrive.HasValue && !HeureDepart.HasValue;
    public bool IsCompleted => HeureArrive.HasValue && HeureDepart.HasValue;
}

/// <summary>
/// ViewModel pour enregistrer l'arrivée
/// </summary>
public class CheckInViewModel
{
    [Required]
    public int IdUtilisateur { get; set; }
    
    [Display(Name = "Heure d'Arrivée")]
    public TimeOnly HeureArrive { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
}

/// <summary>
/// ViewModel pour enregistrer le départ
/// </summary>
public class CheckOutViewModel
{
    [Required]
    public int IdPr { get; set; }
    
    [Display(Name = "Heure de Départ")]
    public TimeOnly HeureDepart { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
}

/// <summary>
/// ViewModel pour le tableau de bord employé
/// </summary>
public class EmployeeDashboardViewModel
{
    [Display(Name = "Nom de l'Employé")]
    public string? NomEmploye { get; set; }
    
    [Display(Name = "CIN")]
    public string? Cin { get; set; }
    
    [Display(Name = "Date d'Embauche")]
    public DateOnly? DateEmbauche { get; set; }
    
    [Display(Name = "Rôle")]
    public string? Role { get; set; }
    
    // Statistiques de présence
    [Display(Name = "Présences ce mois")]
    public int PresencesCeMois { get; set; }
    
    [Display(Name = "Heures totales ce mois")]
    public double HeuresTotalesCeMois { get; set; }
    
    // Statistiques de stock (pour Magasinier)
    [Display(Name = "Alertes de Stock")]
    public int AlertesStock { get; set; }
    
    [Display(Name = "Articles en Rupture")]
    public int ArticlesEnRupture { get; set; }
    
    // Statistiques d'achats (pour Responsable d'Achat)
    [Display(Name = "Achats ce mois")]
    public int AchatsCeMois { get; set; }
    
    [Display(Name = "Total Achats ce mois")]
    [DisplayFormat(DataFormatString = "{0:N2} MAD")]
    public double TotalAchatsCeMois { get; set; }
    
    // Présence aujourd'hui
    public PresenceViewModel? PresenceAujourdhui { get; set; }
    
    // Notifications non lues
    [Display(Name = "Notifications Non Lues")]
    public int NotificationsNonLues { get; set; }
    
    public List<StockNotificationViewModel> DernieresNotifications { get; set; } = new();
}
