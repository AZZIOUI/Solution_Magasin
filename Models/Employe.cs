using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Employe")]
public partial class Employe
{
    [Key]
    [Column("id_utilisateur")]
    public int IdUtilisateur { get; set; }

    [Column("CIN")]
    [StringLength(30)]
    [Unicode(false)]
    public string? Cin { get; set; }

    [Column("prenom_emp")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PrenomEmp { get; set; }

    [Column("nom_emp")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NomEmp { get; set; }

    [Column("dateEmbauche")]
    public DateOnly? DateEmbauche { get; set; }

    [InverseProperty("IdUtilisateurNavigation")]
    public virtual ICollection<Presence> Presences { get; set; } = new List<Presence>();
}
