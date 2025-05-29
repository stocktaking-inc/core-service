using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using CoreService.Models;

namespace CoreService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Пользовательский конвертер для Item.StatusType
            var statusTypeConverter = new ValueConverter<Item.StatusType, string>(
                v => v == Item.StatusType.InStock ? "In Stock" :
                     v == Item.StatusType.OutOfStock ? "Out of Stock" :
                     v == Item.StatusType.LowStock ? "Low Stock" : "Out of Stock",
                v => v == "In Stock" ? Item.StatusType.InStock :
                     v == "Out of Stock" ? Item.StatusType.OutOfStock :
                     v == "Low Stock" ? Item.StatusType.LowStock : Item.StatusType.OutOfStock);

            // Пользовательский конвертер для Supplier.EntityStatus
            var entityStatusConverter = new ValueConverter<Supplier.EntityStatus, string>(
                v => v.ToString(),
                v => v == "active" ? Supplier.EntityStatus.Active :
                     v == "inactive" ? Supplier.EntityStatus.Inactive : Supplier.EntityStatus.Active);

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
                    .HasConversion(statusTypeConverter)
                    .HasDefaultValue(Item.StatusType.OutOfStock);

                // Relationships
                entity.HasOne(i => i.Location)
                    .WithMany(w => w.Items)
                    .HasForeignKey(i => i.LocationId)
                    .IsRequired(false);

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
                    .HasConversion(entityStatusConverter)
                    .HasDefaultValue(Supplier.EntityStatus.Active);
            });
        }
    }
}