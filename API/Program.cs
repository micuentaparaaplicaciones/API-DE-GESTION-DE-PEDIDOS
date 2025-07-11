// 11-7-2025 10:00       product implemented faltan unit tests
using API.BusinessRules;
using API.BusinessRules.Interfaces;
using API.Data;
using API.DataServices;
using API.DataServices.Interfaces;
using API.Entities;
using API.Middleware;
using API.Security;
using API.Security.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

/// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

/// Add CORS policy 
var connectionString = builder.Configuration.GetConnectionString("PEDIDOS");

/// Add ApplicationDbContext with Oracle provider 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseOracle(connectionString).LogTo(Console.WriteLine, LogLevel.Information);
});



/// Add IUserDataService with UserDataService implementation 
builder.Services.AddScoped<IUserDataService, UserDataService>();

/// Add ICustomerDataService with CustomerDataService implementation
builder.Services.AddScoped<ICustomerDataService, CustomerDataService>();

// Add ICategoryDataService with CategoryDataService implementation 
builder.Services.AddScoped<ICategoryDataService, CategoryDataService>();

// Add ISupplierDataService with SupplierDataService implementation 
builder.Services.AddScoped<ISupplierDataService, SupplierDataService>();

// Add IProductDataService with ProductDataService implementation 
builder.Services.AddScoped<IProductDataService, ProductDataService>();



/// Add IUserBusinessRules with UserBusinessRules implementation
builder.Services.AddScoped<IUserBusinessRules, UserBusinessRules>();

/// Add ICutomerBusinessRules with CustomerBusinessRules implementation
builder.Services.AddScoped<ICustomerBusinessRules, CustomerBusinessRules>();

/// Add ICategoryBusinessRules with CategoryBusinessRules implementation
builder.Services.AddScoped<ICategoryBusinessRules, CategoryBusinessRules>();

/// Add ISupplierBusinessRules with SupplierBusinessRules implementation
builder.Services.AddScoped<ISupplierBusinessRules, SupplierBusinessRules>();

/// Add IProductBusinessRules with ProductBusinessRules implementation
builder.Services.AddScoped<IProductBusinessRules, ProductBusinessRules>();



/// Add IUserSecurity with UserSecurity implementation
builder.Services.AddScoped<IUserSecurityService, UserSecurityService>();

/// Add ICutomerSecurity with CutomerSecurity implementation
builder.Services.AddScoped<ICustomerSecurityService, CustomerSecurityService>();

/// Add Identity services with UserDbContext and User entity
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

/// Add Identity services with CustomerDbContext and Customer entity
builder.Services.AddScoped<IPasswordHasher<Customer>, PasswordHasher<Customer>>();

// Add JWT service for token generation
builder.Services.AddScoped<IJwtService, JwtService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
