using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using YGODBExtractor;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using OpenQA.Selenium.DevTools.V118.Browser;

namespace SeleniumTest
{
    class Program
    {      
        static void Main(string[] args)
        {
            //Set the test group
            CardGroup CurrentTestGroup = CardGroup.Aqua_Monsters;

            //Initialize the driver and open the browser
            Driver.OpenBrowser();

            //Open Konami's Card Search Page
            Driver.GoToURL(GlobalData.KonamiDB_URL);
            KonamiCardSearchPage.WaitUntilPageIsLoaded();

            //Clear Cookies Banner
            KonamiCardSearchPage.AcceptCookiesBanner();

            //Load the Current DB for the group being worked on
            LoadCurrentDBFile(CurrentTestGroup);

            //Test the Data
            //foreach(CardInfo cardInfo in CurrentDB.AquaMonsters)
            //{
              //  Console.WriteLine(cardInfo.GetMasterInfoLine()); 
            //}
            
            //Do the group search
            KonamiCardSearchPage.SearchMonsterCard(CurrentTestGroup);

            //Extract how many cards in total are in this group
            int totalCards = KonamiCardListPage.GetCardListTotalCards();

            //Scan all the pages to populate the Konami URL list          
            int totalPages = KonamiCardListPage.GetPageCount();
            Dictionary<string, string> KonamiURLs = new Dictionary<string, string>();
            for(int x = 1; x <= totalPages; x++) 
            {
                int cardsInPage = KonamiCardListPage.GetCardsCountInCurrentPage();
                for(int y = 1; y <= cardsInPage; y++) 
                {
                    string cardName = KonamiCardListPage.GetCardName(y);
                    string cardURL = KonamiCardListPage.GetCardURL(y);
                    //Add the Name/URL combo into the dictionary
                    KonamiURLs.Add(cardName, cardURL);
                }
                //Move to the next page except if we already on the last page
                if (x < totalPages) { KonamiCardListPage.ClickNextPage(); }
            }

            //test
            //foreach (KeyValuePair<string, string> entry in KonamiURLs)
            //{
            //Console.WriteLine("Key: " +  entry.Key + "|URL:" + entry.Value);
            //}

            //Now Access each individual Card
            foreach (KeyValuePair<string, string> card in KonamiURLs)
            {
                //set the card name for readibility and use
                string CardName = card.Key;
                string KomaniURL = "https://www.db.yugioh-card.com/" + card.Value;

                //Go to the card info page
                Driver.GoToURL(KomaniURL);
                KonamiCardInfoPage.WaitUntilPageIsLoaded();

                //Extract the amount of sets (we already have the name)
                int setsCountNow = KonamiCardInfoPage.GetSetsCount();

                //Check this card against the current DB
                if(CurrentDB.CardExist(CardName)) 
                {
                    //Check its sets
                    int currentSetsAmountInDB = CurrentDB.GetCard(CardName).SetsCount;

                    if (setsCountNow > currentSetsAmountInDB)
                    {
                        //New Sets exists, extract the sets again and gets the new set(s) TCG links from Prodeck
                        CardInfo CardsNewInfo = CurrentDB.GetCard(CardName).GetCopyWithoutSets();
                        for(int x = 1; x <= setsCountNow; x++)
                        {
                            string releaseDate = KonamiCardInfoPage.GetSetReleaseDate(x);
                            string code = KonamiCardInfoPage.GetSetCode(x);
                            string name = KonamiCardInfoPage.GetSetName(x);
                            string rarity = KonamiCardInfoPage.GetRarity(x);
                            CardsNewInfo.AddSet(releaseDate, code, name, rarity);
                        }

                        //Go to its Prodeck URL
                        Driver.GoToURL("https://ygoprodeck.com/card/elemental-mistress-doriado-8254"); //TODO: GetProDeckURL(NAme);
                        ProdeckCardInfoPage.WaitUntilPageIsLoaded();

                        //Validate if this page contains TCG prices
                        if(ProdeckCardInfoPage.PageContainsTCGPrices())
                        {
                            if(ProdeckCardInfoPage.TCGPricesHasViewMore())
                            {
                                //Click the view more and extract the links there
                            }
                            else
                            {
                                //extract the links directly from the page.
                            }
                        }
                        else
                        {
                            //Do nothing, all prices were set to $0.00 by default.
                        }
                        
                    }
                    else
                    {
                        //otherwise simply extract the prices from the saved TCG Player URL list
                        CardInfo CardsNewInfo = CurrentDB.GetCard(CardName).GetCopy();
                        foreach(Set thisSet in CardsNewInfo.Sets)
                        {
                            string Code = thisSet.Code;
                            string TCGURL = "https://www.tcgplayer.com/product/520489/yugioh-age-of-overlord-sp-little-knight?promo_name=homepage-small-cards&promo_id=homepage-small-cards&promo_creative=left-side-stack&promo_position=4&Language=English"; // TODO: GetTCGPlayerURL(string code);
                            Driver.GoToURL(TCGURL);
                            TCGCardInfoPage.WaitUntilPageIsLoaded();

                            //Extract the updated prices
                            string marketPrice = TCGCardInfoPage.GetMarketPrice();
                            string mediamPrice = TCGCardInfoPage.GetMediamPrice();

                            //Overide the current prices
                            thisSet.OverridePrices(marketPrice, mediamPrice);

                            //Add it to the new DB
                            NewDB.CardInfoList.Add(CardsNewInfo);
                            NewDB.CardNamesList.Add(CardName);
                        }

                    }
                }
                else
                {
                    //TODO: Otherwise, extract the whole card
                }
            }

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
                //Extract the line and create the CardInfo Object from it.
                Line = SR_SaveFile.ReadLine();
                CardInfo thisCard = new CardInfo(Line);

                //Add it to the list
                CurrentDB.CardInfoList.Add(thisCard);
                CurrentDB.CardNamesList.Add(thisCard.Name);
            }


        }
    }
}
