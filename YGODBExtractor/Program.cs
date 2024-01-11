using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using YGODBExtractor;

namespace SeleniumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initialize the driver and open the browser
            Driver.OpenBrowser();

            //Open Konami's Card Search Page
            Driver.GoToURL(GlobalData.KonamiDB_URL);
            KonamiCardSearchPage.WaitUntilPageIsLoaded();

            //Clear Cookies Banner
            KonamiCardSearchPage.AcceptCookiesBanner();

            //Load the Current DB for the group being worked on
            LoadCurrentDBFile(CardGroup.Aqua_Monsters);

            //Test the Data
            //foreach(CardInfo cardInfo in CurrentDB.AquaMonsters)
            //{
              //  Console.WriteLine(cardInfo.GetMasterInfoLine()); 
            //}
            
            //Do the group search
            KonamiCardSearchPage.SearchMonsterCard(CardGroup.Aqua_Monsters);

            //Extract how many cards in total are in this group
            int totalcards = KonamiCardListPage.GetCardListTotalCards();





            Console.WriteLine("TEST PASSED!!!!!!!!!!!!!!!!!!");
        }

        private static void LoadCurrentDBFile(CardGroup cardGroup) 
        {
            //Stream that reads the actual save file.
            StreamReader SR_SaveFile = new StreamReader(
                Directory.GetCurrentDirectory() + "\\CurrentDB\\" + cardGroup + ".txt");

            //First line contains how many cards are in this group
            string Line = SR_SaveFile.ReadLine();
            int CardAmount = Convert.ToInt32(Line);

            //Loop thru each card, convert it into a CardInfo Object and Populate the Current Master DB
            for (int i = 0; i < CardAmount; i++) 
            {
                Line = SR_SaveFile.ReadLine();

                switch(cardGroup)
                {
                    case CardGroup.Aqua_Monsters: CurrentDB.AquaMonsters.Add(new CardInfo(Line)); break;
                        //TODO: Finish this swtich statement
                }
            }


        }
    }
}
