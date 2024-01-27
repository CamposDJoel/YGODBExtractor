using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

        public static List<string> CardsThatFailedManualSearch = new List<string>();
        public static List<string> CodesWithoutTCGLink = new List<string>();
        public static List<string> TCGUrlsThatFailedLoading = new List<string>();
        public static List<string> TCGUrlsRescued = new List<string>();
        public static List<string> CardsWithoutProdeckURL = new List<string>();

        //Loggin
        public static List<string> Log = new List<string>();
        public static List<string> SpecialLog = new List<string>();
     
        public static void RecordLog(string message)
        {
            Console.WriteLine(message);
            Log.Add(message);
        }
    }
}
