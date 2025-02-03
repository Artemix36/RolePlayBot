using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
class Stat
{
    public int XP {get; set;} = 0;
    public int Money {get; set;} = 0;
    [Key]
    public int CharacterID {get; set;} = 0;
    private readonly ILogger<Stat> StatLogger;
    public Stat()
    {
        StatLogger = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.AddFilter("System", LogLevel.Debug).SetMinimumLevel(LogLevel.Information);
        }).CreateLogger<Stat>();
    }
    public Stat? GetStats(string ConnectionString)
    {
        StatLogger.LogInformation("Got Request to Get Stats for Character by Character ID: {ID}", this.CharacterID);
        if(CharacterID > 0)
        {
            try
            {
                using (StatContext db = new StatContext(ConnectionString))
                {
                    Stat? stat = new Stat();
                    stat = db.Stats.FirstOrDefault(r => r.CharacterID == this.CharacterID);
                    if(stat is not null)
                    {
                        StatLogger.LogInformation("Found Stats for Character {ID}", this.CharacterID);
                        return stat;
                    }
                    else
                    {
                        StatLogger.LogInformation("Not Found Stats for Character {ID}", this.CharacterID);
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                StatLogger.LogError("Couldn't add stats for Character by Character ID: {ID} with: {exception}", CharacterID, ex.Message);
                return null;
            }
        }
        else
        {
            StatLogger.LogInformation("Character ID can't be 0 or less");
            return null;
        }
    }
    public async Task<int> AddStat(string ConnectionString)
    {
        StatLogger.LogInformation("Got Request to Add Stats to Character by Character ID: {ID}", this.CharacterID);
        Stat? stat = new Stat();
        stat.CharacterID = this.CharacterID;
        stat = stat.GetStats(ConnectionString);
        if(CharacterID > 0 && stat is null && await DoesCharacterExist(ConnectionString))
        {
            try
            {
                using (StatContext db = new StatContext(ConnectionString))
                {
                    db.Add(this);
                    db.SaveChanges();
                    StatLogger.LogInformation("Added stats for Character {ID}", CharacterID);
                    return 1;
                }
            }
            catch(Exception ex)
            {
                StatLogger.LogError("Couldn't Add Stat for Character {ID} with: {exception}", CharacterID, ex.Message);
                return -1;
            }
        }
        else
        {
            StatLogger.LogInformation("Couldn't Add Stats for Character ID {ID}: ID can't be 0 or less/Stat is existing", CharacterID);
            return 0;
        }
    }
    public async Task<int> ChangeStat(string ConnectionString)
    {
        StatLogger.LogInformation("Got Request to Change Stats for Character by Character ID: {ID}", this.CharacterID);
        if(CharacterID > 0 && await DoesCharacterExist(ConnectionString))
        {
            try
            {
                using (StatContext db = new StatContext(ConnectionString))
                {
                    db.Stats.Entry(this).State = EntityState.Modified;
                    db.Stats.Update(this);
                    db.SaveChanges();
                    StatLogger.LogInformation("Updated Stats for Character by ID: {ID} SET: {XP} - {Money}", CharacterID, XP, Money);
                    return 1;
                }
            }
            catch(Exception ex)
            {
                StatLogger.LogError("Couldn't update stats for Character by Character ID: {ID} with: {exception}", CharacterID, ex.Message);
                return -1;
            }
        }
        else
        {
            StatLogger.LogInformation("Couldn't Change Stats for Character ID {ID}: ID can't be 0 or less/Character doesn't exist",CharacterID);
            return 0;
        }
    }
    protected async Task<bool> DoesCharacterExist(string ConnectionString)
    {
        Character character = new Character();
        character.ID = CharacterID;
        List<Character>? characters = await character.GetCharacters(ConnectionString);
        if(characters is not null && characters.Count != 0)
        {
            return true;
        }
        return false;
    }
}