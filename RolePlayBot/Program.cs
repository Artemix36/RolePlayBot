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
        ILogger logger = loggerFactory.CreateLogger("Main");
        
    }
}

sealed class Bot
{
        public BotVars botVars { internal get; pub; ic set; };
        static ILogger BotLogger;

        public Bot(ILoggerFactory loggerFactory, BotVars ConfigBotVars)
        {   
            BotLogger = loggerFactory.CreateLogger("RPBot");
            botVars = ConfigBotVars;
        }

        static void Start()
        {
            try
            {
                TelegramBotClient telegram_bot = new TelegramBotClient(TGtoken);
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
    public string TGtoken { internal get; set; } = string.Empty;
    public string? DBBaseURL { internal get; set; } = string.Empty;
    public string? StarttupMessage { internal get; set; } = string.Empty;
}