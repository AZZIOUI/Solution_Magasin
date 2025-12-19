using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("DetailAchat")]
public partial class DetailAchat
{
    [Key]
    [Column("id_da")]
    public int IdDa { get; set; }

    [Column("qte_da")]
    public int? QteDa { get; set; }

    [Column("montant_da")]
    public double? MontantDa { get; set; }

    [Column("id_achat")]
    public int IdAchat { get; set; }

    [Column("id_article")]
    public int IdArticle { get; set; }

    [ForeignKey("IdAchat")]
    [InverseProperty("DetailAchats")]
    public virtual Achat IdAchatNavigation { get; set; } = null!;

    [ForeignKey("IdArticle")]
    [InverseProperty("DetailAchats")]
    public virtual Article IdArticleNavigation { get; set; } = null!;
}
