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

        public static string GetSetReleaseDate(int index)
        {
            return Element.GetText("//div[@class=\"t_body\"]/div[" + index + "]//div[@class=\"time\"]");
        }
        public static string GetSetCode(int  index)
        {
            return Element.GetText("//div[@id=\"update_list\"]//div[@class=\"t_body\"]/div[@class=\"t_row\"][" + index + "]//div[@class=\"card_number\"]");
        }
        public static string GetSetName(int index)
        {
            return Element.GetText("//div[@id=\"update_list\"]//div[@class=\"t_body\"]/div[@class=\"t_row\"][" + index + "]//div[@class=\"pack_name flex_1\"]");
        }
        public static string GetRarity(int index)
        {
            return Element.GetText("//div[@id=\"update_list\"]/div[@class=\"t_body\"]/div[" + index + "]//span");
        }
    }
}
