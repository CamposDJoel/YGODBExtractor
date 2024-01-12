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

        public static string Xpath_InvalidPage = "//span[.='Sorry but that page does not exist on our site!']";

        public static void WaitUntilPageIsLoaded()
        {
            Element.WaitUntilElementIsVisble(Xpath_CardNameBanner);
            Element.WaitUntilElementIsVisble(Xpath_ProductDetailsContainer);
            Element.WaitUntilElementIsVisble(Xpath_PricesHeader);
        }

        public static bool IsAValidPage()
        {
            return !Element.ElementExist(Xpath_InvalidPage);
        }

        public static string GetCode()
        {
            return Element.GetText(Xpath_Code);
        }
        public static string GetMarketPrice()
        {
            string price  = Element.GetText(Xpath_MarketPrice);
            if(price == "-") { price = "$0.00"; }
            return price;
        }
        public static string GetMediamPrice()
        {
            string price = Element.GetText(Xpath_MediamPrice);
            if (price == "-") { price = "$0.00"; }
            return price;
        }
    }
}
