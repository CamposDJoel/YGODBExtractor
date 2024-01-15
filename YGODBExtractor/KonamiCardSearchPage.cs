using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YGODBExtractor
{
    public static class KonamiCardSearchPage
    {
        //Common element IDs
        public static string Xpath_GroupBanner = "//div[@id=\"ctype_set\"]";
        public static string Xpath_CookiesAcceptButton = "//button[@id=\"onetrust-accept-btn-handler\"]";

        public static string Xpath_SearchButton = "//div[@id=\"submit_area\"]//span[.='Search']";
        public static string Xpath_MonsterGroup = "//li/span[.='Monster Cards']";
        public static string Xpath_SpellGroup = "//li/span[.='Spell Cards']";
        public static string Xpath_TrapGroup = "//li/span[.='Trap Cards']";


        public static string Xpath_WarriorTypeButton = "//li[@class=\"species_4_en\"]//span";
        public static string Xpath_BeastTypeButton = "//li[@class=\"species_6_en\"]//span";



        public static void WaitUntilPageIsLoaded()
        {
            Element.WaitUntilElementIsVisble(Xpath_SearchButton);
            Element.WaitUntilElementIsVisble(Xpath_GroupBanner);
        }
        public static void AcceptCookiesBanner()
        {
            //Try to accept cockies
            try
            {
                Element.ClickByXpath(Xpath_CookiesAcceptButton);
            }
            catch (Exception)
            {
                GlobalData.Log.Add("No Cookies Banner was displayed. It wasnt clear out.");
            }
        }
        public static void SearchMonsterCard(CardGroup group)
        {
            //Convert the group enum to its string counterpart
            string groupString = GlobalData.CardGroupToString(group);

            //Click the Monster Group Tab
            Element.ClickByXpath(Xpath_MonsterGroup);

            //Click the respective group
            if (group == CardGroup.Beast_Monsters) { Element.ClickByXpath(Xpath_BeastTypeButton); }
            else if (group == CardGroup.Warrior_Monsters) { Element.ClickByXpath(Xpath_WarriorTypeButton); }
            else 
            {
                Element.ClickByXpath("//span[contains(text(), \"" + groupString + "\")]");
            }

            //Click Search
            Element.ClickByXpath(Xpath_SearchButton);

            //Call the CardListPage to wait until that page is loaded
            KonamiCardListPage.WaitUntilPageIsLoaded();

            //Change the view to 100 cards per page as a list
            KonamiCardListPage.Set100ItemsPerPageView();
        }
        public static void SearchSpellCard(CardGroup group)
        {
            //Convert the group enum to its string counterpart
            string groupString = GlobalData.CardGroupToString(group);

            //Click the Monster Group Tab
            Element.ClickByXpath(Xpath_MonsterGroup);

            //Click the respective group
            if (group == CardGroup.Beast_Monsters) { Element.ClickByXpath(Xpath_BeastTypeButton); }
            else if (group == CardGroup.Warrior_Monsters) { Element.ClickByXpath(Xpath_WarriorTypeButton); }
            else
            {
                Element.ClickByXpath("//span[contains(text(), \"" + groupString + "\")]");
            }

            //Click Search
            Element.ClickByXpath(Xpath_SearchButton);

            //Call the CardListPage to wait until that page is loaded
            KonamiCardListPage.WaitUntilPageIsLoaded();

            //Change the view to 100 cards per page as a list
            KonamiCardListPage.Set100ItemsPerPageView();
        }
    }
}
