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
    }
}
