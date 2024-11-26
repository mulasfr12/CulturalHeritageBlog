using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

public partial class CulturalHeritageDbContext : DbContext
{
    public CulturalHeritageDbContext()
    {
    }

    public CulturalHeritageDbContext(DbContextOptions<CulturalHeritageDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CulturalHeritage> CulturalHeritages { get; set; }

    public virtual DbSet<CulturalHeritageTheme> CulturalHeritageThemes { get; set; }

    public virtual DbSet<HeritageImage> HeritageImages { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MULAS;Database=CulturalHeritageDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFAA9241DCAF");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HeritageId).HasColumnName("HeritageID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Heritage).WithMany(p => p.Comments)
                .HasForeignKey(d => d.HeritageId)
                .HasConstraintName("FK__Comment__Heritag__48CFD27E");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comment__UserID__47DBAE45");
        });

        modelBuilder.Entity<CulturalHeritage>(entity =>
        {
            entity.HasKey(e => e.HeritageId).HasName("PK__Cultural__23C145163135D5FD");

            entity.ToTable("CulturalHeritage");

            entity.Property(e => e.HeritageId).HasColumnName("HeritageID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NationalMinority).HasMaxLength(100);
        });

        modelBuilder.Entity<CulturalHeritageTheme>(entity =>
        {
            // Explicitly map the table name to "CulturalHeritageTheme"
            entity.ToTable("CulturalHeritageTheme");

            // Configure the composite primary key
            entity.HasKey(ct => new { ct.HeritageId, ct.ThemeId });

            // Configure relationships
            entity.HasOne(ct => ct.Heritage)
                .WithMany(ch => ch.CulturalHeritageThemes)
                .HasForeignKey(ct => ct.HeritageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CulturalHeritageTheme_CulturalHeritage");

            entity.HasOne(ct => ct.Theme)
                .WithMany(t => t.CulturalHeritageThemes)
                .HasForeignKey(ct => ct.ThemeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CulturalHeritageTheme_Theme");
        });
        modelBuilder.Entity<HeritageImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Heritage__7516F4ECCB54019B");

            entity.ToTable("HeritageImage");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.HeritageId).HasColumnName("HeritageID");
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Heritage).WithMany(p => p.HeritageImages)
                .HasForeignKey(d => d.HeritageId)
                .HasConstraintName("FK__HeritageI__Herit__5070F446");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Log__5E5499A86AB5BFB8");

            entity.ToTable("Log");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Log__UserID__4CA06362");
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.ThemeID).HasName("PK__Theme__FBB3E4B9F4118FF9");

            entity.ToTable("Theme");

            entity.HasIndex(e => e.Name, "UQ__Theme__737584F65DE0B1FE").IsUnique();

            entity.Property(e => e.ThemeID).HasColumnName("ThemeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC0998FCAC");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E413543FB4").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534ABD4C553").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
