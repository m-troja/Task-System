using System.Text.Json.Serialization;
using Serilog;
using Task_System.Data;
using Task_System.Exception.Handler;
using Task_System.Model.DTO.Cnv;
using Task_System.Service;
using Task_System.Service.Impl;

// -------------------
// Configure Serilog
// -------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(@"C:\tmp\log\Task-System.log",
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: 7)
    .CreateLogger();

try
{
    Log.Information("Starting Task-System WebApplication...");

    var builder = WebApplication.CreateBuilder(args);

    // -------------------
    // Replace default logging with Serilog
    // -------------------
    builder.Host.UseSerilog();

    // -------------------
    // Register services
    // -------------------
    builder.Services.AddDbContext<PostgresqlDbContext>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IIssueService, IssueService>();
    builder.Services.AddScoped<IRegisterService, RegisterService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<UserCnv>();
    builder.Services.AddScoped<CommentCnv>();
    builder.Services.AddScoped<IssueCnv>();
    builder.Services.AddScoped<ProjectCnv>();
    builder.Services.AddControllers()
           .AddJsonOptions(opt =>
           {
               opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
           });

    // -------------------
    // Swagger
    // -------------------
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionHandler>();
    app.UseRouting();
    app.MapControllers();

    Log.Information("Task-System WebApplication started successfully.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}
