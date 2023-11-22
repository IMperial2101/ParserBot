using HtmlAgilityPack;
using Telegram.Bot.Types;

namespace ParseBotSolution
{
    internal class MainParseFunction 
    {

        private CancellationToken cancellationToken;
        public bool disposed = false;
        private HtmlDocument doc = new HtmlDocument();
        private HttpClient httpClient;
        private Proxy proxy;
        private ChatId chatId;
        HtmlNodeCollection itemsAllInfo;
        string firstItemLink = "";
        List<AvitoItem> avitoItemsToSend = new List<AvitoItem>();
        public string urlToParse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_proxy">12314325</param>
        /// <param name="_urlToParse"></param>
        /// <param name="_chatId"></param>
        /// <param name="_cancellationToken"></param>
        public MainParseFunction(Proxy _proxy,string _urlToParse,ChatId _chatId,CancellationToken _cancellationToken)
        {
            urlToParse = _urlToParse;
            chatId = _chatId;
            proxy = _proxy;
            httpClient = MakeHttpClient();
            cancellationToken = _cancellationToken;
        }
        public async Task CheckNewItemsHttpClientAsync()
         {
            Console.WriteLine("Проверка {0}", urlToParse);          
            
            await downloadPageAsync();
            itemsAllInfo = GetItemsAllInfoFromDoc();
            firstItemLink = GetFirstItemLinkFromAllItems(itemsAllInfo);
            Console.WriteLine($"First item link: {firstItemLink}");

        Next_iteration:
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    proxy.ActiveUse = false;
                    break;
                }
                Task.Delay(new Random().Next(60000, 65000)).Wait();
                await downloadPageAsync();

                itemsAllInfo = GetItemsAllInfoFromDoc();

                if (ChekCurrAvitoItem(firstItemLink, itemsAllInfo[0]))
                    continue;

               
                avitoItemsToSend.Clear();
                foreach (var item in itemsAllInfo)
                {
                    AvitoItem currAvitoItem = new AvitoItem();

                    MakeAvitoItem(currAvitoItem, item);
                    if (currAvitoItem.link == "https://www.avito.ru" + firstItemLink)
                    {
                        firstItemLink = GetFirstItemLinkFromAllItems(itemsAllInfo);

                        SendAvitoItemsToUserAsync(avitoItemsToSend);
                        
                        goto Next_iteration;
                    }
                    avitoItemsToSend.Add(currAvitoItem);
                    
                }
                firstItemLink = GetFirstItemLinkFromAllItems(itemsAllInfo);

            }

        }

        HttpClient MakeHttpClient()
        {
            HttpClientHandler httpHandler = new HttpClientHandler();
            ProxyController.AddProxyToHttpClientHandler(httpHandler, proxy);

            HttpClient httpClient = new HttpClient(httpHandler);

            httpClient.MaxResponseContentBufferSize = int.MaxValue;
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36");
            return httpClient;

        }
        private async Task downloadPageAsync()
        {
            Console.WriteLine("downloadPageAsync...");
            HttpResponseMessage response;
            response = await httpClient.GetAsync(urlToParse);
            Console.WriteLine("Ответ получен");
            if (!response.IsSuccessStatusCode)
                return;

            doc.LoadHtml(await response.Content.ReadAsStringAsync());
            Console.WriteLine("Page download");
        }
        private string SharpDescription(string description)
        {
            if (description.Length > 200)
                return description.Substring(0,200)+"...";
            return description;
        }
        private void MakeAvitoItem(AvitoItem currAvitoItem, HtmlNode item)
        {
            HtmlNode itemSeller = item.SelectSingleNode(".//div[contains(@class,'iva-item-aside')]");
            HtmlNode sellerRatingDiv = itemSeller.SelectSingleNode(".//div[contains(@data-marker, 'seller-rating')]");
            HtmlNode itemBody = item.SelectSingleNode(".//div[contains(@class,'iva-item-body')]");
            try{
                currAvitoItem.name = itemBody.SelectSingleNode(".//div[contains(@class ,'iva-item-titleStep')]").InnerText;
                currAvitoItem.price = itemBody.SelectSingleNode(".//div[contains(@class ,'iva-item-priceStep')]").InnerText;
                currAvitoItem.param = itemBody.SelectSingleNode(".//div[contains(@class ,'iva-item-autoParamsStep')]")?.InnerText;
                currAvitoItem.description = SharpDescription(itemBody?.SelectSingleNode(".//div[contains(@class ,'iva-item-descriptionStep')]").InnerText);
                currAvitoItem.link = "https://www.avito.ru" + itemBody.SelectSingleNode(".//a").Attributes["href"].Value;
                currAvitoItem.photoLink = item.SelectSingleNode(".//div[contains(@class,'photo-slider-item')]//img")?.Attributes["src"].Value;
                currAvitoItem.avitoItemSeller.rating = sellerRatingDiv?.SelectSingleNode(".//div[contains(@class, 'SellerRating')]//span[contains(@class, 'desktop')]").InnerText;
                currAvitoItem.avitoItemSeller.ratingCount = sellerRatingDiv?.SelectSingleNode(".//span[contains(@data-marker, \"seller-rating/summary\")]").InnerText;
                currAvitoItem.avitoItemSeller.name = itemSeller.SelectSingleNode(".//div[contains(@class,'style-title')]")?.InnerText;
            }
            catch (Exception ex){
                Console.WriteLine("Exception variable: {0}", ex.TargetSite.Name);
            }

        }
        private bool ChekCurrAvitoItem(string firstItemLink, HtmlNode itemBody)
        {
            string link = itemBody.SelectSingleNode(".//a").Attributes["href"].Value;
            Console.WriteLine($"Link to check: {link}");
            if (firstItemLink == link)
                return true;
            return false;
        }
        private void WriteLineAvitoItemDescription(AvitoItem currAvitoItem)
        {
            Console.WriteLine($"_________________________________\n" +
                $"Имя: {currAvitoItem.name}\n" +
                $"Цена: {currAvitoItem.price}\n" +
                $"Параметры: {currAvitoItem.price}\n" +
                $"Описание: {currAvitoItem.description}\n" +
                $"Ссылка: {currAvitoItem.link}\n" +
                $"Продавец\n" +
                $"Имя: {currAvitoItem.avitoItemSeller.name}, рейтинг: {currAvitoItem.avitoItemSeller.rating}, отзывы: {currAvitoItem.avitoItemSeller.ratingCount}");

        }
        private void WriteLineAvitoItemsToSend(List<AvitoItem> avitoItemsToSend)
        {
            foreach (var avitoItem in avitoItemsToSend){
                if (avitoItem != null)
                    WriteLineAvitoItemDescription(avitoItem);
            }
        }
        private async void SendAvitoItemsToUserAsync(List<AvitoItem> avitoItemsToSend)
        {
            await Task.Run(async () => {
                foreach (var item in avitoItemsToSend)
                {
                    AvitoTelegramBot.SendAvitoItemAsync(item, chatId);
                    await Task.Delay(3000);
                }
            });
        }

        private HtmlNodeCollection GetItemsAllInfoFromDoc()
        {
            return doc.DocumentNode.SelectNodes("//div[@data-marker = 'item']//div[contains(@class ,'iva-item-content')]");
        }
        private string GetFirstItemLinkFromAllItems(HtmlNodeCollection itemsAllInfo)
        {
            return itemsAllInfo[0].SelectSingleNode(".//div[contains(@class,'iva-item-body')]//a").Attributes["href"].Value;
        }

    }
}

