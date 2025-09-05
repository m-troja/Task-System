using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Task_System.Data;
using Task_System.Exception.Handler;
using Task_System.Model.Entity;
using Task_System.Service;
using Task_System.Service.Impl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PostgresqlDbContext>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();


app.UseRouting();
app.MapControllers();  

app.Run();

Role role = new Role("test");

