using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

/// <summary>
/// Contexte de base de données pour l'authentification avec Identity
/// UNIQUEMENT pour les tables Identity - ne gère PAS les tables business
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Ignorer TOUTES les entités business existantes pour éviter que EF les gère
        builder.Ignore<Achat>();
        builder.Ignore<Article>();
        builder.Ignore<Categorie>();
        builder.Ignore<Client>();
        builder.Ignore<DetailAchat>();
        builder.Ignore<DetailVente>();
        builder.Ignore<Employe>();
        builder.Ignore<Facture>();
        builder.Ignore<Fournisseur>();
        builder.Ignore<NotificationStock>();
        builder.Ignore<Payment>();
        builder.Ignore<Presence>();
        builder.Ignore<Retour>();
        builder.Ignore<Review>();
        builder.Ignore<Stock>();
        builder.Ignore<Vente>();

        // Configuration de ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            
            // Ignorer les propriétés de navigation
            entity.Ignore(u => u.Client);
            entity.Ignore(u => u.Employe);
        });

        // Tables Identity
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("AspNetRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("AspNetUserRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("AspNetUserTokens");
    }
}
