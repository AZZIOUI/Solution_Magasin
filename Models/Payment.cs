using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Payment")]
public partial class Payment
{
    [Key]
    [Column("id_payment")]
    public int IdPayment { get; set; }

    [Column("methode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Methode { get; set; }

    [Column("date_payment", TypeName = "datetime")]
    public DateTime? DatePayment { get; set; }

    [Column("estPaye")]
    public bool? EstPaye { get; set; }

    [InverseProperty("IdPaymentNavigation")]
    public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();
}
