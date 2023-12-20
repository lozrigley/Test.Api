using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Api;
using Test.Core;
using Test.Core.Models;
using Test.DataAccess;

var builder = WebApplication.CreateBuilder(args);

var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (isDocker)
{
    builder.Configuration.AddJsonFile("appsettings.Docker.json", false, true);
}
else
{
    builder.Configuration.AddJsonFile("appsettings.json", false, true);
}
builder.Services.AddDbContext<TestApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation($"IsDocker = {isDocker}");

app.MapGet("/", () => "Hello World!");
app.UseCors("AllowAll");
app.AddProductEndpoints();
app.AddCustomerEndpoints();
app.AddOrderEndpoints();
app.Run();