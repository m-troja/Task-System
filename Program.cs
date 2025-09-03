using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Task_System.Data;
using Task_System.Model;
using Task_System.Service.Impl;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja DbContext
builder.Services.AddDbContext<PostgresqlDbContext>();

// Rejestracja serwisów
builder.Services.AddScoped<UserService>();

// Rejestracja kontrolerów
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();  // mapuje wszystkie kontrolery
});


app.Urls.Add("http://localhost:5000");


// Run HTTP server
app.Run();

Role role = new Role("test");

