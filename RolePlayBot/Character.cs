using Microsoft.Extensions.Logging;
class Character
{
    public int ID {get; set;} = 0;
    public string Name {get; set;} = string.Empty;
    public int? Age {get; set;}
    public long TelegramID {get; set;} = 0;
    public int RoleID {get; set;} = 0;
    private readonly ILogger<Character> CharacterLogger;
    public Character()
    {
        CharacterLogger = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.AddFilter("System", LogLevel.Debug).SetMinimumLevel(LogLevel.Information);
        }).CreateLogger<Character>();
    }
    public int Register(string ConnectionString)
    {
        using (CharacterContext db = new CharacterContext(ConnectionString))
        {
            CharacterLogger.LogInformation("Got new request to register Character {charactername}. Finding Role", this.Name);
            Role? role = new Role();
            role.ID = this.RoleID;
            role = role.GetRole(ConnectionString);
            if(role is not null)
            {
                CharacterLogger.LogInformation("Found corresponding Role: {charactername} is {rolename}. Processing further", this.Name, role.Name);
                try
                {
                    db.Characters.AddRange(this);
                    db.SaveChanges();
                    CharacterLogger.LogInformation("Registered new Character: {charactername}", this.Name);
                    return 1;
                }
                catch(Exception ex)
                {
                    CharacterLogger.LogError(ex.Message);
                    return -1;
                }
            }
            else
            {
                CharacterLogger.LogError("Bad role! Can't register Character!");
                return 0;
            }
        }
    }
}
