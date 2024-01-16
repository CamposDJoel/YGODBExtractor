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
using System.Diagnostics;

namespace YGODBExtractor
{
    class Program
    {      
        static void Main(string[] args)
        {
            //STEP 1:Suite Set up
            SUITE_SETUP();

            //STEP 2: Set the TEST GROUP(s)
            /*
            List<CardGroup> CardGroups = new List<CardGroup>
            {
                CardGroup.Aqua_Monsters,
                CardGroup.Beast_Monsters,
                CardGroup.BeastWarrior_Monsters,
                CardGroup.Cyberse_Monsters,
                CardGroup.Dinosaur_Monsters,
                CardGroup.DivineBeast_Monsters,
                CardGroup.Dragon_Monsters,
                CardGroup.Fairy_Monsters,
                CardGroup.Fiend_Monsters,
                CardGroup.Fish_Monsters,
                CardGroup.Insect_Monsters,
                CardGroup.Machine_Monsters,
                CardGroup.Plant_Monsters,
                CardGroup.Psychic_Monsters,
                CardGroup.Pyro_Monsters,
                CardGroup.Reptile_Monsters,
                CardGroup.Rock_Monsters,
                CardGroup.SeaSerpent_Monsters,
                CardGroup.Spellcaster_Monsters,
                CardGroup.Thunder_Monsters,
                CardGroup.Warrior_Monsters,
                CardGroup.WingedBeast_Monsters,
                CardGroup.Wyrm_Monsters,
                CardGroup.Zombie_Monsters
            };*/

            List<CardGroup> CardGroups = new List<CardGroup>
            {
                CardGroup.Counter_Traps
            };

            //STEP 3: RUN ALL THE TEST CASES
            foreach (CardGroup CurrentTestGroup in CardGroups) 
            {
                try
                {
                    //STEP 3: Test Set up
                    TEST_SETUP(CurrentTestGroup);

                    //STEP 4: RUN THE TC
                    MASTERTESTCASE(CurrentTestGroup);
                }
                catch (Exception e)
                {
                    //Log
                    GlobalData.RecordLog("!!!!!! SOMETHING FAILED DURING TEST SETUP or MASTER TC, SKIPPING GROUP, REVIEW EXCEPTION BELOW:");
                    GlobalData.RecordLog(e.Message);
                }
                
                //Test Teardown
                TEST_TEARDOWN(CurrentTestGroup);
            }

            //FINAL: Suite Teardown
            SUITE_TEARDOWN();
        }

