using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("NotificationStock")]
public partial class NotificationStock
{
    [Key]
    [Column("id_not")]
    public int IdNot { get; set; }

    [Column("id_article")]
    public int IdArticle { get; set; }

    [Column("msg")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Msg { get; set; }

    [Column("date_not", TypeName = "datetime")]
    public DateTime? DateNot { get; set; }

    [Column("vu")]
    public bool? Vu { get; set; }

    [ForeignKey("IdArticle")]
    [InverseProperty("NotificationStocks")]
    public virtual Article IdArticleNavigation { get; set; } = null!;
}
