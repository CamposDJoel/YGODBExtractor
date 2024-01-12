using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public static class KonamiCardListPage
    {
        public static string Xpath_ResultsBanner = "//div[contains(text(), \"Results\")]";
        public static string Xpath_CardListContainer = "//div[@id=\"card_list\"]";
        public static string Xpath_CardsPerPageDDBase = "//select[@id=\"rp\"]";
        public static string Xpath_CardsPerPageDD100Option = "//option[.='Show 100 items per page.']";
        public static string Xpath_ViewAsListButton = "//span[.='View as List']";
        public static string Xpath_CardTotals = "//div[@class=\"text\"]";
        public static string Xpath_CardInfoRow = "//div[@id=\"card_list\"]//div[@class=\"card_name flex_1\"]/input";

        private static int _CurrentPage = 1;


        public static void WaitUntilPageIsLoaded()
        {
            Element.WaitUntilElementIsVisble(Xpath_ResultsBanner);
            Element.WaitUntilElementIsVisble(Xpath_CardListContainer);
        }
        public static void Set100ItemsPerPageView()
        {
            Element.ClickByXpath(Xpath_CardsPerPageDDBase);
            Element.ClickByXpath(Xpath_CardsPerPageDD100Option);
            Element.ClickByXpath(Xpath_ViewAsListButton);           
        }
        public static int GetCardListTotalCards()
        {
            string totalsString = Element.GetText(Xpath_CardTotals);

            //mod the string to make it ready for conversion
            int indexofF = totalsString.IndexOf('f');
            totalsString.Replace(",", "");

            totalsString = totalsString.Substring(indexofF + 2);

            int total = Convert.ToInt32(totalsString);

            return total;
        }
        public static int GetPageCount()
        {
            int totalcards = GetCardListTotalCards();
            int pages = (totalcards / 100) + 1;
            return pages;

        }
        public static int GetCardsCountInCurrentPage()
        {
            return Element.GetElementCount(Xpath_CardInfoRow);
        }
        public static string GetCardName(int index)
        {
            return Element.GetText("//div[@id=\"card_list\"]/div/div[" + index + "]//div[@class=\"card_name flex_1\"]//span");
        }
        public static string GetCardURL(int index)
        {
            ////div[@id="card_list"]/div/div[${i}]//div[@class="card_name flex_1"]//input@value
            return Element.GetElementAttribute("//div[@id=\"card_list\"]/div/div[" + index + "]//div[@class=\"card_name flex_1\"]//input", "value");
        }
        public static void ClickNextPage()
        {
            _CurrentPage++;
            Element.ScrollToView("//a[.='" + _CurrentPage + "']");
            Element.ClickByXpath("//a[.='" + _CurrentPage + "']");
            WaitUntilPageIsLoaded();
        }
    }
}
