using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botToken = "8326388876:AAHPImcwNgv460YQC87HO3XU8alL1OPCsKg"; 

var botClient = new TelegramBotClient(botToken);

using var cts = new CancellationTokenSource();

botClient.StartReceiving(
    async (bot, update, token) =>
    {
        if (update.Message?.Text is { } text)
        {
            var chatId = update.Message.Chat.Id;

            switch (text)
            {
                case "/start":
                    await ShowMainMenu(bot, chatId);
                    break;

                case "/help":
                    await ShowHelp(bot, chatId);
                    break;

                case "📋 Мероприятия":
                    await ShowEvents(bot, chatId);
                    break;

                case "✅ Мои записи":
                    await ShowMyEvents(bot, chatId);
                    break;

                case "❓ Помощь":
                    await ShowHelp(bot, chatId);
                    break;

                case "👨‍💻 О боте":
                    await ShowAbout(bot, chatId);
                    break;

                case "🔙 Назад в меню":
                    await ShowMainMenu(bot, chatId);
                    break;

                default:
                    await bot.SendTextMessageAsync(
                        chatId,
                        "❌ Я не понимаю эту команду.\nПожалуйста, используйте кнопки меню.",
                        replyMarkup: GetMainKeyboard());
                    break;
            }
        }
        else if (update.CallbackQuery != null)
        {
            var chatId = update.CallbackQuery.Message!.Chat.Id;
            var messageId = update.CallbackQuery.Message.MessageId;
            var data = update.CallbackQuery.Data;

            if (data.StartsWith("register_"))
            {
                var eventId = data.Split('_')[1];
                await bot.SendTextMessageAsync(
                    chatId,
                    $"✅ Вы успешно зарегистрированы на мероприятие!\n\n" +
                    $"📌 Номер регистрации: #{eventId}-{DateTime.Now:ddMMyy}-{chatId.ToString().Substring(0, 5)}\n" +
                    $"📆 Дата регистрации: {DateTime.Now:dd.MM.yyyy HH:mm}\n\n" +
                    $"Напоминание придет за 24 часа до начала.",
                    replyMarkup: GetBackKeyboard());
            }
            else if (data.StartsWith("cancel_"))
            {
                await bot.SendTextMessageAsync(
                    chatId,
                    "✅ Регистрация успешно отменена.\n\n" +
                    "Будем ждать вас на других мероприятиях!",
                    replyMarkup: GetBackKeyboard());
            }
            else if (data.StartsWith("details_"))
            {
                var eventId = data.Split('_')[1];
                await ShowEventDetails(bot, chatId, eventId);
            }

            await bot.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    },
    (bot, ex, token) =>
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
        return Task.CompletedTask;
    },
    new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } },
    cts.Token
);

Console.WriteLine("🎓 Бот мероприятий Академии Топ запущен!");
Console.WriteLine($"Бот: @{(await botClient.GetMeAsync()).Username}");
Console.WriteLine("Нажмите Enter для остановки...");
Console.ReadLine();
cts.Cancel();

// ==================== МЕНЮ И НАВИГАЦИЯ ====================

static async Task ShowMainMenu(ITelegramBotClient bot, long chatId)
{
    var message = "🎓 *Академия Топ* — официальный бот мероприятий колледжа\n\n" +
                  "👋 Здравствуйте! Я помогу вам:\n" +
                  "• Узнать о предстоящих событиях\n" +
                  "• Зарегистрироваться на мероприятия\n" +
                  "• Отслеживать ваши записи\n" +
                  "• Получать важные уведомления\n\n" +
                  "📌 *Выберите раздел:*";

    await bot.SendTextMessageAsync(
        chatId,
        message,
        parseMode: ParseMode.Markdown,
        replyMarkup: GetMainKeyboard());
}

// ==================== МЕРОПРИЯТИЯ ====================

