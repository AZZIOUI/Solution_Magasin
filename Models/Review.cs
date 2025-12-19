using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Review")]
public partial class Review
{
    [Key]
    [Column("id_review")]
    public int IdReview { get; set; }

    [Column("comment")]
    [StringLength(300)]
    [Unicode(false)]
    public string? Comment { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }

    [Column("id_client")]
    public int IdClient { get; set; }

    [Column("id_article")]
    public int IdArticle { get; set; }

    [ForeignKey("IdArticle")]
    [InverseProperty("Reviews")]
    public virtual Article IdArticleNavigation { get; set; } = null!;

    [ForeignKey("IdClient")]
    [InverseProperty("Reviews")]
    public virtual Client IdClientNavigation { get; set; } = null!;
}
