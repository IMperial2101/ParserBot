using System.Net;
using Newtonsoft.Json.Linq;


namespace ParseBotSolution
{
    internal class ProxyController
    {
        private List<Proxy> proxiesList = new List<Proxy>();
        public ProxyController()
        {
            proxiesList = GetProxiesToList();
        }
        public List<Proxy> GetProxiesToList()
        {
            List<Proxy> proxiesList = new List<Proxy>();

            WebClient webClient = new WebClient();
            string html = webClient.DownloadString("https://proxy6.net/api/769d0a67bd-e5b6362d8b-23f6b69891/getproxy");
            JObject proxiesJsonFullInfo = JObject.Parse(html);

            JObject proxiesJson = (JObject)proxiesJsonFullInfo["list"];
            foreach (var currentProxyJson in proxiesJson)
            {
                JObject oneProxyJson = (JObject)currentProxyJson.Value;
                string host = (string)oneProxyJson["host"];
                string port = (string)oneProxyJson["port"];
                string user = (string)oneProxyJson["user"];
                string pass = (string)oneProxyJson["pass"];
                string country = (string)oneProxyJson["country"];
                Proxy proxy = new Proxy(host, port, user, pass, country);

                proxiesList.Add(proxy);
            }
            return proxiesList;
        }
        public void WritelineProxy()
        {
            foreach (var proxy in proxiesList)
                Console.WriteLine($"{proxy.Host}:{proxy.Port} {proxy.Login} {proxy.Password} {proxy.Country}");
        }
        public static void AddProxyToHttpClientHandler(HttpClientHandler handler,Proxy proxy)
        {
            handler.Proxy = new WebProxy(proxy.Host, Convert.ToInt32(proxy.Port));
            handler.Proxy.Credentials = new NetworkCredential(proxy.Login, proxy.Password);
            handler.UseDefaultCredentials = false;
        }
        public static void AddProxyToDataBase(List<Proxy> proxyList,DataBase dataBase)
        {
            dataBase.SqlCommand("TRUNCATE TABLE Proxies");
            var first = proxyList[0];
            string sqlCommand = string.Format("INSERT INTO Proxies (host, port, login, password, country) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')", first.Host,first.Port,first.Login,first.Password,first.Country);

            Proxy currProxy;
            string valueToAdd;
            for(int i = 1; i < proxyList.Count; i++)
            {
                currProxy = proxyList[i];
                valueToAdd = string.Format(",('{0}', '{1}', '{2}', '{3}', '{4}')", currProxy.Host, currProxy.Port, currProxy.Login, currProxy.Password, currProxy.Country);
                sqlCommand += valueToAdd;
            }
            sqlCommand += ";";
            dataBase.SqlCommand(sqlCommand);
        }
        public Proxy GetNoActiveProxy()
        {
            for(int i = 0; i< proxiesList.Count;i++){
                if (proxiesList[i].ActiveUse == false){
                    proxiesList[i].ActiveUse = true;
                    return proxiesList[i];
                } 
            }
            throw new Exception("No Proxy");
            return new Proxy();
        }

    }
}
