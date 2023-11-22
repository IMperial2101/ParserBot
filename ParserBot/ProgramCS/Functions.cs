using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace ParseBotSolution
{
    partial class Program
    {
       public static void FirstStartParsing()
        {
            foreach (var link in Links)
            {
                StartNewParsing(link);
            }
        }
        public static void AddLinksToParse(Dictionary<long, string> linksToAdd)
        {
            foreach (var link in linksToAdd)
            {
                StartNewParsing(link);
                Console.WriteLine($"Запущен поиск по ссылке {link.Value}");
            }
        }
        public static async void DeleteNotActualLinks(Dictionary<long, string> linksToDelete)
        {
            foreach (var link in linksToDelete)
            {
                if (activeTasksParse.ContainsKey(link.Key))
                {
                    Task task = activeTasksParse[link.Key];
                    activeTasksParse.Remove(link.Key);

                    mainParseFunctionObjects.Remove(link.Key);

                    CancellationTokenSource cancellationTokenSource = cancellationTokenSources[link.Key];

                    cancellationTokenSource.Cancel();
                    cancellationTokenSources.Remove(link.Key);

                    Console.WriteLine($"Останавливается поиск для чата {link.Key}");
                    await task;
                    task.Dispose();
                    Console.WriteLine("Поиск остановлен");
                }
            }
        }
        public static void CheckNewLinks( Dictionary<long, string> newLinks)
        {
            linksToAdd.Clear();

            foreach (var pair in newLinks){
                if (!Links.ContainsKey(pair.Key))
                    linksToAdd.Add(pair.Key, pair.Value);         
            }
            CheckChangeLinks(newLinks);
                
        }
        private static void CheckChangeLinks(Dictionary<long, string> newLinks)
        {
            foreach (var pair in newLinks)
                if (Links.ContainsKey(pair.Key))
                {
                    var curr = Links[pair.Key];
                    if (pair.Value != curr)
                    {
                        mainParseFunctionObjects[pair.Key].urlToParse = pair.Value;
                        Console.WriteLine($"Изменение ссылка для чата {pair.Key}");
                    }
                }
        }
        public static Dictionary<long, string> CheckNoActualLinks(Dictionary<long, string> oldLinks, Dictionary<long, string> newLinks, Dictionary<long, string> notActualLinks)
        {
            notActualLinks.Clear();

            foreach (var pair in oldLinks)
            {
                if (!newLinks.ContainsKey(pair.Key))
                {
                    notActualLinks.Add(pair.Key, pair.Value);
                }
            }

            return notActualLinks;
        }
        private static void fillLinksNewLinks(Dictionary<long, string> links, Dictionary<long, string> newLinks)
        {
            links.Clear();
            foreach (var link in newLinks)
                links.Add(link.Key, link.Value);
        }
        static void StartNewParsing(KeyValuePair<long,string> link)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            mainParseFunctionObjects.Add(link.Key, new MainParseFunction(proxyController.GetNoActiveProxy(), link.Value, link.Key, cancellationToken));

            Task task = Task.Run(() => mainParseFunctionObjects[link.Key].CheckNewItemsHttpClientAsync());

            activeTasksParse.Add(link.Key, task);

            cancellationTokenSources.Add(link.Key, cancellationTokenSource);
        }
        
    }
}
