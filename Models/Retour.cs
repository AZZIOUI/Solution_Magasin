using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Retour")]
public partial class Retour
{
    [Key]
    [Column("id_retour")]
    public int IdRetour { get; set; }

    [Column("motif")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Motif { get; set; }

    [Column("date_retour", TypeName = "datetime")]
    public DateTime? DateRetour { get; set; }

    [Column("id_article")]
    public int IdArticle { get; set; }

    [Column("id_vente")]
    public int IdVente { get; set; }

    [ForeignKey("IdArticle")]
    [InverseProperty("Retours")]
    public virtual Article IdArticleNavigation { get; set; } = null!;

    [ForeignKey("IdVente")]
    [InverseProperty("Retours")]
    public virtual Vente IdVenteNavigation { get; set; } = null!;
}
