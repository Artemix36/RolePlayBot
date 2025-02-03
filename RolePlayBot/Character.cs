using Microsoft.EntityFrameworkCore;
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
    public async Task<List<Character>?> GetCharacters(string ConnectionString)
    {
        using (CharacterContext db = new CharacterContext(ConnectionString))
        {
            CharacterLogger.LogInformation("Got new request to get list of Characters. Checking if TGID is not 0");
            if(TelegramID > 0)
            {
                CharacterLogger.LogInformation("Processing request for Characters of TGID: {TGID}", this.TelegramID);
                List<Character>? Characters = await db.Characters.Where(c => c.TelegramID == this.TelegramID).ToListAsync();
                if(Characters is not null)
                {
                    CharacterLogger.LogInformation("Found {number} characters by TGID: {TGID}", Characters.Count() ,this.TelegramID);
                    return Characters;
                }
                else
                {
                    CharacterLogger.LogInformation("Not Found characters by TGID: {TGID}",this.TelegramID);
                    return null;
                }
            }
            if(ID > 0)
            {
                CharacterLogger.LogInformation("Processing request for Characters with ID: {TGID}", ID);
                List<Character>? Characters = await db.Characters.Where(c => c.ID == ID).ToListAsync();
                if(Characters is not null)
                {
                    CharacterLogger.LogInformation("Found {number} characters by ID: {TGID}", Characters.Count() , ID);
                    return Characters;
                }
                else
                {
                    CharacterLogger.LogInformation("Not Found characters by ID: {TGID}", ID);
                    return null;
                }
            }
            else
            {
                CharacterLogger.LogInformation("ID or TGID:{TGID} is invalid! Processing will be stopped", TelegramID);
                return null;
            }
        }
    }
    public async Task<int> ChangeCharacter(string ConnectionString)
    {
        CharacterLogger.LogInformation("Got Request to Change Character by ID: {ID}", this.ID);
        Stat characterStat = new Stat{CharacterID = ID};
        Role? role = new Role();
        role.ID = RoleID;
        role = role.GetRole(ConnectionString);
        if(ID > 0 && await characterStat.DoesCharacterExist(ConnectionString) && role is not null)
        {
            try
            {
                using(CharacterContext db = new CharacterContext(ConnectionString))
                {
                    db.Characters.Entry(this).State = EntityState.Modified;
                    db.Characters.Update(this);
                    await db.SaveChangesAsync();
                    CharacterLogger.LogInformation("Updated Character by ID: {ID} SET: {Name} - {Age} - {TGID} - {ROLEId}", ID, Name, Age, TelegramID, RoleID);
                    return 1;
                }
            }
            catch(Exception ex)
            {
                CharacterLogger.LogError("Failed to update Character with ID: {ID} with exception {exception}", ID, ex.Message);
                return -1;
            }
        }
        else
        {
            CharacterLogger.LogInformation("Couldn't Change Character for {ID}: ID can't be 0 or less/Character doesn't exist", ID);
            return 0;
        }
    }
}
