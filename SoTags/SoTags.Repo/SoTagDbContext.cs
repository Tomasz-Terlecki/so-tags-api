using Microsoft.EntityFrameworkCore;
using SoTags.Domain.Models;

namespace SoTags.Repo;

public class SoTagDbContext : DbContext
{
    public SoTagDbContext(DbContextOptions<SoTagDbContext> options) : base(options)
    {
    }

    public DbSet<SoTag> SoTags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure SoTag entity
        modelBuilder.Entity<SoTag>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Count)
                .IsRequired();

            entity.Property(e => e.Share)
                .IsRequired()
                .HasPrecision(10, 4);

            entity.Property(e => e.HasSynonyms)
                .IsRequired();

            entity.Property(e => e.IsModeratorOnly)
                .IsRequired();

            entity.Property(e => e.IsRequired)
                .IsRequired();
        });
    }
}

