using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MedCRM.Data;
using System.Text;
using MedCRM.Models.Bot;
using Microsoft.EntityFrameworkCore;

namespace MedCRM.Models
{
    public class BotHandler
    {
        public static TelegramBotClient client;

        static KeyboardButton buttonGetSchedule { get; set; } = new KeyboardButton() { Text = "Узнать свое расписание" };
        static KeyboardButton buttonSendPhone { get; set; } = new KeyboardButton() { Text = "Отправить свой телефон", RequestContact = true };
        static ReplyKeyboardMarkup sendPhoneMarkup { get; set; } = new ReplyKeyboardMarkup(new KeyboardButton[] { buttonSendPhone }, resizeKeyboard: true, oneTimeKeyboard: true);
        static ReplyKeyboardMarkup getScheduleMarkup { get; set; } = new ReplyKeyboardMarkup(new KeyboardButton[] { buttonGetSchedule }, resizeKeyboard: true, oneTimeKeyboard: true);


        static List<string> phoneList { get; set; } = new List<string>();
        static List<Patient> patientList { get; set; } = new List<Patient>();
        static List<Notification> notificationList { get; set; } = new List<Notification>();

        public BotHandler()
        {
            string token = BotConfig.Token;
            client = new TelegramBotClient(token);
            client.OnMessage += BotOnMessageReceived;
            client.StartReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            //старт бота
            if (message.Text == "/start")
            {
                var patient = patientList.Where(x => x.TelegramId == message.From.Id).FirstOrDefault();
                if (patient != null)
                {
                    var messageText = "Добро пожаловать";
                    await client.SendTextMessageAsync(message.Chat.Id, messageText);
                }
                else
                {
                    var messageText = "Добро пожаловать. Ваш телефон не найден в списке. Нажмите кнопку отправить телефон";
                    await client.SendTextMessageAsync(message.Chat.Id, messageText, replyMarkup: sendPhoneMarkup);
                }
            }
            //узнать расписание
            if (message.Text == "Узнать свое расписание")
            {
                var patient = patientList.Where(x => x.TelegramId == message.From.Id).FirstOrDefault();
                if (patient != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in patient.Notifications)
                    {
                        sb.Append(item + "\n");
                    }

                }
            }
            //добавление телефона и создание обьект Patient
            if (message.Type == MessageType.Contact)
            {
                var phoneNumber = message.Contact.PhoneNumber;
                if (!phoneList.Contains(phoneNumber))
                {
                    phoneList.Add(phoneNumber);
                    CreatePatient(phoneNumber, message.Contact);
                    await client.SendTextMessageAsync(message.Chat.Id, "Спасибо, Ваш телефон добавлен в базу данных");
                    await client.SendTextMessageAsync(message.Chat.Id, "Нажмите кнопку узнать расписание", replyMarkup: getScheduleMarkup);
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Ваш телефон Уже есть в базе данных");
                    await client.SendTextMessageAsync(message.Chat.Id, "Нажмите кнопку узнать расписание", replyMarkup: getScheduleMarkup);
                }
            }
        }

        private static void CreatePatient(string phoneNumber, Contact contact)
        {
            Patient patient = new Patient();
            patient.PhoneNumber = phoneNumber;
            patient.TelegramId = contact.UserId;
            patientList.Add(patient);

            //подключение к БД
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-MedCRM-D4E7B1AB-34B5-4B96-97E3-8132DB18ACDC;Trusted_Connection=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                var a = context.Patients.FirstOrDefault(x=> x.PhoneNumber== patient.PhoneNumber);
            }
        }


        //private async void NotificationSender()
        //{
        //    //уведомления только сегодня
        //    var notificationsToday = notificationList.Where(x => x.SendTime.Date == DateTime.Now.Date);

        //    foreach (var notification in notificationsToday)
        //    {
        //        int chatId = patientList.Where(x => x.Id == notification.PatientId).FirstOrDefault().telegramId;
        //        await client.SendTextMessageAsync(chatId, notification.NotificationText);
        //    }
        //}


        //private async void NotificationSender()
        //{
        //    foreach (var patient in patientList)
        //    {
        //        await client.SendTextMessageAsync(patient.TelegramId, "It is " + DateTime.Now);
        //    }
        //}
    }
}
