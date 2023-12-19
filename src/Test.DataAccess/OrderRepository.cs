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

    public async Task<OneOf<Order, NotFound>> GetByIdAsync(Guid id)
    {
        var order= await _context.Orders.FindAsync(id);

        if (order is not null)
        {
            return order;
        }

        return new NotFound();
    }

    public async Task<OneOf<Order, MissingReferenceData>> CreateAsync(OrderDto orderDto)
    {
        var customer = await _context.Customers.FindAsync(orderDto.CustomerId);
        var product = await _context.Products.FindAsync(orderDto.ProductId);
        if (product is null || customer is null)
        {
            return new MissingReferenceData();
        }
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = orderDto.CustomerId,
            ProductId = orderDto.ProductId,
            Status = orderDto.Status,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = null
        };
        
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<OneOf<Order, NotFound, MissingReferenceData>> UpdateAsync(OrderDto orderDto)
    {
        var customer = await _context.Customers.FindAsync(orderDto.CustomerId);
        var product = await _context.Products.FindAsync(orderDto.ProductId);
        if (product is null || customer is null)
        {
            return new MissingReferenceData();
        }
        var existingOrder = await _context.Orders.FindAsync(orderDto.Id);
        if (existingOrder is null)
        {
            return new NotFound();
        }
        
        var order = new Order
        {
            Id = orderDto.Id,
            CustomerId = orderDto.CustomerId,
            ProductId = orderDto.ProductId,
            Status = orderDto.Status,
            CreatedDate = existingOrder.CreatedDate,
            UpdatedDate = DateTime.UtcNow
        };
        
        _context.Entry(existingOrder).CurrentValues.SetValues(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null)
        {
            return new NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return new Success();
    }
}