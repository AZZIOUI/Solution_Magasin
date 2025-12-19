using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Stock")]
public partial class Stock
{
    [Key]
    [Column("id_st")]
    public int IdSt { get; set; }

    [Column("stockmax")]
    public int? Stockmax { get; set; }

    [Column("stockmin")]
    public int? Stockmin { get; set; }

    [Column("qte_dispo")]
    public int? QteDispo { get; set; }

    [Column("date_modification")]
    public DateOnly? DateModification { get; set; }

    [Column("id_article")]
    public int IdArticle { get; set; }

    [ForeignKey("IdArticle")]
    [InverseProperty("Stocks")]
    public virtual Article IdArticleNavigation { get; set; } = null!;
}
