using Microsoft.EntityFrameworkCore;
using Test.Core.Models;

namespace Test.DataAccess;

public class TestApiDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public TestApiDbContext(DbContextOptions options) : base(options)
    {
        
    }
}