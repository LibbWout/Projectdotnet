using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace project.project;

public partial class ProjectContext : DbContext
{
    public ProjectContext()
    {
    }

    public ProjectContext(DbContextOptions<ProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bestellijnen> Bestellijnens { get; set; }

    public virtual DbSet<Bestellingen> Bestellingens { get; set; }

    public virtual DbSet<Gebruiker> Gebruikers { get; set; }

    public virtual DbSet<Productdetail> Productdetails { get; set; }

    public virtual DbSet<Producten> Productens { get; set; }

    public virtual DbSet<Rollen> Rollens { get; set; }

    public virtual DbSet<Roltoewijzingen> Roltoewijzingens { get; set; }

    public virtual DbSet<Tafel> Tafels { get; set; }

    public virtual DbSet<Tafeltoewijzingen> Tafeltoewijzingens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3307;uid=root;pwd=root;database=project");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bestellijnen>(entity =>
        {
            entity.HasKey(e => new { e.BestellingId, e.ProductId }).HasName("PRIMARY");

            entity.ToTable("bestellijnen");

            entity.HasIndex(e => e.ProductId, "fk__bestellijnen__product_id");

            entity.Property(e => e.BestellingId)
                .HasColumnType("int(11)")
                .HasColumnName("bestelling_id");
            entity.Property(e => e.ProductId)
                .HasColumnType("int(11)")
                .HasColumnName("product_id");
            entity.Property(e => e.Hoeveelheid)
                .HasColumnType("int(11)")
                .HasColumnName("hoeveelheid");
        });

        modelBuilder.Entity<Bestellingen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bestellingen");

            entity.HasIndex(e => e.GebruikerId, "fk__bestellingen__gebruiker_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.GebruikerId)
                .HasColumnType("int(11)")
                .HasColumnName("gebruiker_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TijdstipBesteld)
                .HasColumnType("datetime")
                .HasColumnName("tijdstip_besteld");
        });

        modelBuilder.Entity<Gebruiker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("gebruikers");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Naam)
                .HasMaxLength(255)
                .HasColumnName("naam");
            entity.Property(e => e.TijdstipGeactiveerd)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime")
                .HasColumnName("tijdstip_geactiveerd");
            entity.Property(e => e.UniekeCode)
                .HasMaxLength(255)
                .HasColumnName("unieke_code");
            entity.Property(e => e.WachtwoordHash)
                .HasMaxLength(255)
                .IsFixedLength()
                .HasColumnName("wachtwoord_hash");
        });

        modelBuilder.Entity<Productdetail>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.Tijdstip }).HasName("PRIMARY");

            entity.ToTable("productdetails");

            entity.HasIndex(e => e.ProductId, "fk__productdetails__product_id");

            entity.Property(e => e.ProductId)
                .HasColumnType("int(11)")
                .HasColumnName("product_id");
            entity.Property(e => e.Tijdstip)
                .HasColumnType("datetime")
                .HasColumnName("tijdstip");
            entity.Property(e => e.Naam)
                .HasMaxLength(255)
                .HasColumnName("naam");
            entity.Property(e => e.Prijs).HasColumnName("prijs");
            entity.Property(e => e.Producttype)
                .HasMaxLength(50)
                .HasColumnName("producttype");
        });

        modelBuilder.Entity<Producten>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("producten");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
        });

        modelBuilder.Entity<Rollen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("rollen");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Naam)
                .HasMaxLength(255)
                .HasColumnName("naam");
        });

        modelBuilder.Entity<Roltoewijzingen>(entity =>
        {
            entity.HasKey(e => new { e.GebruikerId, e.RolId }).HasName("PRIMARY");

            entity.ToTable("roltoewijzingen");

            entity.HasIndex(e => e.RolId, "fk__roltoewijzingen__rol_id");

            entity.Property(e => e.GebruikerId)
                .HasColumnType("int(11)")
                .HasColumnName("gebruiker_id");
            entity.Property(e => e.RolId)
                .HasColumnType("int(11)")
                .HasColumnName("rol_id");
        });

        modelBuilder.Entity<Tafel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tafels");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Naam)
                .HasColumnType("int(11)")
                .HasColumnName("naam");
        });

        modelBuilder.Entity<Tafeltoewijzingen>(entity =>
        {
            entity.HasKey(e => new { e.GebruikerId, e.TafelId, e.TijdstipToegewezen }).HasName("PRIMARY");

            entity.ToTable("tafeltoewijzingen");

            entity.HasIndex(e => e.TafelId, "fk__tafeltoewijzingen__tafel_id");

            entity.Property(e => e.GebruikerId)
                .HasColumnType("int(11)")
                .HasColumnName("gebruiker_id");
            entity.Property(e => e.TafelId)
                .HasColumnType("int(11)")
                .HasColumnName("tafel_id");
            entity.Property(e => e.TijdstipToegewezen)
                .HasColumnType("datetime")
                .HasColumnName("tijdstip_toegewezen");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
