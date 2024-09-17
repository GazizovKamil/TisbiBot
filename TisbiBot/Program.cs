using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    static async Task Main(string[] args)
    {
        // Создаем экземпляр клиента Telegram Bot с помощью токена вашего бота
        var botClient = new TelegramBotClient("7210825116:AAG8k6qh9LROPynn1Pa5hXpwQawbj3MvGeI");

        // Источник для управления отменой асинхронных операций
        var cts = new CancellationTokenSource();

        // Запуск long polling
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Получать все типы обновлений
        };

        // Запуск приема сообщений
        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        Console.WriteLine("Bot is up and running. Press any key to exit...");
        Console.ReadLine();

        cts.Cancel();
    }

    // Асинхронный метод для обработки входящих обновлений
    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
        {
            var message = update.Message;
            var chatId = message.Chat.Id;
            var messageText = message.Text;

            Console.WriteLine($"Received a message from {chatId}: {messageText}");

            // Отправляем клавиатуру с 3 кнопками
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Смешной стикер 1", "sticker_1"),
                    InlineKeyboardButton.WithCallbackData("Смешной стикер 2", "sticker_2"),
                    InlineKeyboardButton.WithCallbackData("Смешной стикер 3", "sticker_3"),
                }
            });

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выберите стикер:",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            var callbackQuery = update.CallbackQuery!;
            var chatId = callbackQuery.Message!.Chat.Id;

            // Обрабатываем нажатие кнопки и отправляем соответствующий стикер и текст
            switch (callbackQuery.Data)
            {
                case "sticker_1":
                    await botClient.SendStickerAsync(chatId, "CAACAgIAAxkBAAEtvSVm6bMWhioptc4iYTF2Wh2xTLdP6gACNDoAArR2CUgmpTx1tcQieDYE", cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(chatId, "Вот вам смешной стикер 1!", cancellationToken: cancellationToken);
                    break;

                case "sticker_2":
                    // Отправляем GIF через file_id
                    await botClient.SendAnimationAsync(chatId, "CgACAgIAAxkBAAOZZum39D-veBmuyNnPCBs_h0shTHwAAtZRAAJM__BKINSk6E2bzgo2BA", cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(chatId, "Вот вам смешной GIF!", cancellationToken: cancellationToken);
                    break;

                case "sticker_3":
                    // Отправляем изображение через file_id
                    await botClient.SendPhotoAsync(chatId, "AgACAgIAAxkBAAOXZum32XPZ0Gj2malRqf1Xsra1aiAAAkDgMRtdIUhLBHyCpfDOcIUBAAMCAAN5AAM2BA", cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(chatId, "Вот вам смешное изображение!", cancellationToken: cancellationToken);
                    break;
            }

        }
    }

    //static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    //{
    //    if (update.Message is Message message && message.Animation is not null)
    //    {
    //        // Получаем идентификатор фотографии
    //        var fileId = message.Animation.FileId;
    //        Console.WriteLine($"File ID: {fileId}");

    //        // Отправляем его обратно пользователю
    //        await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Ваш file_id: {fileId}", cancellationToken: cancellationToken);
    //    }
    //}

    // Метод для обработки ошибок
    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}
