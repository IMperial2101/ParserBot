using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseBotSolution
{
    internal class AvitoItem 
    {
        public AvitoItemSeller avitoItemSeller = new AvitoItemSeller();
        public string link;
        public string param;
        public string name;
        public string price;
        public string description;
        public string adress;
        public string photoLink;

        public bool start = true;
    }
}
