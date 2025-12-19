using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("DetailVente")]
public partial class DetailVente
{
    [Key]
    [Column("id_dv")]
    public int IdDv { get; set; }

    [Column("qte_dv")]
    public int? QteDv { get; set; }

    [Column("montant_dv")]
    public double? MontantDv { get; set; }

    [Column("id_vente")]
    public int IdVente { get; set; }

    [Column("id_article")]
    public int IdArticle { get; set; }

    [ForeignKey("IdArticle")]
    [InverseProperty("DetailVentes")]
    public virtual Article IdArticleNavigation { get; set; } = null!;

    [ForeignKey("IdVente")]
    [InverseProperty("DetailVentes")]
    public virtual Vente IdVenteNavigation { get; set; } = null!;
}
