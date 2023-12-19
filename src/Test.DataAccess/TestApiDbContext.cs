using Microsoft.EntityFrameworkCore;
using Test.Core.Models;

namespace Test.DataAccess;

public class TestApiDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Customer> Customers { get; set; }
    
    public DbSet<Order> Orders { get; set; }

    public TestApiDbContext(DbContextOptions options) : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Product) // Each Order has one Product
            .WithMany(p => p.Orders) // Product can have many Orders
            .HasForeignKey(o => o.ProductId); // Foreign key is ProductId
        
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer) // Each Order has one Customer
            .WithMany(c => c.Orders) // Customer can have many Orders
            .HasForeignKey(o => o.CustomerId); // Foreign key is CustomerId
    }
}