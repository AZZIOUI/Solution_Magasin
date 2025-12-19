using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

[Table("Client")]
public partial class Client
{
    [Key]
    [Column("id_client")]
    public int IdClient { get; set; }

    [Column("nom_client")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NomClient { get; set; }

    [Column("prenom_client")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PrenomClient { get; set; }

    [Column("adresse_client")]
    [StringLength(200)]
    [Unicode(false)]
    public string? AdresseClient { get; set; }

    [Column("mail_client")]
    [StringLength(200)]
    [Unicode(false)]
    public string? MailClient { get; set; }

    [Column("tel_client")]
    [StringLength(50)]
    [Unicode(false)]
    public string? TelClient { get; set; }

    [Column("aspnet_user_id")]
    public int? AspnetUserId { get; set; }

    [InverseProperty("IdClientNavigation")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("IdClientNavigation")]
    public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();
}
