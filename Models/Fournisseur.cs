using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Fournisseur")]
public partial class Fournisseur
{
    [Key]
    [Column("id_fourni")]
    public int IdFourni { get; set; }

    [Column("CIN")]
    [StringLength(30)]
    [Unicode(false)]
    public string? Cin { get; set; }

    [Column("nom_fourni")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NomFourni { get; set; }

    [Column("prenom_fourni")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PrenomFourni { get; set; }

    [Column("adresse_fourni")]
    [StringLength(200)]
    [Unicode(false)]
    public string? AdresseFourni { get; set; }

    [Column("tel_fourni")]
    [StringLength(50)]
    [Unicode(false)]
    public string? TelFourni { get; set; }

    [Column("mail_fourni")]
    [StringLength(200)]
    [Unicode(false)]
    public string? MailFourni { get; set; }

    [InverseProperty("IdFourniNavigation")]
    public virtual ICollection<Achat> Achats { get; set; } = new List<Achat>();
}
