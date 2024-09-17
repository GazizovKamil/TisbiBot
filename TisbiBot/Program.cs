using Telegram.Bot;  // Импорт библиотеки Telegram.Bot для работы с Telegram API
using Telegram.Bot.Polling;  // Импортируем для использования механизма long polling
using Telegram.Bot.Types;  // Импортируем типы данных, связанные с Telegram, такие как Update, Message и другие
using Telegram.Bot.Exceptions;  // Для обработки исключений, возникающих при работе с Telegram API

// Создаем экземпляр клиента Telegram Bot с помощью токена вашего бота
var botClient = new TelegramBotClient("your-bot-token-here");

// Источник для управления отменой асинхронных операций (например, завершение polling)
var cancellationTokenSource = new CancellationTokenSource();

// Настройки приемника обновлений (ReceiverOptions)
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }, // Получать все типы обновлений (например, сообщения, команды, фото и т.д.)
};

// Запускаем прием обновлений с помощью метода long polling
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,  // Метод для обработки входящих обновлений (сообщений)
    pollingErrorHandler: HandlePollingErrorAsync,  // Метод для обработки ошибок, возникающих в процессе long polling
    receiverOptions: receiverOptions,  // Опции для настройки, какие обновления получать
    cancellationToken: cancellationTokenSource.Token  // Токен отмены для завершения приема обновлений
);

// Асинхронный метод для обработки входящих обновлений
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Проверяем, что update содержит сообщение, иначе завершаем обработку
    if (update.Message is not { } message) return;
    
    // Проверяем, что сообщение содержит текст, иначе выходим
    if (message.Text is not { } messageText) return;

    // Извлекаем идентификатор чата, чтобы знать, куда отправить ответное сообщение
    long chatId = message.Chat.Id;

    // Отправляем сообщение обратно в чат с текстом, который был получен от пользователя
    await botClient.SendTextMessageAsync(
        chatId: chatId,  // Указываем идентификатор чата
        text: $"You said: {messageText}",  // Формируем текст ответа, повторяя сообщение пользователя
        cancellationToken: cancellationToken  // Указываем токен отмены для управления асинхронной задачей
    );
}

// Метод для обработки ошибок, которые могут возникнуть в процессе long polling
Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    // Определяем тип ошибки. Если это ошибка API, выводим код и сообщение.
    var ErrorMessage = exception switch
    {
        // Если это ошибка запроса к API Telegram, выводим код и сообщение ошибки
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",

        // Для всех остальных исключений выводим их в текстовом виде
        _ => exception.ToString()
    };

    // Выводим сообщение об ошибке в консоль
    Console.WriteLine(ErrorMessage);
    
    // Возвращаем завершенную задачу
    return Task.CompletedTask;
}

// Не даем приложению завершиться сразу, ожидаем команду в консоли (чтобы бот продолжал работать)
Console.ReadLine();
