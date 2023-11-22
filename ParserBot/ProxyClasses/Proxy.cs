
namespace ParseBotSolution
{
    internal class Proxy
    {
        private string host;
        private string port;
        private string login;
        private string password;
        private string country;
        private bool activeUse = false;

        public Proxy()
        {

        }
        public Proxy(string host, string port, string login, string password, string country)
        {
            this.host = host;
            this.port = port;
            this.login = login;
            this.password = password;
            this.country = country;
        }
        
        public string Host
        {
            get { return host; }
            set { host = value; }
        }
        public string Port
        {
            get { return port; }
            set { port = value; }
        }
        public string Login
        {
            get { return login; }
            set { login = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string Country
        {
            get { return country; }
            set { country = value; }
        }
        public bool ActiveUse
        {
            get { return activeUse; }
            set { activeUse = value; }
        }
    }

}
