using CoreService.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<Item> Items { get; set; }
    public DbSet<Warehouse> Warehouse { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(i => i.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(i => i.Article).HasColumnName("article").HasMaxLength(50).IsRequired();
            entity.Property(i => i.Category).HasColumnName("category").HasMaxLength(50).IsRequired();
            entity.Property(i => i.Quantity).HasColumnName("quantity").IsRequired().HasDefaultValue(0);
            entity.Property(i => i.LocationId).HasColumnName("location");
            entity.Property(i => i.SupplierId).HasColumnName("supplier").IsRequired();
            entity.Property(i => i.Status)
                  .HasColumnName("status")
                  .HasConversion(
                      v => v.ToString(),
                      v => v == "Out of Stock" ? Item.StatusType.OutOfStock :
                           v == "In Stock" ? Item.StatusType.InStock :
                           v == "Low Stock" ? Item.StatusType.LowStock : Item.StatusType.OutOfStock)
                  .HasDefaultValue(Item.StatusType.OutOfStock);
            entity.HasOne(i => i.Location)
                  .WithMany()
                  .HasForeignKey(i => i.LocationId)
                  .HasConstraintName("items_location_fkey")
                  .HasPrincipalKey(w => w.Id)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(i => i.Supplier)
                  .WithMany()
                  .HasForeignKey(i => i.SupplierId)
                  .HasConstraintName("items_supplier_fkey")
                  .HasPrincipalKey(s => s.Id)
                  .IsRequired(true)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(i => i.Article).IsUnique();
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("warehouse");
            entity.HasKey(w => w.Id);
            entity.Property(w => w.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(w => w.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(w => w.Address).HasColumnName("address");
            entity.Property(w => w.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnName("supplier_id").ValueGeneratedOnAdd();
            entity.Property(s => s.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(s => s.ContactPerson).HasColumnName("contact_person").HasMaxLength(100);
            entity.Property(s => s.Email).HasColumnName("email").HasMaxLength(100);
            entity.Property(s => s.Phone).HasColumnName("phone").HasMaxLength(20);
            entity.Property(s => s.Status)
                  .HasColumnName("status")
                  .HasConversion(
                      v => v.ToString().ToLower(),
                      v => v == "active" ? Supplier.EntityStatus.Active :
                           v == "inactive" ? Supplier.EntityStatus.Inactive : Supplier.EntityStatus.Active)
                  .HasDefaultValue(Supplier.EntityStatus.Active);
        });
    }
}
