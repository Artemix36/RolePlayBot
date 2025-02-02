using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;

class MessageHandler
{
    public static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.CallbackQuery != null)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery, cancellationToken);
                return;
            }

            var message = update.Message;
            var user = message.From;
            var chat = message.Chat;
            InlineKeyboardMarkup inlineKeyboard = null;
            Console.WriteLine($"Получено сообщение: {message.Text}");

            switch (message.Text)
            {
                case "/start":
                    inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Добавить нового персонажа", "/register"),
                            InlineKeyboardButton.WithCallbackData("Ваши персонажи", "/characters")
                        },
                    });
                    break;
                case "/register":
                    
                    break;
                case "/characters":
                    
                    break; 
            }

            string responseText = "Выберите действие:";
            await botClient.SendTextMessageAsync(chat.Id, responseText, replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        try
        {
            var user = callbackQuery.From;
            var chat = callbackQuery.Message.Chat;
            string path = "BotVars.json";
            InlineKeyboardMarkup replyMarkup =  null;
            string responseText = "Произошла ошибка. Попробуйте снова";
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(path, optional: false, reloadOnChange: true).Build();
            BotVars? BotVars = configuration.Get<BotVars>();
            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

            switch (callbackQuery.Data)
            {
                case "/register":
                    {
                        responseText = "/reg";
                        replyMarkup = null;
                        await botClient.SendTextMessageAsync(chat.Id, responseText,  cancellationToken: cancellationToken);
                        return;
                    }
                case "/characters":
                    {
                        Character testCharacter1 = new Character { Name = "MegaGAY1", Age = 12, RoleID = 1, TelegramID = 1 };
                        Character testCharacter2 = new Character { Name = "MegaGAY2", Age = 13, RoleID = 2, TelegramID = 1 };
                        List<Character>? characters = await testCharacter2.GetCharacters(BotVars.ConnectionString);
                        List<InlineKeyboardButton[]> buttons = new();
                        if (characters is not null && characters.Count != 0)
                        {
                            responseText = "Выберите персонажа:";
                            foreach (var character in characters)
                            {
                                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(character.Name, $"character_{character}") });
                            }
                        }
                        else
                        {
                            responseText = "У нет персонажа";
                        }

                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "back") });
                        replyMarkup = new InlineKeyboardMarkup(buttons);
                                    
                        break;
                    }
                case "/character":
                    {
                        responseText = "";
                        var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Вся информация", "/info"),
                                InlineKeyboardButton.WithCallbackData("Халтурики", "/gigs"),
                            },
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Добавить эди", "/ed+"),
                                InlineKeyboardButton.WithCallbackData("Отнять Эди", "/ed-")
                            },
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Добавить опыт", "/hp+"),
                                InlineKeyboardButton.WithCallbackData("Отнять опыт", "/hp-"),
                            },
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Назад", "/characters")
                            }
                        });
                        replyMarkup = null;
                        break;
                    }
            }

            await botClient.EditMessageTextAsync( chatId: callbackQuery.Message.Chat.Id, messageId: callbackQuery.Message.MessageId, text: responseText, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

}