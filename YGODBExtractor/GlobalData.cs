using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public static class GlobalData
    {
        //The actual Web Driver must be global to be accessible everywhere
        public static IWebDriver Chrome;

        //ULRS
        public static string KonamiDB_URL = "https://www.db.yugioh-card.com/yugiohdb/card_search.action";
        public static string Prodeck_URL = "https://ygoprodeck.com/card-database/?&num=24&offset=0";

        //Loggin
        public static List<string> Log = new List<string>();
    }
}
