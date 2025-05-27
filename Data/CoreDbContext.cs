using Microsoft.EntityFrameworkCore;
using CoreService.Models;

namespace CoreService.Data
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<EmailRequest> EmailRequests { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TotalPurchases).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Status).HasConversion<string>();
                
                entity.HasMany(c => c.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId);
            });

            modelBuilder.Entity<EmailRequest>(entity =>
            {
                entity.ToTable("email_requests");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Subject).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Body).HasColumnType("text").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
                entity.Property(e => e.Status).HasConversion<string>();
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Article).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.Date).HasColumnType("timestamp").IsRequired();
                
                entity.HasMany(o => o.Items)
                      .WithOne(oi => oi.Order)
                      .HasForeignKey(oi => oi.OrderId);
                
                entity.HasOne(o => o.Customer)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(o => o.CustomerId);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)").IsRequired();
                
                entity.HasOne(oi => oi.Item)
                      .WithMany()
                      .HasForeignKey(oi => oi.ItemId);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoices");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.IssuedDate).HasColumnType("timestamp").IsRequired();
            });

            modelBuilder.Entity<Error>(entity =>
            {
                entity.ToTable("errors");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Message).HasColumnType("text").IsRequired();
            });
        }
    }
}