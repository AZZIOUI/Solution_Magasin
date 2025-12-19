using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Facture")]
public partial class Facture
{
    [Key]
    [Column("id_facture")]
    public int IdFacture { get; set; }

    [Column("code_facture")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CodeFacture { get; set; }

    [Column("date_facture")]
    public DateOnly? DateFacture { get; set; }

    [Column("montant_total")]
    public double? MontantTotal { get; set; }

    [Column("file_path")]
    [StringLength(300)]
    [Unicode(false)]
    public string? FilePath { get; set; }

    [Column("id_vente")]
    public int IdVente { get; set; }

    [ForeignKey("IdVente")]
    [InverseProperty("Factures")]
    public virtual Vente IdVenteNavigation { get; set; } = null!;
}
