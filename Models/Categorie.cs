using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Categorie")]
public partial class Categorie
{
    [Key]
    [Column("id_cat")]
    public int IdCat { get; set; }

    [Column("nom_cat")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NomCat { get; set; }

    [Column("description_cat")]
    [StringLength(200)]
    [Unicode(false)]
    public string? DescriptionCat { get; set; }

    [InverseProperty("IdCatNavigation")]
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
