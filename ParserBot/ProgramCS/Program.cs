using HtmlAgilityPack;
using System.Net.Http;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ParseBotSolution
{
    partial class Program
    {
        static Program()
        {
            Links = StartProcedures.GetLinksFromDBtoParse(Links, dataBase);
        }
        static DataBase dataBase = new DataBase();
        static Dictionary<long, CancellationTokenSource> cancellationTokenSources = new Dictionary<long, CancellationTokenSource>();
        static Dictionary<long, string> Links = new Dictionary<long, string>();
        static Dictionary<long, Task> activeTasksParse = new Dictionary<long, Task>();
        static Dictionary<long, MainParseFunction> mainParseFunctionObjects = new Dictionary<long, MainParseFunction>();
        static Dictionary<long, string> newLinks = new Dictionary<long, string>();
        static Dictionary<long, string> linksToAdd = new Dictionary<long, string>();
        static Dictionary<long, string> linksToDelete = new Dictionary<long, string>() ;
        static ProxyController proxyController = new ProxyController();
       //static List<Proxy> proxiesList = proxyController.GetProxiesToList();

        static async Task Main(string[] args)
        {
            
            proxyController.WritelineProxy();
            FirstStartParsing();


            Thread newThread = new Thread(async () =>
            {
                while (true)
                {
                    Thread.Sleep(10000);
                    Console.WriteLine("Проерка новых ссылок");

                    newLinks = StartProcedures.GetLinksFromDBtoParse(newLinks,dataBase);
                    CheckNewLinks(newLinks);
                    linksToDelete = CheckNoActualLinks(Links, newLinks,linksToDelete);
                    fillLinksNewLinks(Links, newLinks);


                    DeleteNotActualLinks(linksToDelete);
                    AddLinksToParse(linksToAdd);
                    
                }
            });
            newThread.Start();
           
        }

       
    }
}   
