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
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TestApiDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }


            //It appears that this in memory EF mock does not respect Ref Integrity which is a bit rubbish IMO
            services.AddDbContext<TestApiDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
            
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<TestApiDbContext>();
                
                db.Database.EnsureCreated();
            }
        });
        
    }
}