using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using OpenQA.Selenium.DevTools.V118.Browser;
using OpenQA.Selenium.DevTools.V118.Storage;

namespace YGODBExtractor
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

            //Load the Current DB for Prodeck URLs and TCG Player
            LoadProdeckURLS();
            LoadTCGPlayerURLs();

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
                //Use this string builder to save the execution log for this card
                StringBuilder sb = new StringBuilder();

                //set the card name for readibility and use
                string CardName = card.Key;
                string KomaniURL = "https://www.db.yugioh-card.com/" + card.Value;
                sb.Append("Card:" + CardName + "|");

                //Go to the card info page
                Driver.GoToURL(KomaniURL);
                KonamiCardInfoPage.WaitUntilPageIsLoaded();

                //Extract the amount of sets (we already have the name)
                int setsCountNow = KonamiCardInfoPage.GetSetsCount();

                //Check this card against the current DB
                if(CurrentDB.CardExist(CardName)) 
                {
                    //log
                    sb.Append("Card In DB|");

                    //Check its sets
                    int currentSetsAmountInDB = CurrentDB.GetCard(CardName).SetsCount;

                    if (setsCountNow > currentSetsAmountInDB)
                    {
                        //Log
                        sb.Append("!!!MORE SETS FOUND!!!|");

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

                        //Check if this card has a Prodeck URL
                        if(CurrentDB.ProdeckURLExist(CardName))
                        {
                            //log
                            sb.Append("Prodeck URL Found!|");

                            //Go to the direct url
                            Driver.GoToURL(CurrentDB.GetProdeckURL(CardName));
                            ProdeckCardInfoPage.WaitUntilPageIsLoaded();
                        }
                        else
                        {
                            //log
                            sb.Append("NO Prodeck URL, doing manual search|");

                            //Go to prodeck and do a manual search
                            Driver.GoToURL(GlobalData.Prodeck_URL);
                            ProDeckCardSearchPage.WaitUntilPageIsLoaded();

                            //Perform the manual search and return if the search was successful
                            bool SearchSucess = ProDeckCardSearchPage.SearchCard(CardName);

                            if(SearchSucess)
                            {
                                sb.Append("Prodeck Search Success!|");

                                //save the url of this card so we dont have to search for it again
                                string currentURL = GlobalData.Chrome.Url;
                                CurrentDB.ProdeckURLs.Add(CardName, currentURL);
                                sb.Append("Prodeck URL saved!|");
                            }
                            else
                            {
                                //Log
                                sb.Append("Prodeck search failed!|");

                                //Save this card name to manually get the url
                                GlobalData.CardsThatFailedManualSearch.Add(CardName);
                            }
                        }

                        //Validate if this page contains TCG prices
                        //This is going to work even if the search failed.
                        if (ProdeckCardInfoPage.PageContainsTCGPrices())
                        {
                            //Log
                            sb.Append("TCG Prices available!|");

                            //Extract the available TCG Player links
                            List<string> availableUrls = new List<string>();
                            if (ProdeckCardInfoPage.TCGPricesHasViewMore())
                            {
                                //Click the view more and extract the links there
                                ProdeckCardInfoPage.ClickViewMore();

                                availableUrls = ProdeckCardInfoPage.GetPricesURLsViewMore();
                                sb.Append(availableUrls.Count + " URLS extracted from view more window.|");
                            }
                            else
                            {
                                //extract the links directly from the page.
                                availableUrls = ProdeckCardInfoPage.GetPricesURLsFromPage();
                                sb.Append(availableUrls.Count + " URLS extracted from page.|");
                            }

                            //Scan each set for its price
                            for (int x = 0; x < setsCountNow; x++)
                            {
                                string code = CardsNewInfo.Sets[x].Code;
                                // TODO: GetTCGPlayerURL(string code);

                            }
                        }
                        else
                        {
                            //Do nothing, all prices were set to $0.00 by default.
                            sb.Append("TCG Prices NOT available! all will be set to zero|");
                        }
                        
                    }
                    else
                    {
                        //log
                        sb.Append("No new Sets!|");

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

                        //log
                        sb.Append("Prices Overrided!|");
                    }
                }
                else
                {
                    //Log
                    sb.Append("NEW CARD!|");

                    //TODO: Otherwise, extract the whole card
                }

                //Save the execution log
                GlobalData.RecordLog(sb.ToString());
            }

            GlobalData.RecordLog("TEST PASSED!!!!!!!!!!!!!!!!!!");
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

            SR_SaveFile.Close();

            //Log
            GlobalData.RecordLog("Load Current DB Successful!");
        }
        private static void LoadProdeckURLS()
        {
            //Stream that reads the actual file.
            StreamReader SR_SaveFile = new StreamReader(
                Directory.GetCurrentDirectory() + "\\MasterURLFiles\\ProdeckURLs.txt");

            //First line contains how many links are in this file
            string Line = SR_SaveFile.ReadLine();
            int LinksAmount = Convert.ToInt32(Line);

            for (int i = 0; i < LinksAmount; i++)
            {
                //Extract the line split the name and URL
                Line = SR_SaveFile.ReadLine();
                string[] tokens = Line.Split("|");

                string cardname = tokens[0];
                string url = tokens[1];

                //Populate the Dictionary
                CurrentDB.ProdeckURLs.Add(cardname, url);
            }

            SR_SaveFile.Close();

            //Log
            GlobalData.RecordLog("Load Prodeck URLs DB Successful!");
        }
        private static void LoadTCGPlayerURLs()
        {
            //Stream that reads the actual file.
            StreamReader SR_SaveFile = new StreamReader(
                Directory.GetCurrentDirectory() + "\\MasterURLFiles\\TCGPlayerURLs.txt");

            //First line contains how many links are in this file
            string Line = SR_SaveFile.ReadLine();
            int LinksAmount = Convert.ToInt32(Line);

            for (int i = 0; i < LinksAmount; i++)
            {
                //Extract the line split the name and URL
                Line = SR_SaveFile.ReadLine();
                string[] tokens = Line.Split("|");

                string code = tokens[0];
                string url = tokens[1];

                //Populate the Dictionary
                CurrentDB.TCGPlayerURLs.Add(code, url);
            }

            SR_SaveFile.Close();

            //Log
            GlobalData.RecordLog("Load TCG Player URLs DB Successful!");
        }
    }
}