        private static void SUITE_SETUP()
        {
            //Log
            GlobalData.RecordLog("---SUITE SETUP START---");

            //Initialize the driver and open the browser
            Driver.OpenBrowser();

            //Load the Current DB for Prodeck URLs and TCG Player
            LoadProdeckURLS();
            LoadTCGPlayerURLs();
        }
        private static void TEST_SETUP(CardGroup CurrentTestGroup)
        {
            //Log
            GlobalData.RecordLog("---TEST SETUP START---");
            GlobalData.RecordLog("---TEST GROUP: " + CurrentTestGroup.ToString() + "---");

            //Open Konami's Card Search Page
            Driver.GoToURL(GlobalData.KonamiDB_URL);
            KonamiCardSearchPage.WaitUntilPageIsLoaded();

            //Clear Cookies Banner
            KonamiCardSearchPage.AcceptCookiesBanner();                     
        }
        private static void MASTERTESTCASE(CardGroup CurrentTestGroup)
        {
            //Log
            GlobalData.RecordLog("---MASTER TC START---");

            //Start a timer
            var watch = new Stopwatch();
            watch.Start();
            GlobalData.RecordLog("Timer Starts!");

            //Load the Current DB for the group being worked on
            LoadCurrentDBFile(CurrentTestGroup);

            //Do the group search
            KonamiCardSearchPage.SearchCardGroup(CurrentTestGroup);

            //Extract how many cards in total are in this group
            int totalCards = KonamiCardListPage.GetCardListTotalCards();

            //Scan all the pages to populate the Konami URL list          
            int totalPages = KonamiCardListPage.GetPageCount();
            Dictionary<string, string> KonamiURLs = new Dictionary<string, string>();
            for (int x = 1; x <= totalPages; x++)
            {
                int cardsInPage = KonamiCardListPage.GetCardsCountInCurrentPage();
                for (int y = 1; y <= cardsInPage; y++)
                {
                    string cardName = KonamiCardListPage.GetCardName(y);
                    string cardURL = KonamiCardListPage.GetCardURL(y);
                    //Add the Name/URL combo into the dictionary
                    KonamiURLs.Add(cardName, cardURL);
                }
                //Move to the next page except if we already on the last page
                if (x < totalPages) { KonamiCardListPage.ClickNextPage(); }
                else { KonamiCardListPage.ResetPageNumber(); }
            }
            //Quick Verification
            if (KonamiURLs.Count != totalCards)
            {
                throw new Exception("The Konami URL extraction failed. Total Cards displayed: " + totalCards + " but URLs Extracted: " + KonamiURLs.Count);
            }

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
                if (CurrentDB.CardExist(CardName))
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
                        for (int x = 1; x <= setsCountNow; x++)
                        {
                            string releaseDate = KonamiCardInfoPage.GetSetReleaseDate(x);
                            string code = KonamiCardInfoPage.GetSetCode(x);
                            string name = KonamiCardInfoPage.GetSetName(x);
                            string rarity = KonamiCardInfoPage.GetRarity(x);
                            CardsNewInfo.AddSet(releaseDate, code, name, rarity);
                        }

                        //Check if this card has a Prodeck URL
                        if (CurrentDB.ProdeckURLExist(CardName))
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

                            if (SearchSucess)
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
                                string Code = CardsNewInfo.Sets[x].Code;
                                Set thisSet = CardsNewInfo.Sets[x];

                                if (CurrentDB.TCGPlayerURLExistg(Code))
                                {
                                    //log
                                    sb.Append("Code: " + Code + "'s TCG URL Found!|");

                                    string TCGURL = CurrentDB.GetTCGPlayerURL(Code);
                                    Driver.GoToURL(TCGURL);
                                    bool PageLoadedCorrectly = TCGCardInfoPage.WaitUntilPageIsLoaded();

                                    if (PageLoadedCorrectly)
                                    {
                                        //Extract the updated prices
                                        string marketPrice = TCGCardInfoPage.GetMarketPrice();
                                        string mediamPrice = TCGCardInfoPage.GetMediamPrice();

                                        //Overide the current prices
                                        thisSet.OverridePrices(marketPrice, mediamPrice);
                                    }
                                    else
                                    {
                                        sb.Append("***TCG Link Page Failed to load... Review it***|");
                                    }
                                }
                                else
                                {
                                    //log
                                    sb.Append("Code: " + Code + "'s TCG URL NOT Found!, searching the extracted urls.|");

                                    //Find the code in the available links extracted from prodeck
                                    string finalURLUsed = "NONE";
                                    foreach (string url in availableUrls)
                                    {
                                        Driver.GoToURL(url);

                                        if (TCGCardInfoPage.IsAValidPage())
                                        {
                                            bool PageLoadedCorrectly = TCGCardInfoPage.WaitUntilPageIsLoaded();

                                            if (PageLoadedCorrectly)
                                            {
                                                //If the page corresponds to the code, then extract its price
                                                string CodeInPage = TCGCardInfoPage.GetCode();
                                                if (Code == CodeInPage)
                                                {
                                                    double currentMarketPrice = CovertPriceToDouble(thisSet.MarketPrice);

                                                    string priceInPageMarketstr = TCGCardInfoPage.GetMarketPrice();
                                                    string priceInPageMedianstr = TCGCardInfoPage.GetMediamPrice();

                                                    double priceInPageMarket = CovertPriceToDouble(priceInPageMarketstr);
                                                    double priceInPageMedian = CovertPriceToDouble(priceInPageMedianstr);

                                                    if (currentMarketPrice == 0.00)
                                                    {
                                                        if (priceInPageMarket > 0)
                                                        {
                                                            thisSet.OverridePrices(priceInPageMarketstr, priceInPageMedianstr);
                                                            finalURLUsed = url;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (priceInPageMarket < currentMarketPrice)
                                                        {
                                                            thisSet.OverridePrices(priceInPageMarketstr, priceInPageMedianstr);
                                                            finalURLUsed = url;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //DO nothing just continue the loop
                                                }
                                            }
                                            else
                                            {
                                                //Also do nothing, bad page
                                            }
                                        }
                                        else
                                        {
                                            //Do nothing, Skip URL
                                        }
                                    }

                                    //if a finalURL was set, save it to the TCG Player URLs list
                                    if (finalURLUsed != "NONE")
                                    {
                                        CurrentDB.TCGPlayerURLs.Add(Code, finalURLUsed);
                                        sb.Append("URL found and saved!|");
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Do nothing, all prices were set to $0.00 by default.
                            sb.Append("TCG Prices NOT available! all will be set to zero|");
                        }

                        //CardIfo Object is ready, Add it to the new DB
                        NewDB.CardInfoList.Add(CardsNewInfo);
                        NewDB.CardNamesList.Add(CardName);
                    }
                    else
                    {
                        //log
                        sb.Append("No new Sets!|");

                        //otherwise simply extract the prices from the saved TCG Player URL list
                        CardInfo CardsNewInfo = CurrentDB.GetCard(CardName).GetCopy();
                        foreach (Set thisSet in CardsNewInfo.Sets)
                        {
                            string Code = thisSet.Code;
                            if (CurrentDB.TCGPlayerURLExistg(Code))
                            {
                                //log
                                sb.Append("Code: " + Code + "'s TCG URL Found!|");

                                string TCGURL = CurrentDB.GetTCGPlayerURL(Code);
                                Driver.GoToURL(TCGURL);
                                bool PageLoadedCorrectly = TCGCardInfoPage.WaitUntilPageIsLoaded();

                                if (PageLoadedCorrectly)
                                {
                                    //Extract the updated prices
                                    string marketPrice = TCGCardInfoPage.GetMarketPrice();
                                    string mediamPrice = TCGCardInfoPage.GetMediamPrice();

                                    //Overide the current prices
                                    thisSet.OverridePrices(marketPrice, mediamPrice);
                                }
                                else
                                {
                                    sb.Append("***TCG Link Page Failed to load... Review it***|");
                                    GlobalData.TCGUrlsThatFailedLoading.Add(thisSet.Code + "|" + TCGURL);
                                }

                            }
                            else
                            {
                                //Do nothing keep the old amount
                                //log
                                sb.Append("Code: " + Code + "'s TCG URL NOT available!|");
                                GlobalData.CodesWithoutTCGLink.Add(CardName + "|" + Code);
                            }
                        }

                        //CardInfo Object is ready, Add it to the new DB
                        NewDB.CardInfoList.Add(CardsNewInfo);
                        NewDB.CardNamesList.Add(CardName);

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

            //Stop Timer
            watch.Stop();
            GlobalData.RecordLog($"Execution Time was: {watch.Elapsed} |");
        }
        private static void TEST_TEARDOWN(CardGroup CurrentTestGroup)
        {
            //TEST TEARDOWN
            SavePostRunFiles(CurrentTestGroup);

            //Final Verification
            /*(if (NewDB.CardNamesList.Count != expectedTotalCards)
            {

                GlobalData.RecordLog("TEST FAILED!!!!!!!!!!!!!!!!!!");
                GlobalData.RecordLog("The total # of cards extracted doesnt match the expected #. Verify output file to determine error. Cards in NewDB: " +
                    NewDB.CardNamesList.Count + " vs expected: " + expectedTotalCards);
            }
            else
            {
                GlobalData.RecordLog("TEST PASSED!!!!!!!!!!!!!!!!!!");
            }*/

            //CLEAN UP
            CurrentDB.CardInfoList.Clear();
            CurrentDB.CardNamesList.Clear();

            NewDB.CardNamesList.Clear();
            NewDB.CardNamesList.Clear();
        }
        private static void SUITE_TEARDOWN()
        {
            GlobalData.Chrome.Close();
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

            //TEST
            /*List<CardInfo> listwithoutsets = new List<CardInfo>();
            foreach(CardInfo card in CurrentDB.CardInfoList) 
            {
                listwithoutsets.Add(card.GetCopyWithoutSets());
            }
            List<string> newDBData = new List<string>();
            newDBData.Add(listwithoutsets.Count.ToString());
            foreach (CardInfo card in listwithoutsets)
            {
                newDBData.Add(card.GetMasterInfoLine());
            }
            //Write file
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\NewDB\\listwithoutsets.txt", newDBData);*/


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

                if(CurrentDB.TCGPlayerURLs.ContainsKey(code)) 
                {
                    Console.WriteLine("Duplicate code found: " + code);
                }
                else
                {
                    //Populate the Dictionary
                    CurrentDB.TCGPlayerURLs.Add(code, url);
                }                
            }

            SR_SaveFile.Close();

            //Log
            GlobalData.RecordLog("Load TCG Player URLs DB Successful!");
        }
        private static double CovertPriceToDouble(string price)
        {
            price = price.Replace("$", "");
            return Convert.ToDouble(price);
        }
        private static void SavePostRunFiles(CardGroup group)
        {
            //Override the Prodeck URLs
            List<string> prodeckUrlsData = new List<string>();
            prodeckUrlsData.Add(CurrentDB.ProdeckURLs.Count.ToString());
            foreach(KeyValuePair<string, string> line in CurrentDB.ProdeckURLs)
            {
                prodeckUrlsData.Add(line.Key + "|" + line.Value);
            }
            //Write file
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\MasterURLFiles\\ProdeckURLs.txt", prodeckUrlsData);
            GlobalData.RecordLog("ProdeckURLs.txt file overwriten!!");

            //////////////////////////////////////////////////////

            //Override the TCG URLs
            List<string> tcgUrlsData = new List<string>();
            tcgUrlsData.Add(CurrentDB.TCGPlayerURLs.Count.ToString());
            foreach (KeyValuePair<string, string> line in CurrentDB.TCGPlayerURLs)
            {
                tcgUrlsData.Add(line.Key + "|" + line.Value);
            }
            //Write file
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\MasterURLFiles\\TCGPlayerURLs.txt", tcgUrlsData);
            GlobalData.RecordLog("TCGPlayerURLs.txt file overwriten!!");

            //////////////////////////////////////////////////////

            //Write out the failed manual search cards
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\Results Data\\CardsThatFailedManualSearch.txt", GlobalData.CardsThatFailedManualSearch);
            GlobalData.RecordLog("CardsThatFailedManualSearch.txt file overwriten!!");

            /////////////////////////////////////////////////////////

            //Write out the failed manual search cards
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\Results Data\\exitingCodesWithoutTCGURL.txt", GlobalData.CodesWithoutTCGLink);
            GlobalData.RecordLog("exitingCodesWithoutTCGURL.txt file overwriten!!");

            /////////////////////////////////////////////////////////

            //Write out the TCG URLS that failed to load
            //Write out the failed manual search cards
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\Results Data\\TCGUrlsThatFailedLoading.txt", GlobalData.TCGUrlsThatFailedLoading);
            GlobalData.RecordLog("TCGUrlsThatFailedLoading.txt file overwriten!!");

            /////////////////////////////////////////////////////////


            //Write out the new DB
            List<string> newDBData = new List<string>();
            newDBData.Add(NewDB.CardInfoList.Count.ToString());
            foreach(CardInfo card in NewDB.CardInfoList)
            {
                newDBData.Add(card.GetMasterInfoLine());
            }
            //Write file
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\NewDB\\" + group + ".txt", newDBData);
            GlobalData.RecordLog("Group New DB file overwriten!!");
        }
    }
}