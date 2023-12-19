using Microsoft.EntityFrameworkCore;
using Test.Api;
using Test.Core;
using Test.DataAccess;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TestApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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

builder.Services.AddScoped<IProductRepository, ProductRepository>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseCors("AllowAll");
app.AddProductEndpoints();
app.Run();