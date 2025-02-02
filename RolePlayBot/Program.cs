using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

class Programm
{
    static async Task Main()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => 
        {
            builder.ClearProviders();
            builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                });
            builder.AddFilter("System", LogLevel.Debug).SetMinimumLevel(LogLevel.Information);
        });
        ILogger logger = loggerFactory.CreateLogger("Program");
        try
        {
            string path = "BotVars.json";
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(path, optional: false, reloadOnChange: true).Build();
            BotVars? BotVars = configuration.Get<BotVars>();

            if(BotVars is not null && BotVars.TGtoken != string.Empty)
            {
                Bot RPBot = new Bot(loggerFactory, BotVars);
                RPBot.Start();
                Character testCharacter = new Character{Name = "MegaGAY", Age = 12, RoleID = 1, TelegramID = 1};
                Role testRole = new Role{Name = "Fixer", Description = "Cool"};
                Stat testStat = new Stat{XP = 10,CharacterID = 1, Money = 2000};
                testStat.ChangeStat(BotVars.ConnectionString);
                List<Character>? characters = await testCharacter.GetCharacters(BotVars.ConnectionString);
                if(characters is not null && characters.Count != 0)
                {
                    testStat = testStat.GetStats(BotVars.ConnectionString);
                    logger.LogInformation($"{testStat.Money}");
                }

            }
            else
            {
                logger.LogError("Couldn't get BotVars for file {file}", path);
            }
        }
        catch(Exception ex)
        {
            logger.LogError("Failed to get vars: {exception}", ex.Message);
        }
    }
}

sealed class Bot
{
        public static BotVars botVars { internal get; set; } = new BotVars();
        private readonly ILogger BotLogger;

        public Bot(ILoggerFactory loggerFactory, BotVars ConfigBotVars)
        {   
            BotLogger = loggerFactory.CreateLogger("RPBot");
            botVars = ConfigBotVars;
        }

        public void Start()
        {
            try
            {
                TelegramBotClient telegram_bot = new TelegramBotClient(botVars.TGtoken);
                ReceiverOptions receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = new[]
                    {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                    },
                    DropPendingUpdates = true,
                };
                BotLogger.LogInformation("Bot started");
            }
            catch(Exception ex)
            {
                BotLogger.LogError("Failed to start: {exception}", ex.Message);
            }
        }
}

sealed class BotVars
{
    public string TGtoken { get; set; } = string.Empty;
    public string? DBBaseURL { get; set; } = string.Empty;
    public string? StarttupMessage { get; set; } = string.Empty;
    public string ConnectionString {get; set;} = string.Empty;
}