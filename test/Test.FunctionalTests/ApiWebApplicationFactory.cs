using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test.DataAccess;

namespace Test.FunctionalTests;

public class ApiWebApplicationFactory : WebApplicationFactory<Test.Api.Test>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {

        });

        builder.ConfigureTestServices(services =>
        {

        });
        
    }
}

public class ApiWebApplicationFactoryWithInMemoryDatabase : WebApplicationFactory<Test.Api.Test>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {

        });

        builder.ConfigureTestServices(services =>
        {
            // Remove the existing context configuration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TestApiDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a database context using an in-memory provider
            services.AddDbContext<TestApiDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Build the service provider and create a scope
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<TestApiDbContext>();
                
                // Ensure the database is created
                db.Database.EnsureCreated();

                // Seed the database with test data if necessary
            }
        });
        
    }
}