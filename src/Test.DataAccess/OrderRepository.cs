using Microsoft.EntityFrameworkCore;
using OneOf;
using Test.Core;
using Test.Core.Models;

namespace Test.DataAccess;

public class OrderRepository : IOrderRepository
{
    private readonly TestApiDbContext _context;
    public OrderRepository(TestApiDbContext context)
    {
        _context = context;   
    }
    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public Task<OneOf<Order, NotFound>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<Order, MissingReferenceData>> CreateAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<Order, NotFound, MissingReferenceData>> UpdateAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<Success, NotFound>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}