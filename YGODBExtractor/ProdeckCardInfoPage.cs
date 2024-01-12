using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    internal class ProdeckCardInfoPage
    {
        public static string Xpath_Cardimage = "//div[@class=\"card-image\"]";
        public static string Xpath_CardInfo = "//ul[@class=\"card-data-info\"]";
        public static string Xpath_TCGPricesContainer = "//div[@class=\"card-sets-tcgplayer-small\"]";
        public static string Xpath_ViewMoreLink = "//div[@class=\"card-sets-tcgplayer-small\"]//a[.='View More']";
        public static string Xpath_ViewMoreLinksContainer = "//div[@class=\"modal-body tcgplayer-modal\"]";

        public static string Xpath_PricesInPage = "//div[@class=\"card-sets-tcgplayer-small\"]/div";

        public static void WaitUntilPageIsLoaded()
        {
            Element.WaitUntilElementIsVisble(Xpath_Cardimage);
            Element.WaitUntilElementIsVisble(Xpath_CardInfo);
        }
        public static bool PageContainsTCGPrices()
        {
            return Element.ElementExist(Xpath_TCGPricesContainer);
        }
        public static bool TCGPricesHasViewMore()
        {
            return Element.ElementExist(Xpath_ViewMoreLink);
        }
        public static List<string> GetPricesURLsFromPage()
        {
            //Get the amount of links available
            int linksAvailable = Element.GetElementCount(Xpath_PricesInPage);
            
            //Extract each link and add it to the list
            List<string> urls = new List<string>();
            for(int i = 0; i < linksAvailable; i++) 
            {
                string url = Element.GetElementAttribute("//div[@class=\"card-sets-tcgplayer-small\"]/div[" + i + "]/a", "href");
                urls.Add(url);
            }

            return urls;
        }

        public static void ClickViewMore()
        {
            Element.ClickByXpath(Xpath_ViewMoreLink);
            Element.WaitUntilElementIsVisble(Xpath_ViewMoreLinksContainer);
        }

        public static List<string> GetPricesURLsViewMore()
        {
            //Click the view more 



            //Get the amount of links available
            int linksAvailable = Element.GetElementCount(Xpath_PricesInPage);

            //Extract each link and add it to the list
            List<string> urls = new List<string>();
            for (int i = 0; i < linksAvailable; i++)
            {
                string url = Element.GetElementAttribute("//div[@class=\"card-sets-tcgplayer-small\"]/div[" + i + "]/a", "href");
                urls.Add(url);
            }

            return urls;
        }
    }
}
