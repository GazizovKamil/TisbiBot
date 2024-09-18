# Telegram Bot на C#

## Описание

Этот проект представляет собой простой Telegram-бот на языке C#, который использует API Telegram для отправки стикеров, GIF-анимаций, изображений, видео-заметок и обработки пользовательских файлов.

### Возможности бота:
- Отправка стикеров, GIF-анимаций, изображений и видео-заметок.
- Прием файлов (фотографий, анимаций, стикеров, видео-заметок) и отображение их `file_id`.
- Использование Inline-клавиатуры для взаимодействия с пользователем.

## Требования

Перед началом работы убедитесь, что на вашем компьютере установлены следующие компоненты:

- .NET 6 или новее
- Библиотека `Telegram.Bot 18.0.0` (установите через NuGet) 

## Настройка проекта

### 1. Создание Telegram-бота

1. Откройте Telegram и найдите бота [BotFather](https://t.me/BotFather).
2. Отправьте команду `/newbot`, чтобы создать нового бота.
3. Следуйте инструкциям для задания имени и получения токена.
4. Сохраните токен, который будет использоваться в коде бота для авторизации через API Telegram.
5. Для оформления стилистики бота введите `/mybots`, выберите нужного бота и нажмите **Edit bot**.

### 2. Создание проекта
1. Создайте новый консольный проект.
2. Установите пакет `Telegram.Bot 18.0.0`, который будет использоваться для работы с Telegram API.

### 3. Написание кода бота

#### 3.1. Стандартная `Main` функция выглядит следующим образом:

```csharp
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
        var botClient = new TelegramBotClient("ВАШ_ТЕЛЕГРАМ_БОТ_ТОКЕН");

        var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Получать все типы обновлений
        };

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
}
```

####3.2. Напишем сразу HandleErrorAsync для обработки ошибок бота и чтобы он не ломался:
```csharp
static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}
```

####3.3. Основной функционал (HandleUpdateAsync):
Этот метод может обрабатывать команды, получать медиафайлы от пользователей и отвечать на выбор callback-кнопок. Основной метод программы — HandleUpdateAsync — обрабатывает все обновления от Telegram API, такие как сообщения и callback-запросы.

####3.3.1. Определение chatId для дальнейшей работы с ботом:

```csharp
long chatId;

if (update.Type == UpdateType.Message && update.Message is { } message)
{
    chatId = message.Chat.Id;
}
else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Message is { } msg)
{
    chatId = msg.Chat.Id;
}
else
{
    return;
}
```
Этот блок кода проверяет тип входящего обновления, полученного ботом, и в зависимости от того, является ли это сообщение или callback-запрос, получает chatId, который используется для отправки ответа пользователю.

####3.3.2. Обработка callback-запросов:

```csharp
if (update.Type == UpdateType.CallbackQuery)
{
    var callbackQuery = update.CallbackQuery!;
    switch (callbackQuery.Data)
    {
        case "sticker_1":
            await botClient.SendStickerAsync(chatId, "СТИКЕР_ID", cancellationToken: cancellationToken);
            await botClient.SendTextMessageAsync(chatId, "Вот вам смешной стикер!", cancellationToken: cancellationToken);
            break;

        case "sticker_2":
            await botClient.SendAnimationAsync(chatId, "GIF_ID", cancellationToken: cancellationToken);
            await botClient.SendTextMessageAsync(chatId, "Вот вам смешной GIF!", cancellationToken: cancellationToken);
            break;

        case "sticker_3":
            await botClient.SendPhotoAsync(chatId, "PHOTO_ID", cancellationToken: cancellationToken);
            await botClient.SendTextMessageAsync(chatId, "Вот вам смешное изображение!", cancellationToken: cancellationToken);
            break;

        case "circle_1":
            await botClient.SendVideoNoteAsync(chatId, "КРУЖОК_ID", cancellationToken: cancellationToken);
            await botClient.SendTextMessageAsync(chatId, "Вот вам кружок!", cancellationToken: cancellationToken);
            break;
    }
}
```
Этот блок кода обрабатывает callback-запросы от пользователя. В зависимости от нажатой кнопки бот отправляет пользователю стикер, GIF, фото или видео-кружок.

####3.3.3. Ожидание команды для загрузки файла:
```csharp
if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
{
    if(update.Message.Text == "/add")
    {
        await botClient.SendTextMessageAsync(chatId, "Отправьте файл, который хотите.", cancellationToken: cancellationToken);
        isWaitFile = true;
        return;
    }
}
```
Здесь бот ожидает команду /add от пользователя. Как только команда получена, бот отправляет сообщение с запросом на загрузку файла.

####3.3.4. Обработка загруженных файлов:
```csharp
if (isWaitFile)
{
    if (update.Message.VideoNote != null)
    {
        var fileId = update.Message.VideoNote.FileId;
        Console.WriteLine($"Получено VideoNote с File ID: {fileId}");
        await botClient.SendTextMessageAsync(chatId, $"Ваш VideoNote file_id: {fileId}", cancellationToken: cancellationToken);
        isWaitFile = false;
        return;
    }
    if (update.Message.Photo != null)
    {
        var fileId = update.Message.Photo.Last().FileId;
        Console.WriteLine($"Получено Photo с File ID: {fileId}");
        await botClient.SendTextMessageAsync(chatId, $"Ваш Photo file_id: {fileId}", cancellationToken: cancellationToken);
        isWaitFile = false;
        return;
    }
    if (update.Message.Animation != null)
    {
        var fileId = update.Message.Animation.FileId;
        Console.WriteLine($"Получено Animation с File ID: {fileId}");
        await botClient.SendTextMessageAsync(chatId, $"Ваш Animation file_id: {fileId}", cancellationToken: cancellationToken);
        isWaitFile = false;
        return;
    }
    if (update.Message.Sticker != null)
    {
        var fileId = update.Message.Sticker.FileId;
        Console.WriteLine($"Получено Sticker с File ID: {fileId}");
        await botClient.SendTextMessageAsync(chatId, $"Ваш Sticker file_id: {fileId}", cancellationToken: cancellationToken);
        isWaitFile = false;
        return;
    }
}
```
Этот блок обрабатывает файлы, загруженные пользователем, и отправляет обратно file_id.

####3.3.5. Создание Inline-клавиатуры:

```csharp
var inlineKeyboard = new InlineKeyboardMarkup(new[]
{
    new[]
    {
        InlineKeyboardButton.WithCallbackData("Смешной стикер", "sticker_1"),
        InlineKeyboardButton.WithCallbackData("Смешной gif", "sticker_2")
    },
    new[]
    {
        InlineKeyboardButton.WithCallbackData("Смешная картинка", "sticker_3"),
        InlineKeyboardButton.WithCallbackData("Смешной кружок", "circle_1")
    }
});

await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Выберите стикер:",
    replyMarkup: inlineKeyboard,
    cancellationToken: cancellationToken
);
```
Этот блок создает Inline-клавиатуру с четырьмя кнопками для взаимодействия с пользователем.
