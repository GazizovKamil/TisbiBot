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
- Активный Telegram-бот и его токен (можно получить через [BotFather](https://t.me/BotFather))
- Библиотека `Telegram.Bot` (установите через NuGet)

## Настройка проекта

### Шаг 1: Клонирование репозитория

Склонируйте этот репозиторий с помощью следующей команды:

```bash
git clone https://github.com/ваш_репозиторий
cd ваш_репозиторий
