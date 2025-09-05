using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Service;
using Task_System.Service.Impl;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja DbContext
builder.Services.AddDbContext<PostgresqlDbContext>();

// Rejestracja serwisów
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();

// Rejestracja kontrolerów
builder.Services.AddControllers();


var app = builder.Build();

app.UseRouting();
app.MapControllers();  

// Run HTTP server
app.Run();

Role role = new Role("test");

