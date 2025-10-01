using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Refit;
using CurrencyExchange.Application.Mappings;
using CurrencyExchange.Application.Services;
using CurrencyExchange.Core.Interfaces;
using CurrencyExchange.Infrastructure.Data;
using CurrencyExchange.Infrastructure.ExternalServices;
using CurrencyExchange.Infrastructure.Factory;
using CurrencyExchange.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework
builder.Services.AddDbContext<CurrencyExchangeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=currencyexchange.db"));

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure Refit
builder.Services.AddRefitClient<IOpenExchangeRatesApi>()
    .ConfigureHttpClient(c => 
    {
        c.BaseAddress = new Uri("https://openexchangerates.org/api");
        c.Timeout = TimeSpan.FromSeconds(30);
    });

// Register repositories
builder.Services.AddScoped<IConversionHistoryRepository, ConversionHistoryRepository>();

// Register services
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<ICurrencyConverterService, CurrencyConverterService>();

// Register factory
builder.Services.AddScoped<IConversionStrategyFactory, ConversionStrategyFactory>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Currency Exchange API",
        Version = "v1",
        Description = "A Web API for currency conversion using real-time exchange rates",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@example.com"
        }
    });
    
    // Add XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Exchange API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyExchangeDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
