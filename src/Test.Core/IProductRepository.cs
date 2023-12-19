using OneOf;
using Test.Core.Models;

namespace Test.Core;

public interface IProductRepository
{
    public Task<List<Product>> GetAllAsync();
    
    public Task<OneOf<Product, NotFound>> GetByIdAsync(Guid id);
    
    public Task<Product> CreateAsync(Product product);
    
    public Task<OneOf<Product, NotFound>> UpdateAsync(Product product);
    
    public Task<OneOf<Success,NotFound>> DeleteAsync(Guid id);
}

public record NotFound();
public record Success();