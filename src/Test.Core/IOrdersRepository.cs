using Test.Core.Models;
using OneOf;

namespace Test.Core;

public interface IOrderRepository
{
    public Task<List<Order>> GetAllAsync();
    
    public Task<OneOf<Order, NotFound>> GetByIdAsync(Guid id);
    
    public Task<OneOf<Order, MissingReferenceData>> CreateAsync(OrderDto orderDto);
    
    public Task<OneOf<Order, NotFound, MissingReferenceData>> UpdateAsync(OrderDto orderDto);
    
    public Task<OneOf<Success,NotFound>> DeleteAsync(Guid id);
}

public record MissingReferenceData();

public record OrderDto
{
    public Guid Id { get; init; }
    public Guid CustomerId{ get; init; }
    public Guid ProductId { get; init; }
    public OrderStatus Status { get; init; }
}


