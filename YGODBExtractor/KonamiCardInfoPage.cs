using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public static class KonamiCardInfoPage
    {
        public static string Xpath_CardNameBanner = "//div[@id=\"cardname\"]/h1";
        public static string Xpath_SetsContainer = "//div[@id=\"update_list\"]";
        public static string Xpath_SetsRows = "//div[@id=\"update_list\"]/div[2]/div[@class='t_row']";

        public static void WaitUntilPageIsLoaded()
        {
            Element.WaitUntilElementIsVisble(Xpath_CardNameBanner);
            Element.WaitUntilElementIsVisble(Xpath_SetsContainer);
        }

        public static int GetSetsCount()
        {
            return Element.GetElementCount(Xpath_SetsRows);
        }

        
    }
}
