using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Article")]
public partial class Article
{
    [Key]
    [Column("id_article")]
    public int IdArticle { get; set; }

    [Column("reference_art")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ReferenceArt { get; set; }

    [Column("nom_art")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NomArt { get; set; }

    [Column("designation_art")]
    [StringLength(200)]
    [Unicode(false)]
    public string? DesignationArt { get; set; }

    [Column("prix_unit")]
    public double? PrixUnit { get; set; }

    [Column("date_ajout")]
    public DateOnly? DateAjout { get; set; }

    [Column("id_cat")]
    public int? IdCat { get; set; }

    [Column("image_path")]
    [StringLength(500)]
    [Unicode(false)]
    public string? ImagePath { get; set; }

    [InverseProperty("IdArticleNavigation")]
    public virtual ICollection<DetailAchat> DetailAchats { get; set; } = new List<DetailAchat>();

    [InverseProperty("IdArticleNavigation")]
    public virtual ICollection<DetailVente> DetailVentes { get; set; } = new List<DetailVente>();

    [ForeignKey("IdCat")]
    [InverseProperty("Articles")]
    public virtual Categorie? IdCatNavigation { get; set; }

    [InverseProperty("IdArticleNavigation")]
    public virtual ICollection<NotificationStock> NotificationStocks { get; set; } = new List<NotificationStock>();

    [InverseProperty("IdArticleNavigation")]
    public virtual ICollection<Retour> Retours { get; set; } = new List<Retour>();

    [InverseProperty("IdArticleNavigation")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("IdArticleNavigation")]
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
