using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace ParseBotSolution
{
    partial class AvitoTelegramBot
    {

        public static readonly TelegramBotClient Bot = new TelegramBotClient("5919982097:AAHgK2XTaj_dbOtJsyIYbSrzp-JILGqE9f0");
        static void Start()
        {

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQuery;

            Bot.StartReceiving(Array.Empty<UpdateType>());

            var me = Bot.GetMeAsync();

            Console.WriteLine("{0} start receiving", me.Result);

            Console.ReadLine();
        }
        static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            Console.WriteLine("Thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("@{0} send {1}\n", messageEventArgs.Message.Chat.Username, messageEventArgs.Message.Text);

            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;

            if (message == null || message.Type != MessageType.Text)
                return;

            

        }
        static async void BotOnCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            
        }
        public static void SendAvitoItemAsync(AvitoItem item,ChatId chatId)
        {
            string message = string.Format(
                $"Name: <b>{item.name}</b>\n" +
                $"Price: {item.price}\n" +
                $"Description: {item.description}\n" +
                $"Link: {item.link}\n");

            Bot.SendTextMessageAsync(
                626774740, 
                message, 
                ParseMode.Html);
        }
    }



}
