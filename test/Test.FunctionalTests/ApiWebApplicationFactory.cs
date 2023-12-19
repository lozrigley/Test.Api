using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

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