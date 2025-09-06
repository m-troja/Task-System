using System.Text.Json.Serialization;
using Task_System.Data;
using Task_System.Exception.Handler;
using Task_System.Model.DTO.Cnv;
using Task_System.Service;
using Task_System.Service.Impl;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PostgresqlDbContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<UserCnv>();
builder.Services.AddControllers().AddJsonOptions(option => { option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseRouting();
app.MapControllers();  
app.Run();


