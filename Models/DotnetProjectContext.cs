using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Solution_Magasin.Models;

public partial class DotnetProjectContext : DbContext
{
    public DotnetProjectContext()
    {
    }

    public DotnetProjectContext(DbContextOptions<DotnetProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achat> Achats { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Categorie> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<DetailAchat> DetailAchats { get; set; }

    public virtual DbSet<DetailVente> DetailVentes { get; set; }

    public virtual DbSet<Employe> Employes { get; set; }

    public virtual DbSet<Facture> Factures { get; set; }

    public virtual DbSet<Fournisseur> Fournisseurs { get; set; }

    public virtual DbSet<NotificationStock> NotificationStocks { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Presence> Presences { get; set; }

    public virtual DbSet<Retour> Retours { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Vente> Ventes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ZED;Database=dotnet_project;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achat>(entity =>
        {
            entity.HasKey(e => e.IdAchat).HasName("PK__Achat__ED270182E8DFCDB2");

            entity.HasOne(d => d.IdFourniNavigation).WithMany(p => p.Achats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Achat__id_fourni__4D94879B");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.IdArticle).HasName("PK__Article__64CB31B8FDBA7DD5");

            entity.HasOne(d => d.IdCatNavigation).WithMany(p => p.Articles).HasConstraintName("FK__Article__id_cat__44FF419A");
        });

        modelBuilder.Entity<Categorie>(entity =>
        {
            entity.HasKey(e => e.IdCat).HasName("PK__Categori__D54686DEFF867280");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("PK__Client__6EC2B6C07F78BFA5");
        });

        modelBuilder.Entity<DetailAchat>(entity =>
        {
            entity.HasKey(e => e.IdDa).HasName("PK__DetailAc__00B7C6E67393A75E");

            entity.HasOne(d => d.IdAchatNavigation).WithMany(p => p.DetailAchats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetailAch__id_ac__5070F446");

            entity.HasOne(d => d.IdArticleNavigation).WithMany(p => p.DetailAchats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetailAch__id_ar__5165187F");
        });

        modelBuilder.Entity<DetailVente>(entity =>
        {
            entity.HasKey(e => e.IdDv).HasName("PK__DetailVe__00B7C6CD84831A7A");

            entity.HasOne(d => d.IdArticleNavigation).WithMany(p => p.DetailVentes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetailVen__id_ar__5AEE82B9");

            entity.HasOne(d => d.IdVenteNavigation).WithMany(p => p.DetailVentes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetailVen__id_ve__59FA5E80");
        });

        modelBuilder.Entity<Employe>(entity =>
        {
            entity.HasKey(e => e.IdUtilisateur).HasName("PK__Employe__1A4FA5B827B60C55");
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.IdFacture).HasName("PK__Facture__6C08ED575425E103");

            entity.HasOne(d => d.IdVenteNavigation).WithMany(p => p.Factures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Facture__id_vent__619B8048");
        });

        modelBuilder.Entity<Fournisseur>(entity =>
        {
            entity.HasKey(e => e.IdFourni).HasName("PK__Fourniss__035DEECF4FD13219");
        });

        modelBuilder.Entity<NotificationStock>(entity =>
        {
            entity.HasKey(e => e.IdNot).HasName("PK__Notifica__6E4C7671050D7F42");

            entity.HasOne(d => d.IdArticleNavigation).WithMany(p => p.NotificationStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__id_ar__4AB81AF0");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.IdPayment).HasName("PK__Payment__862FEFE014B98D38");
        });

        modelBuilder.Entity<Presence>(entity =>
        {
            entity.HasKey(e => e.IdPr).HasName("PK__Presence__0148A34E4E909523");

            entity.HasOne(d => d.IdUtilisateurNavigation).WithMany(p => p.Presences)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Presence__id_uti__403A8C7D");
        });

        modelBuilder.Entity<Retour>(entity =>
        {
            entity.HasKey(e => e.IdRetour).HasName("PK__Retour__4E89E94F61945E7A");

            entity.HasOne(d => d.IdArticleNavigation).WithMany(p => p.Retours)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Retour__id_artic__6477ECF3");

            entity.HasOne(d => d.IdVenteNavigation).WithMany(p => p.Retours)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Retour__id_vente__656C112C");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.IdReview).HasName("PK__Review__2F79F8C7912B0A56");

            entity.HasOne(d => d.IdArticleNavigation).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__id_artic__5EBF139D");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__id_clien__5DCAEF64");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.IdSt).HasName("PK__Stock__014858EB29631BEF");

            entity.HasOne(d => d.IdArticleNavigation).WithMany(p => p.Stocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stock__id_articl__47DBAE45");
        });

        modelBuilder.Entity<Vente>(entity =>
        {
            entity.HasKey(e => e.IdVente).HasName("PK__Vente__459533B319319C7A");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Ventes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vente__id_client__5629CD9C");

            entity.HasOne(d => d.IdPaymentNavigation).WithMany(p => p.Ventes).HasConstraintName("FK__Vente__id_paymen__571DF1D5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
