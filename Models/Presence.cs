using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Presence")]
public partial class Presence
{
    [Key]
    [Column("id_pr")]
    public int IdPr { get; set; }

    [Column("id_utilisateur")]
    public int IdUtilisateur { get; set; }

    [Column("dateP")]
    public DateOnly? DateP { get; set; }

    [Column("heure_arrive")]
    public TimeOnly? HeureArrive { get; set; }

    [Column("heure_depart")]
    public TimeOnly? HeureDepart { get; set; }

    [ForeignKey("IdUtilisateur")]
    [InverseProperty("Presences")]
    public virtual Employe IdUtilisateurNavigation { get; set; } = null!;
}