static async Task ShowEvents(ITelegramBotClient bot, long chatId)
{
    var message = "📋 *Актуальные мероприятия*\n" +
                  "━━━━━━━━━━━━━━━━━━━━━━\n\n" +

                  "1️⃣ *IT-лекция: Введение в C#*\n" +
                  "👨‍🏫 Лектор: Иванов А.А.\n" +
                  "📆 25 декабря 2024, 18:00\n" +
                  "📍 Академия Топ, ауд. 301\n" +
                  "⏳ Длительность: 2 часа\n" +
                  "👥 Свободно: 15 из 20 мест\n\n" +

                  "2️⃣ *Мастер-класс по Telegram Bot API*\n" +
                  "👨‍🏫 Лектор: Петров В.Б.\n" +
                  "📆 28 декабря 2024, 16:00\n" +
                  "📍 Академия Топ, ауд. 105\n" +
                  "⏳ Длительность: 3 часа\n" +
                  "👥 Свободно: 8 из 15 мест\n\n" +

                  "3️⃣ *Карьерный день в IT*\n" +
                  "🤝 Участвуют: Yandex, Mail.ru, Tinkoff\n" +
                  "📆 30 декабря 2024, 14:00\n" +
                  "📍 Академия Топ, конференц-зал\n" +
                  "⏳ Длительность: 4 часа\n" +
                  "👥 Свободно: 25 из 50 мест\n\n" +

                  "4️⃣ *Хакатон: Новогодний кодинг*\n" +
                  "🏆 Призовой фонд: 50 000 руб.\n" +
                  "📆 5 января 2025, 10:00\n" +
                  "📍 Академия Топ, коворкинг\n" +
                  "⏳ Длительность: 24 часа\n" +
                  "👥 Свободно: 12 из 30 мест\n\n" +

                  "━━━━━━━━━━━━━━━━━━━━━━\n" +
                  "⬇️ *Нажмите на кнопку для записи:*";

    var keyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("🔹 Записаться: C# лекция", "register_1") },
        new[] { InlineKeyboardButton.WithCallbackData("🔹 Записаться: Telegram Bot API", "register_2") },
        new[] { InlineKeyboardButton.WithCallbackData("🔹 Записаться: Карьерный день", "register_3") },
        new[] { InlineKeyboardButton.WithCallbackData("🔹 Записаться: Хакатон", "register_4") },
        new[] { InlineKeyboardButton.WithCallbackData("📊 Подробнее о мероприятии", "details_all") }
    });

    await bot.SendTextMessageAsync(
        chatId,
        message,
        parseMode: ParseMode.Markdown,
        replyMarkup: keyboard);
}

