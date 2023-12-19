using Test.Core.Models;
using OneOf;

namespace Test.Core;

public interface IOrderRepository
{
    public Task<List<Order>> GetAllAsync();
    
    public Task<OneOf<Order, NotFound>> GetByIdAsync(Guid id);
    
    public Task<OneOf<Order, MissingReferenceData>> CreateAsync(Order order);
    
    public Task<OneOf<Order, NotFound, MissingReferenceData>> UpdateAsync(Order order);
    
    public Task<OneOf<Success,NotFound>> DeleteAsync(Guid id);
}

public record MissingReferenceData();


