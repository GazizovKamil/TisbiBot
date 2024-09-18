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
