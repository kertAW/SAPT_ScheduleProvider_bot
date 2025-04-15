using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using SAPT_Bot.Routes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SAPT_Bot
{
    public class Program
    {
        private static ScheduleRoute scheduleRoute;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        public static TelegramBotClient Bot { get; set; }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        static void Main(string[] args)
        {
            using var cts = new CancellationTokenSource();
            var bot = new TelegramBotClient("7970806482:AAHG-K-uUONSXCGmw7lvdFsOyqwFuu3lkXs", cancellationToken: cts.Token);

            Bot = bot;

            var me = bot.GetMe();
            me.Wait();

            bot.OnMessage += OnMessage;
            bot.OnUpdate += Bot_OnUpdate;

            scheduleRoute = new ScheduleRoute(bot);

            Console.WriteLine($"@{me.Result.Username} is running... Press Enter to terminate.");
            Console.ReadLine();
            cts.Cancel();
        }

        private async static Task Bot_OnUpdate(Update update)
        {
            // Если inline keyboard button - то update.CallbackQuery
            if (update.CallbackQuery != null)
            {
                Console.WriteLine($"Received id=\"{update.CallbackQuery.Id}\" with data \"{update.CallbackQuery.Data}\"");
                await Bot.AnswerCallbackQuery(update.CallbackQuery.Id);

                switch (update.CallbackQuery.Data)
                {
                    case "menuButton_scheduleMailing":
                        await scheduleRoute.GoToScheduleMailing(update.CallbackQuery.Message);
                        break;
                    case "menuButton_guide":
                        break;
                    case "menuButton_reportProblem":
                        break;
                    case "menuButton_authors":
                        break;
                    case "schedule_fullSchedule":
                        await scheduleRoute.GetFullScheduleChoice(update.CallbackQuery.Message);
                        break;
                    case "schedule_todayFullSchedule":
                        await scheduleRoute.GetFullSchedule(update.CallbackQuery.Message, FullScheduleMode.Today);
                        break;
                    case "schedule_tomorrowFullSchedule":
                        await scheduleRoute.GetFullSchedule(update.CallbackQuery.Message, FullScheduleMode.Tomorrow);
                        break;
                    case "schedule_changeGroup":
                        break;
                }
            }
        }

        private async static Task OnMessage(Message message, UpdateType type)
        {
            if (message.Text is null) return;

            Console.WriteLine($"Received {type} '{message.Text}' in {message.Text}");
            await Bot.SendMessage(message.Chat, $"{message.From} said: {message.Text}");
            await SendMenu(message, type);
        }

        private async static Task SendMenu(Message message, UpdateType type)
        {
            var menuMessage = await Bot.SendMessage(message.Chat.Id, "Это меню!", 
                ParseMode.Html, 
                protectContent: true,
                replyMarkup: new InlineKeyboardButton[][]
                {
                    [("Рассылка расписания", "menuButton_scheduleMailing")],
                    [("Справочник", "menuButton_guide")], 
                    [("Сообщить о неполадках", "menuButton_reportProblem"), ("Авторы", "menuButton_authors")],
                });
        }
    }
}
