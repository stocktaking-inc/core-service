using CoreService.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<Item> Items { get; set; }
    public DbSet<Warehouse> Warehouse { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Good> Goods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Good>(entity =>
      {
        entity.ToTable("good");
        entity.HasKey(g => g.Id);
        entity.Property(g => g.Id).HasColumnName("id").ValueGeneratedOnAdd();
        entity.Property(g => g.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        entity.Property(g => g.Article).HasColumnName("article").HasMaxLength(20).IsRequired();
        entity.Property(g => g.PurchasePrice).HasColumnName("purchase_price").HasColumnType("decimal(10, 2)");
        entity.Property(g => g.Category).HasColumnName("category").HasMaxLength(50);
        entity.Property(g => g.SupplierId).HasColumnName("supplier_id").IsRequired();

        // Явно указываем связь с Supplier
        entity.HasOne(g => g.Supplier)
          .WithMany(s => s.Goods)
          .HasForeignKey(g => g.SupplierId)
          .OnDelete(DeleteBehavior.Restrict);
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
            v => v.ToString(),
            v => v == "active" ? Supplier.EntityStatus.Active :
              v == "inactive" ? Supplier.EntityStatus.Inactive : Supplier.EntityStatus.Active)
          .HasDefaultValue(Supplier.EntityStatus.Active);
      });
    }
}
