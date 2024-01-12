using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    internal class TCGCardInfoPage
    {
        public static string Xpath_CardNameBanner = "//h1[@class=\"product-details__name\"]";
        public static string Xpath_ProductDetailsContainer = "//div[@class=\"product__item-details__description\"]";
        public static string Xpath_PricesHeader = "//tr[@class=\"price-points__header\"]";

        public static string Xpath_Code = "//ul[@class=\"product__item-details__attributes\"]/li[1]//span";
        public static string Xpath_MarketPrice = "//section[@class=\"price-points price-guide__points\"]/table/tr[2]/td[2]/span";
        public static string Xpath_MediamPrice = "//section[@class=\"price-points price-guide__points\"]/table/tr[4]/td[2]/span";

        public static void WaitUntilPageIsLoaded()
        {
            Element.WaitUntilElementIsVisble(Xpath_CardNameBanner);
            Element.WaitUntilElementIsVisble(Xpath_ProductDetailsContainer);
            Element.WaitUntilElementIsVisble(Xpath_PricesHeader);
        }

        public static string GetCode()
        {
            return Element.GetText(Xpath_Code);
        }
        public static string GetMarketPrice()
        {
            return Element.GetText(Xpath_MarketPrice);
        }
        public static string GetMediamPrice()
        {
            return Element.GetText(Xpath_MediamPrice);
        }
    }
}
