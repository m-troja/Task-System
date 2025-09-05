using Microsoft.EntityFrameworkCore;
using System;
using Task_System.Model.Entity;

namespace Task_System.Data
{
    public class PostgresqlDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public PostgresqlDbContext(DbContextOptions<PostgresqlDbContext> options)
            : base(options)
        {
        }
      
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DotNetEnv.Env.Load("dev.env");

            var dbName = Environment.GetEnvironmentVariable("TS_DB_NAME") ?? "testdb";
            var dbHost = Environment.GetEnvironmentVariable("TS_DB_HOST") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("TS_DB_PORT") ?? "5432";
            var dbUser = Environment.GetEnvironmentVariable("TS_DB_USER") ?? "postgres";
            var dbPassword = Environment.GetEnvironmentVariable("TS_DB_PASSWORD") ?? "secret";
           
            Console.WriteLine($"Connecting to PostgreSQL at {dbHost}:{dbPort}, Database: {dbName}, User: {dbUser}");

            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users);
        }
    }
}
