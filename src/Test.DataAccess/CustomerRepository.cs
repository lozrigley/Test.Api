using Microsoft.EntityFrameworkCore;
using OneOf;
using Test.Core;
using Test.Core.Models;

namespace Test.DataAccess;

public class CustomerRepository : ICustomerRepository
{
    
    private readonly TestApiDbContext _context;
    public CustomerRepository(TestApiDbContext context)
    {
        _context = context;   
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<OneOf<Customer, NotFound>> GetByIdAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer is not null)
        {
            return customer;
        }

        return new NotFound();
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<OneOf<Customer, NotFound>> UpdateAsync(Customer customer)
    {
        var existingCustomer = await _context.Customers.FindAsync(customer.Id);
        if (existingCustomer is null)
        {
            return new NotFound();
        }

        _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
        await _context.SaveChangesAsync();
        return customer;

    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            return new NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return new Success();
    }
}