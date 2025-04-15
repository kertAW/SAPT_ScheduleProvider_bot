using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace SAPT_Bot.Routes
{
    public class ScheduleRoute
    {
        private TelegramBotClient bot;

        public ScheduleRoute(TelegramBotClient bot)
        {
            this.bot = bot;
        }

        public async Task GoToScheduleMailing(Message message)
        {
            await bot.SendMessage(message.Chat.Id, "Страница расписаний",
                replyMarkup: new InlineKeyboardButton[][]
                {
                    [("Получить полное расписание", "schedule_fullSchedule")],
                    [("Выбрать группу", "schedule_changeGroup")]
                });
        }

        public async Task GetFullScheduleChoice(Message message)
        {
            await bot.SendMessage(message.Chat.Id, "Выберите день, по которому хотите получить расписание",
                replyMarkup: new InlineKeyboardButton[][]
                {
                    [("Сегодня", "schedule_todayFullSchedule"), ("Завтра", "schedule_tomorrowFullSchedule")]
                });
        }

        public async Task GetFullSchedule(Message message, FullScheduleMode Mode)
        {
            using (var http = new HttpClient())
            {
                string date = "";

                if (Mode == FullScheduleMode.Today)
                {
                    date = DateTime.Now.ToString("dd.MM");
                }
                else
                {
                    date = DateTime.Now.AddDays(1).ToString("dd.MM");
                }

                try
                {
                    await bot.SendDocument(message.Chat.Id, $@"https://gbpousapt.ru/public/РАСПИСАНИЕ/{date}.pdf", "Мегахуйня");
                }
                catch
                {
                    await bot.SendMessage(message.Chat.Id, "На данный день ещё не выложено расписание, либо произошла ошибка");
                }
            }
        }
    }

    public enum FullScheduleMode
    {
        Today,
        Tomorrow
    }
}
