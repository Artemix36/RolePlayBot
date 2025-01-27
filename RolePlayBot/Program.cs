using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Configuration;

class Programm
{
    static void Main()
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
            Bot? bot = configuration.Get<Bot>();
        }
        catch(Exception ex)
        {
            logger.LogError("Failed to get vars: {exception}", ex.Message);
        }
    }
}

sealed class Bot
{
        public static string TGtoken {internal get; set;} = string.Empty;
        public static string? DBBaseURL {internal get; set;} = string.Empty;
        public static string? StarttupMessage {internal get; set;} = string.Empty;
        static ILogger BotLogger;
        Bot(ILoggerFactory loggerFactory, string configTGtoken, string? configDBBaseURL, string? configStarttupMessage)
        {
            BotLogger = loggerFactory.CreateLogger("Bot");
            TGtoken = configTGtoken;
            DBBaseURL = configDBBaseURL;
            StarttupMessage = configStarttupMessage;
        }
        static void Start()
        {
            try
            {
                TelegramBotClient telegram_bot = new TelegramBotClient(Bot.TGtoken);
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