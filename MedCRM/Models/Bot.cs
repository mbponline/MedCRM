using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedCRM.Models
{
    public class Bot
    {
        public static TelegramBotClient client;
        static InlineKeyboardMarkup docListKeyboard;
        static ReplyKeyboardMarkup initialKeyboard = new ReplyKeyboardMarkup();
        static List<string> phoneList { get; set; } = new List<string>();


        public Bot()
        {
            string token = "836564131:AAGvDqFDA1NYGjN3i_ltdYYrZk2Hjixy1fU";
            client = new TelegramBotClient(token);
            client.OnCallbackQuery += BotOnCallbackQuery;
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
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
            var info = e.CallbackQuery.Data;
            await client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Вы прикреплены к "+ info, replyMarkup: docListKeyboard);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            KeyboardButton telBtn = new KeyboardButton() { Text = "Отправить свой телефон", RequestContact = true };

            var markup = new ReplyKeyboardMarkup(new KeyboardButton[] { telBtn }, resizeKeyboard: true, oneTimeKeyboard: true);

            var greetingMessage = "Добро пожаловать";
            if (message.Text == "/start")
            {
                if (message.Contact != null)
                {
                    if (string.IsNullOrEmpty(message.Contact.PhoneNumber))
                    {
                        if (phoneList.Contains(message.Contact.PhoneNumber))
                            greetingMessage = "Добро пожаловать";
                        else
                            greetingMessage = "Добро пожаловать. Ваш телефон не найден в списке. Нажмите кнопку отправить телефон";

                    }
                }
                await client.SendTextMessageAsync(message.Chat.Id, greetingMessage, replyMarkup: markup);
            }

            if (message.Text == "/show")
            {
                await client.SendTextMessageAsync(message.Chat.Id, greetingMessage, replyMarkup: docListKeyboard);
            }

            //добавление телефона
            if (message.Type == MessageType.Contact)
            {
                var phoneNumber = message.Contact.PhoneNumber;
                if (!phoneList.Contains(phoneNumber))
                {
                    phoneList.Add(phoneNumber);
                    await client.SendTextMessageAsync(message.Chat.Id, "Спасибо, Ваш телефон добавлен в базу данных");
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Ваш телефон Уже есть в базе данных");
                }
            }

        }
    }
}