static async Task ShowEventDetails(ITelegramBotClient bot, long chatId, string eventId)
{
    var message = eventId switch
    {
        "1" => "📌 *Детальная информация*\n\n" +
               "*IT-лекция: Введение в C#*\n\n" +
               "👨‍🏫 *Спикер:* Иванов Андрей Александрович\n" +
               "   Senior Developer в компании Yandex\n" +
               "   Опыт: 8 лет\n\n" +
               "📋 *Программа:*\n" +
               "• Основы синтаксиса C#\n" +
               "• ООП на практике\n" +
               "• Работа с .NET 8\n" +
               "• Живой кодинг\n\n" +
               "📚 *Необходимо знать:*\n" +
               "• Базовые основы программирования\n\n" +
               "🎁 *Бонус:*\n" +
               "• Все участники получат презентацию и исходный код",

        "2" => "📌 *Детальная информация*\n\n" +
               "*Мастер-класс по Telegram Bot API*\n\n" +
               "👨‍🏫 *Спикер:* Петров Владимир Борисович\n" +
               "   Team Lead в Telegram-студии\n" +
               "   Автор 5 успешных ботов\n\n" +
               "📋 *Программа:*\n" +
               "• Архитектура Telegram ботов\n" +
               "• Webhook vs Long Polling\n" +
               "• Работа с клавиатурами\n" +
               "• Деплой на сервер\n\n" +
               "💻 *Формат:*\n" +
               "• Теоретическая часть + практика\n" +
               "• Каждый создаст своего бота",

        "3" => "📌 *Детальная информация*\n\n" +
               "*Карьерный день в IT*\n\n" +
               "🏢 *Компании-участники:*\n" +
               "• Яндекс — стажировки и вакансии\n" +
               "• Mail.ru Group — карьерные программы\n" +
               "• Тинькофф — IT-стажировки\n\n" +
               "📋 *В программе:*\n" +
               "• Презентации компаний\n" +
               "• Мастер-классы по собеседованиям\n" +
               "• Нетворкинг с HR\n" +
               "• Розыгрыш мерча\n\n" +
               "🎫 *Вход свободный, необходима регистрация*",

        "4" => "📌 *Детальная информация*\n\n" +
               "*Хакатон: Новогодний кодинг*\n\n" +
               "🏆 *Призовой фонд:* 50 000 руб.\n" +
               "🥇 1 место — 30 000 руб.\n" +
               "🥈 2 место — 15 000 руб.\n" +
               "🥉 3 место — 5 000 руб.\n\n" +
               "📋 *Тема:* Разработка новогодних IT-решений\n" +
               "💻 Формат: Командный (2-4 человека)\n" +
               "⏳ Время: 24 часа\n\n" +
               "🍕 *Организаторы обеспечивают:*\n" +
               "• Питание и напитки\n" +
               "• Рабочие места\n" +
               "• Wi-Fi\n" +
               "• Наставников",

        _ => "📊 *Все мероприятия*\n\n" +
              "В декабре-январе запланировано 4 мероприятия.\n" +
              "Средняя посещаемость: 75%\n" +
              "Рейтинг мероприятий: 4.8/5\n\n" +
              "Следующее мероприятие через 3 дня!"
    };

    var keyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("✅ Записаться", $"register_{eventId}") },
        new[] { InlineKeyboardButton.WithCallbackData("🔙 К списку мероприятий", "back_to_events") }
    });

    await bot.SendTextMessageAsync(
        chatId,
        message,
        parseMode: ParseMode.Markdown,
        replyMarkup: keyboard);
}

// ==================== МОИ МЕРОПРИЯТИЯ ====================

static async Task ShowMyEvents(ITelegramBotClient bot, long chatId)
{
    var message = "✅ *Ваши активные регистрации*\n" +
                  "━━━━━━━━━━━━━━━━━━━━━━\n\n" +

                  "📌 *IT-лекция: Введение в C#*\n" +
                  "📆 25 декабря 2024, 18:00\n" +
                  "📍 Аудитория 301\n" +
                  "🎫 Билет: #REG-2024-12-25-001\n" +
                  "✅ Статус: Подтверждено\n" +
                  "⏰ Напоминание: за 24 часа\n\n" +

                  "📌 *Мастер-класс по Telegram Bot API*\n" +
                  "📆 28 декабря 2024, 16:00\n" +
                  "📍 Аудитория 105\n" +
                  "🎫 Билет: #REG-2024-12-28-089\n" +
                  "✅ Статус: Подтверждено\n" +
                  "⏰ Напоминание: за 24 часа\n\n" +

                  "━━━━━━━━━━━━━━━━━━━━━━\n" +
                  "📊 Всего мероприятий: 2\n" +
                  "🎟 Следующее: через 3 дня\n\n" +

                  "⬇️ *Управление записями:*";

    var keyboard = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("❌ Отменить C# лекцию", "cancel_1"),
            InlineKeyboardButton.WithCallbackData("❌ Отменить мастер-класс", "cancel_2")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("📅 Добавить в календарь", "add_to_calendar")
        }
    });

    await bot.SendTextMessageAsync(
        chatId,
        message,
        parseMode: ParseMode.Markdown,
        replyMarkup: keyboard);
}

// ==================== ИНФОРМАЦИЯ ====================

