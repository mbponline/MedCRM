using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MedCRM.Data;
using System.Text;

namespace MedCRM.Models
{
    public class Bot
    {
        public static TelegramBotClient client;

        static InlineKeyboardMarkup docListKeyboard { get; set; }
        static ReplyKeyboardMarkup initialKeyboard { get; set; }  = new ReplyKeyboardMarkup();

        static List<string> phoneList { get; set; } = new List<string>();
        static List<Patient> patientList { get; set; } = new List<Patient>();
        static List<Notification> notificationList { get; set; } = new List<Notification>();

        public Bot()
        {
            string token = "836564131:AAGvDqFDA1NYGjN3i_ltdYYrZk2Hjixy1fU";
            client = new TelegramBotClient(token);
            client.OnCallbackQuery += BotOnCallbackQuery;
            client.OnMessage += BotOnMessageReceived;
            client.StartReceiving();


            //врачи
            InlineKeyboardButton docBtn1 = new InlineKeyboardButton() { Text = "Доктор Айболит", CallbackData = "Доктор Айболит" };
            InlineKeyboardButton docBtn2 = new InlineKeyboardButton() { Text = "Доктор Дулитл", CallbackData = "Доктор Дулитл" };

            //клавиатура с врачами
            List<InlineKeyboardButton> docBtnList = new List<InlineKeyboardButton>();
            docBtnList.Add(docBtn1);
            docBtnList.Add(docBtn2);

            docListKeyboard = new InlineKeyboardMarkup(docBtnList);

            //кнопка с телефоном
            KeyboardButton telBtn = new KeyboardButton() { Text = "Отправить свой телефон", RequestContact = true };
        }


        private static async void BotOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var docChoice = e.CallbackQuery.Data;
           // await client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Вы прикреплены к " + docChoice);
            KeyboardButton emptyBtn = new KeyboardButton() { Text = "Узнать свое расписание" };
            var markuptest = new ReplyKeyboardMarkup(new KeyboardButton[] { emptyBtn }, resizeKeyboard: true);
            await client.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, messageId: e.CallbackQuery.Message.MessageId, text: "Вы прикреплены к " + docChoice);
            await client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Нажмите кнопку узнать расписание", replyMarkup: markuptest);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
         {
            var message = messageEventArgs.Message;
            KeyboardButton telBtn = new KeyboardButton() { Text = "Отправить свой телефон", RequestContact = true };
            var markup = new ReplyKeyboardMarkup(new KeyboardButton[] { telBtn }, resizeKeyboard: true, oneTimeKeyboard: true);

            var greetingMessage = "Добро пожаловать";
            //старт бота
            if (message.Text == "/start")
            {
                var patient = patientList.Where(x => x.telegramId == message.From.Id).FirstOrDefault();
                if (patient != null)
                {
                    greetingMessage = "Добро пожаловать";
                    await client.SendTextMessageAsync(message.Chat.Id, greetingMessage);
                }
                else
                {
                    greetingMessage = "Добро пожаловать. Ваш телефон не найден в списке. Нажмите кнопку отправить телефон";
                    await client.SendTextMessageAsync(message.Chat.Id, greetingMessage, replyMarkup: markup);
                }
            }

            if (message.Text == "Узнать свое расписание")
            {
                var patient = patientList.Where(x => x.telegramId == message.From.Id).FirstOrDefault();
                if (patient !=null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in patient.Notifications)
                    {
                        sb.Append(item + "\n");
                    }
                    
                }

            }
            //добавление телефона и прикрепление к врачу
            if (message.Type == MessageType.Contact)
            {
                var phoneNumber = message.Contact.PhoneNumber;
                if (!phoneList.Contains(phoneNumber))
                {
                    phoneList.Add(phoneNumber);
                    CreatePatient(phoneNumber, message.Contact);
                    await client.SendTextMessageAsync(message.Chat.Id, "Спасибо, Ваш телефон добавлен в базу данных");
                    await client.SendTextMessageAsync(message.Chat.Id, "Выберите врача", replyMarkup: docListKeyboard);
                }
                else
                {
                    KeyboardButton emptyBtn = new KeyboardButton() { Text = "Узнать свое расписание" };
                    var markuptest = new ReplyKeyboardMarkup(new KeyboardButton[] { emptyBtn }, resizeKeyboard: true);
                    await client.SendTextMessageAsync(message.Chat.Id, "Ваш телефон Уже есть в базе данных");
                    await client.SendTextMessageAsync(message.Chat.Id, "Нажмите кнопку узнать расписание", replyMarkup: markuptest);
                }
            }

        }

        private static void CreatePatient(string phoneNumber, Contact contact)
        {
            Patient patient = new Patient();
            patient.PhoneNumber = phoneNumber;
            patient.telegramId = contact.UserId;
            patientList.Add(patient);
        }


    }
}
