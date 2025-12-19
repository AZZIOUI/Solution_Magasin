using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solution_Magasin.Models;

/// <summary>
/// Utilisateur de l'application avec support pour Identity
/// Peut être lié à un Client ou un Employé
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Type d'utilisateur: "Client" ou "Employe"
    /// </summary>
    public string? UserType { get; set; }

    /// <summary>
    /// ID du client si l'utilisateur est un client
    /// </summary>
    public int? ClientId { get; set; }

    /// <summary>
    /// ID de l'employé si l'utilisateur est un employé
    /// </summary>
    public int? EmployeId { get; set; }

    /// <summary>
    /// Prénom de l'utilisateur
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Nom de l'utilisateur
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Date de création du compte
    /// </summary>
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indique si le compte est actif
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation vers le client associé
    /// </summary>
    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    /// <summary>
    /// Navigation vers l'employé associé
    /// </summary>
    [ForeignKey("EmployeId")]
    public virtual Employe? Employe { get; set; }
}