static async Task ShowHelp(ITelegramBotClient bot, long chatId)
{
    var message = "❓ *Помощь и поддержка*\n" +
                  "━━━━━━━━━━━━━━━━━━━━━━\n\n" +

                  "📌 *Основные команды:*\n\n" +
                  "• /start — Главное меню\n" +
                  "• /events — Все мероприятия\n" +
                  "• /my — Мои записи\n" +
                  "• /cancel — Отмена регистрации\n" +
                  "• /help — Помощь\n\n" +

                  "📌 *Как записаться на мероприятие?*\n" +
                  "1. Нажмите «Мероприятия»\n" +
                  "2. Выберите интересующее событие\n" +
                  "3. Нажмите «Записаться»\n" +
                  "4. Подтвердите регистрацию\n\n" +

                  "📌 *Как отменить запись?*\n" +
                  "1. Перейдите в «Мои записи»\n" +
                  "2. Выберите мероприятие\n" +
                  "3. Нажмите «Отменить»\n\n" +

                  "📌 *Уведомления:*\n" +
                  "• Напоминание за 24 часа\n" +
                  "• Напоминание за 1 час\n" +
                  "• Информация об изменениях\n\n" +

                  "📞 *Техническая поддержка:*\n" +
                  "• @academytop_support\n" +
                  "• support@academytop.ru\n" +
                  "• +7 (495) 123-45-67\n\n" +

                  "⏰ *Часы работы:*\n" +
                  "Пн-Пт: 9:00 - 20:00\n" +
                  "Сб: 10:00 - 18:00\n" +
                  "Вс: Выходной";

    await bot.SendTextMessageAsync(
        chatId,
        message,
        parseMode: ParseMode.Markdown,
        replyMarkup: GetBackKeyboard());
}

static async Task ShowAbout(ITelegramBotClient bot, long chatId)
{
    var message = "🏫 *Официальный бот Академии Топ*\n" +
                  "━━━━━━━━━━━━━━━━━━━━━━\n\n" +

                  "🎓 *Академия Топ* — ведущий IT-колледж,\n" +
                  "готовящий специалистов в сфере\n" +
                  "информационных технологий с 2010 года.\n\n" +

                  "📊 *Статистика колледжа:*\n" +
                  "• 5000+ студентов\n" +
                  "• 200+ преподавателей\n" +
                  "• 50+ IT-компаний-партнеров\n" +
                  "• 90% трудоустройство выпускников\n\n" +

                  "🤖 *О боте:*\n" +
                  "Версия: 2.1.0\n" +
                  "Разработчик: IT-отдел Академии Топ\n" +
                  "Активных пользователей: 1,234\n" +
                  "Проведено мероприятий: 89\n" +
                  "Выдано билетов: 3,456\n\n" +

                  "📱 *Наши ресурсы:*\n" +
                  "• Сайт: academytop.ru\n" +
                  "• Telegram: @academytop\n" +
                  "• VK: vk.com/academytop\n" +
                  "• YouTube: /academytop\n\n" +

                  "📍 *Адрес:*\n" +
                  "г. Москва, ул. Программистов, д. 1\n" +
                  "м. Технопарк\n\n" +

                  "© 2024 Академия Топ. Все права защищены.";

    await bot.SendTextMessageAsync(
        chatId,
        message,
        parseMode: ParseMode.Markdown,
        replyMarkup: GetBackKeyboard());
}

// ==================== КЛАВИАТУРЫ ====================

static IReplyMarkup GetMainKeyboard()
{
    return new ReplyKeyboardMarkup(new[]
    {
        new[] { new KeyboardButton("📋 Мероприятия"), new KeyboardButton("✅ Мои записи") },
        new[] { new KeyboardButton("❓ Помощь"), new KeyboardButton("👨‍💻 О боте") }
    })
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false,
        Keyboard = new[]
        {
            new[]
            {
                new KeyboardButton("📋 Мероприятия"),
                new KeyboardButton("✅ Мои записи")
            },
            new[]
            {
                new KeyboardButton("❓ Помощь"),
                new KeyboardButton("👨‍💻 О боте")
            }
        }
    };
}

static IReplyMarkup GetBackKeyboard()
{
    return new ReplyKeyboardMarkup(new[]
    {
        new[] { new KeyboardButton("🔙 Назад в меню") }
    })
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = true
    };
}