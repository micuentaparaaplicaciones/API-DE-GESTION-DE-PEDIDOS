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
var connectionString = builder.Configuration.GetConnectionString("ConexionSGO");

/// Add UserDbContext with Oracle provider 
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseOracle(connectionString).LogTo(Console.WriteLine, LogLevel.Information);
});

/// Add IUserDatabase with UserDatabase implementation 
builder.Services.AddScoped<IUserDataService, UserDataService>();

/// Add IUserBusinessRules with UserBusinessRules implementation
builder.Services.AddScoped<IUserBusinessRules, UserBusinessRules>();

/// Add IUserSecurity with UserSecurity implementation
builder.Services.AddScoped<IUserSecurityService, UserSecurityService>();

/// Add Identity services with UserDbContext and User entity
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

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

//app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
