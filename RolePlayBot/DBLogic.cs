using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;

class CharacterContext : DbContext
{
    private readonly string connectionString;
    private readonly ILogger<CharacterContext> CharacterDBlogger;
    public DbSet<Character> Characters { get; set; }
    public CharacterContext(string ConfigConnectionString)
    {
        CharacterDBlogger = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.AddFilter("System", LogLevel.Debug).SetMinimumLevel(LogLevel.Information);
        }).CreateLogger<CharacterContext>();
        connectionString = ConfigConnectionString ?? throw new ArgumentNullException(nameof(connectionString));

        try
        {
            CharacterDBlogger.LogDebug("Connection string was supplied");
            Database.EnsureCreated();
        }
        catch(Exception ex)
        {
            CharacterDBlogger.LogError("Couldn't start DBContext: {exception}, {exceptionAll}", ex.Message, ex);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }
}

class RoleContext : DbContext
{
    private readonly string connectionString;
    private readonly ILogger<RoleContext> RoleDBLogger;
    public DbSet<Role> Roles { get; set; }

    public RoleContext(string ConfigConnectionString)
    {
        RoleDBLogger = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.AddFilter("System", LogLevel.Debug).SetMinimumLevel(LogLevel.Information);
        }).CreateLogger<RoleContext>();
        connectionString = ConfigConnectionString ?? throw new ArgumentNullException(nameof(connectionString));

        try
        {
            RoleDBLogger.LogDebug("Connection string was supplied");
            Database.EnsureCreated();
        }
        catch(Exception ex)
        {
            RoleDBLogger.LogError("Couldn't start DBContext: {exception}, {exceptionAll}", ex.Message, ex);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }
}

class StatContext: DbContext
{
    private readonly string connectionString;
    private readonly ILogger<StatContext> StatDBLogger;
    public DbSet<Stat> Stats { get; set; }
    public StatContext(string ConfigConnectionString)
    {
        StatDBLogger = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.AddFilter("System", LogLevel.Debug).SetMinimumLevel(LogLevel.Information);
        }).CreateLogger<StatContext>();
        connectionString = ConfigConnectionString ?? throw new ArgumentNullException(nameof(connectionString));

        try
        {
            StatDBLogger.LogDebug("Connection string was supplied");
            Database.EnsureCreated();
        }
        catch(Exception ex)
        {
            StatDBLogger.LogError("Couldn't start DBContext: {exception}, {exceptionAll}", ex.Message, ex);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseNpgsql(connectionString);
    }
}