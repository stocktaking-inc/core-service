using Microsoft.EntityFrameworkCore;
using CoreService.Models;

namespace CoreService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    // New inventory tables
    public DbSet<Item> Items { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure enums for PostgreSQL
        modelBuilder.HasPostgresEnum<Item.StatusType>();
        modelBuilder.HasPostgresEnum<Supplier.EntityStatus>();

        // Configure Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Article).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasDefaultValue(Item.StatusType.OutOfStock);
            
            // Relationships
            entity.HasOne(i => i.Location)
                .WithMany(w => w.Items)
                .HasForeignKey(i => i.LocationId)
                .IsRequired(false);  // Made optional to match your SQL data
            
            entity.HasOne(i => i.Supplier)
                .WithMany(s => s.Items)
                .HasForeignKey(i => i.SupplierId);
        });

        // Configure Warehouse
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("warehouse");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Configure Supplier
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasDefaultValue(Supplier.EntityStatus.Active);
        });
    }
}