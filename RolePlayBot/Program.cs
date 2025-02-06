using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using static Telegram.Bot.TelegramBotClient;

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

                using var cts = new CancellationTokenSource();

                MessageHandler messageHandler = new MessageHandler();
                BotLogger.LogInformation("Bot started");
                telegram_bot.StartReceiving(MessageHandler.UpdateHandler, MessageHandler.ErrorHandler, receiverOptions, cts.Token);
                Console.ReadLine();
                cts.Cancel();
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