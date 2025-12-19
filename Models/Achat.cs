using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Achat")]
public partial class Achat
{
    [Key]
    [Column("id_achat")]
    public int IdAchat { get; set; }

    [Column("date_achat", TypeName = "datetime")]
    public DateTime? DateAchat { get; set; }

    [Column("total_ach")]
    public double? TotalAch { get; set; }

    [Column("id_fourni")]
    public int IdFourni { get; set; }

    [InverseProperty("IdAchatNavigation")]
    public virtual ICollection<DetailAchat> DetailAchats { get; set; } = new List<DetailAchat>();

    [ForeignKey("IdFourni")]
    [InverseProperty("Achats")]
    public virtual Fournisseur IdFourniNavigation { get; set; } = null!;
}
