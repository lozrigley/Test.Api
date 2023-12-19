using Microsoft.EntityFrameworkCore;
using OneOf;
using Test.Core;
using Test.Core.Models;

namespace Test.DataAccess;

public class ProductRepository : IProductRepository
{
    
    private readonly TestApiDbContext _context;
    public ProductRepository(TestApiDbContext context)
    {
         _context = context;   
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<OneOf<Product, NotFound>> GetByIdAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is not null)
        {
            return product;
        }

        return new NotFound();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<OneOf<Product, NotFound>> UpdateAsync(Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id);
        if (existingProduct is null)
        {
            return new NotFound();
        }

        _context.Entry(existingProduct).CurrentValues.SetValues(product);
        await _context.SaveChangesAsync();
        return product;

    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            return new NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return new Success();
    }
}