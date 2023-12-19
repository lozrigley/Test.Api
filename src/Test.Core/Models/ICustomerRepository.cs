
using OneOf;
using Test.Core.Models;
namespace Test.Core.Models;
public interface ICustomerRepository
{
    public Task<List<Customer>> GetAllAsync();
    
    public Task<OneOf<Customer, NotFound>> GetByIdAsync(Guid id);
    
    public Task<Customer> CreateAsync(Customer customer);
    
    public Task<OneOf<Customer, NotFound>> UpdateAsync(Customer customer);
    
    public Task<OneOf<Success,NotFound>> DeleteAsync(Guid id);
}