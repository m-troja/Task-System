using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using Task_System.Controller;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Service;
using Task_System.Config;

namespace Task_System.Data;

public class PostgresqlDbContext : DbContext
{
    private readonly ILogger<IssueController> l;

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Key> Keys { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Team> Teams { get; set; }
    public PostgresqlDbContext(DbContextOptions<PostgresqlDbContext> options, ILogger<IssueController> l)
        : base(options)
    {
        this.l = l;
    }
  
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DotNetEnv.Env.Load("dev.env");
        var dbName = Environment.GetEnvironmentVariable("TS_DB_NAME") ?? "testdb";
        var dbHost = Environment.GetEnvironmentVariable("TS_DB_HOST") ?? "localhost";
        var dbPort = Environment.GetEnvironmentVariable("TS_DB_PORT") ?? "5432";
        var dbUser = Environment.GetEnvironmentVariable("TS_DB_USER") ?? "postgres";
        var dbPassword = Environment.GetEnvironmentVariable("TS_DB_PASSWORD") ?? "postgres";

        l.log($"DB_NAME: {dbName}, DB_HOST: {dbHost}, DB_PORT: {dbPort}, DB_USER: {dbUser}");
        
        Console.WriteLine($"Connecting to PostgreSQL at {dbHost}:{dbPort}, Database: {dbName}, User: {dbUser}");
        l.log($"Connecting to PostgreSQL at {dbHost}:{dbPort}, Database: {dbName}, User: {dbUser}");
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

            optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //  USER - ROLE many-to-many relationship
        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users);

        // ISSUE - relationship to USER (Assignee)
        modelBuilder.Entity<Issue>()
          .HasOne(i => i.Assignee)
          .WithMany(u => u.AssignedIssues)
          .HasForeignKey("AssigneeId")
          .OnDelete(DeleteBehavior.Restrict);
        ;
        // ISSUE - relationship to USER (Author)
        modelBuilder.Entity<Issue>()
          .HasOne(i => i.Author)
          .WithMany(u => u.AuthoredIssues)
          .HasForeignKey("AuthorId")
          .OnDelete(DeleteBehavior.Restrict);

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
        new Role { Id = 1, Name = "ROLE_USER" },
        new Role { Id = 2, Name = "ROLE_ADMIN" }
       );

        // COMMENT - relationships to USER (Author) and ISSUE (Issue)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Issue)
            .WithMany(i => i.Comments)
            .HasForeignKey("IssueId")
            .OnDelete(DeleteBehavior.Cascade);

        // PROJECT - relationship to ISSUE (Issues)
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Issues)
            .WithOne(i => i.Project)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        // KEY - one-to-one relationship with ISSUE 
        modelBuilder.Entity<Key>()
            .HasOne(k => k.Issue)
            .WithOne(i => i.Key)
            .HasForeignKey<Key>(k => k.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // KEY - many-to-one relationship with PROJECT
        modelBuilder.Entity<Key>()
            .HasOne(k => k.Project)
            .WithMany(p => p.Keys)
            .HasForeignKey(k => k.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ACTIVITY - relationship to ISSUE (Issue)
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.Issue)
            .WithMany(i => i.Activities)
            .HasForeignKey(a => a.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // ISSUE - relationship to TEAM (Team)
        modelBuilder.Entity<Issue>()
            .HasOne( i => i.Team)
            .WithMany(t => t.Issues)
            .HasForeignKey(i => i.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    // Automatically set CreatedAt for entities implementing IAutomaticDates
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;


        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAutomaticDates &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var auditable = (IAutomaticDates)entry.Entity;

            if (entry.State == EntityState.Added)
                auditable.CreatedAt = nowUtc;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
