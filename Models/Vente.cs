using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Vente")]
public partial class Vente
{
    [Key]
    [Column("id_vente")]
    public int IdVente { get; set; }

    [Column("date_vente", TypeName = "datetime")]
    public DateTime? DateVente { get; set; }

    [Column("total_v")]
    public double? TotalV { get; set; }

    [Column("adresse_liv")]
    [StringLength(200)]
    [Unicode(false)]
    public string? AdresseLiv { get; set; }

    [Column("status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column("id_client")]
    public int IdClient { get; set; }

    [Column("id_payment")]
    public int? IdPayment { get; set; }

    [InverseProperty("IdVenteNavigation")]
    public virtual ICollection<DetailVente> DetailVentes { get; set; } = new List<DetailVente>();

    [InverseProperty("IdVenteNavigation")]
    public virtual ICollection<Facture> Factures { get; set; } = new List<Facture>();

    [ForeignKey("IdClient")]
    [InverseProperty("Ventes")]
    public virtual Client IdClientNavigation { get; set; } = null!;

    [ForeignKey("IdPayment")]
    [InverseProperty("Ventes")]
    public virtual Payment? IdPaymentNavigation { get; set; }

    [InverseProperty("IdVenteNavigation")]
    public virtual ICollection<Retour> Retours { get; set; } = new List<Retour>();
}
