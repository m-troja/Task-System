using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using Task_System.Data;
using Task_System.Exception.Handler;
using Task_System.Model.DTO.Cnv;
using Task_System.Security;
using Task_System.Service;
using Task_System.Service.Impl;

// Load env variables
DotNetEnv.Env.Load("dev.env");
var _LogDir = Environment.GetEnvironmentVariable("TS_LOG_DIR") ?? "/home/michal";
var _HttpPort = Environment.GetEnvironmentVariable("TS_HTTP_PORT") ?? "6901";

// -------------------
// Configure Serilog
// -------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(_LogDir,
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: 7,
                  outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Task-System WebApplication...");

    var builder = WebApplication.CreateBuilder(args);

    // -------------------
    // JWT Config
    // -------------------
    var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret not configured.");
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured.");
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience not configured.");

    // Register JwtGenerator
    builder.Services.AddSingleton<JwtGenerator>(sp =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var logger = sp.GetRequiredService<ILogger<JwtGenerator>>();
        return new JwtGenerator(config, logger);
    });

    // Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(jwtSecret); // zmiana na UTF8
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

    builder.Services.AddAuthorization();

    // -------------------
    // Replace default logging with Serilog
    // -------------------
    builder.Host.UseSerilog();

    // -------------------
    // Register services
    // -------------------
    builder.Services.AddDbContext<PostgresqlDbContext>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IActivityService, ActivityService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IIssueService, IssueService>();
    builder.Services.AddScoped<IRegisterService, RegisterService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<ILoginService, LoginService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITeamService, TeamService>();
    builder.Services.AddScoped<UserCnv>();
    builder.Services.AddScoped<TeamCnv>();
    builder.Services.AddScoped<CommentCnv>();
    builder.Services.AddScoped<IssueCnv>();
    builder.Services.AddScoped<ProjectCnv>();
    builder.Services.AddScoped<PasswordService>();

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

    // ALLOW CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            policy => policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });


    // App HttpPort
    var app = builder.Build();

    // CORS
    app.UseCors("AllowAll");

    builder.WebHost.UseUrls($"http://0.0.0.0:{_HttpPort}");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (args.Length == 0 || !args[0].Contains("ef"))
    {
        app.UseMiddleware<GlobalExceptionHandler>();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        Log.Information("Task-System WebApplication started successfully.");
        app.Run();
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}
