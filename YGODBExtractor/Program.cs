//Joel Campos

using System.Text;
using System.Diagnostics;

namespace YGODBExtractor
{
    class Program
    {      
        static void Main(string[] args)
        {
            //Master timer
            var masterwatch = new Stopwatch();
            masterwatch.Start();

            //STEP 1:Suite Set up
            SUITE_SETUP();

            //STEP 2: Set the TEST GROUP(s)          
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
                CardGroup.Zombie_Monsters,
                CardGroup.Normal_Spells,
                CardGroup.Continuous_Spells,
                CardGroup.QuickPlay_Spells,
                CardGroup.Field_Spells,
                CardGroup.Equip_Spells,
                CardGroup.Ritual_Spells,
                CardGroup.Normal_Traps,
                CardGroup.Continuous_Traps,
                CardGroup.Counter_Traps
            };

            /*List<CardGroup> CardGroups = new List<CardGroup>
            {
                CardGroup.Beast_Monsters
            };*/

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

            //TCGRecuerMasterCase();

            masterwatch.Stop();
            GlobalData.RecordLog($"Execution Time for the entire script was: {masterwatch.Elapsed} |");

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
                //set the card name for readibility and use
                string CardName = card.Key;
                string KomaniURL = "https://www.db.yugioh-card.com/" + card.Value;
                Console.Write("Card:" + CardName + "|");

                //Go to the card info page
                Driver.GoToURL(KomaniURL);
                KonamiCardInfoPage.WaitUntilPageIsLoaded();

                //Extract the amount of sets (we already have the name)
                int setsCountNow = KonamiCardInfoPage.GetSetsCount();

                //Check this card against the current DB
                if (CurrentDB.CardExist(CardName))
                {
                    //log
                    Console.Write("Card In DB|");

                    //EXTRA CHECK FOR THE PRODECK URL
                    if (!CurrentDB.ProdeckURLExist(CardName))
                    {
                        //populate the missing prodeck url list
                        GlobalData.CardsWithoutProdeckURL.Add(CardName);
                    }

                        //Check its sets
                        int currentSetsAmountInDB = CurrentDB.GetCard(CardName).SetsCount;

                    if (setsCountNow > currentSetsAmountInDB)
                    {
                        //Log
                        Console.Write("!!!MORE SETS FOUND!!!|");

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
                            Console.Write("Prodeck URL Found!|");

                            //Go to the direct url
                            Driver.GoToURL(CurrentDB.GetProdeckURL(CardName));
                            ProdeckCardInfoPage.WaitUntilPageIsLoaded();
                        }
                        else
                        {
                            //log
                            Console.Write("NO Prodeck URL, doing manual search|");

                            //Go to prodeck and do a manual search
                            Driver.GoToURL(GlobalData.Prodeck_URL);
                            ProDeckCardSearchPage.WaitUntilPageIsLoaded();

                            //Perform the manual search and return if the search was successful
                            bool SearchSucess = ProDeckCardSearchPage.SearchCard(CardName);

                            if (SearchSucess)
                            {
                                Console.Write("Prodeck Search Success!|");

                                //save the url of this card so we dont have to search for it again
                                string currentURL = GlobalData.Chrome.Url;
                                CurrentDB.ProdeckURLs.Add(CardName, currentURL);
                                Console.Write("Prodeck URL saved!|");
                            }
                            else
                            {
                                //Log
                                Console.Write("Prodeck search failed!|");

                                //Save this card name to manually get the url
                                GlobalData.CardsThatFailedManualSearch.Add(CardName);
                            }
                        }

                        //Validate if this page contains TCG prices
                        //This is going to work even if the search failed.
                        if (ProdeckCardInfoPage.PageContainsTCGPrices())
                        {
                            //Log
                            Console.Write("TCG Prices available!|");

                            //Extract the available TCG Player links
                            List<string> availableUrls = new List<string>();
                            if (ProdeckCardInfoPage.TCGPricesHasViewMore())
                            {
                                //Click the view more and extract the links there
                                ProdeckCardInfoPage.ClickViewMore();

                                availableUrls = ProdeckCardInfoPage.GetPricesURLsViewMore();
                                Console.Write(availableUrls.Count + " URLS extracted from view more window.|");
                            }
                            else
                            {
                                //extract the links directly from the page.
                                availableUrls = ProdeckCardInfoPage.GetPricesURLsFromPage();
                                Console.Write(availableUrls.Count + " URLS extracted from page.|");
                            }

                            //Scan each set for its price
                            for (int x = 0; x < setsCountNow; x++)
                            {
                                string Code = CardsNewInfo.Sets[x].Code;
                                Set thisSet = CardsNewInfo.Sets[x];

                                if (CurrentDB.TCGPlayerURLExistg(Code))
                                {
                                    //log
                                    Console.Write("Code: " + Code + "'s TCG URL Found!|");

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
                                        Console.Write("***TCG Link Page Failed to load... Review it***|");
                                    }
                                }
                                else
                                {
                                    //log
                                    Console.Write("Code: " + Code + "'s TCG URL NOT Found!, searching the extracted urls.|");

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
                                        Console.Write("URL found and saved!|");
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Do nothing, all prices were set to $0.00 by default.
                            Console.Write("TCG Prices NOT available! all will be set to zero|");
                        }

                        //CardIfo Object is ready, Add it to the new DB
                        NewDB.CardInfoList.Add(CardsNewInfo);
                        NewDB.CardNamesList.Add(CardName);
                    }
                    else
                    {
                        //log
                        Console.Write("No new Sets!|");

                        //otherwise simply extract the prices from the saved TCG Player URL list
                        CardInfo CardsNewInfo = CurrentDB.GetCard(CardName).GetCopy();
                        foreach (Set thisSet in CardsNewInfo.Sets)
                        {
                            string Code = thisSet.Code;
                            if (CurrentDB.TCGPlayerURLExistg(Code))
                            {
                                //log
                                Console.Write("Code: " + Code + "'s TCG URL Found!|");

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
                                    Console.Write("***TCG Link Page Failed to load... Review it***|");
                                    GlobalData.TCGUrlsThatFailedLoading.Add(thisSet.Code + "|" + TCGURL);
                                }

                            }
                            else
                            {
                                //Do nothing keep the old amount
                                //log
                                Console.Write("Code: " + Code + "'s TCG URL NOT available!|");
                                GlobalData.CodesWithoutTCGLink.Add(CardName + "|" + Code);
                            }
                        }

                        //CardInfo Object is ready, Add it to the new DB
                        NewDB.CardInfoList.Add(CardsNewInfo);
                        NewDB.CardNamesList.Add(CardName);

                        //log
                        Console.Write("Prices Overrided!|");
                    }
                }
                else
                {
                    //Log
                    Console.Write("NEW CARD!|");

                    //TODO: Otherwise, extract the whole card
                }

                //Breawkline
                Console.WriteLine("");
                Console.WriteLine("------------------------------------------------------------------");
            }

            //Stop Timer
            watch.Stop();
            GlobalData.RecordLog($"Execution Time for card group was: {watch.Elapsed} |");
        }
        private static void TCGRecuerMasterCase()
        {            
            LoadTCGRescueList();

            string PreviousName = "NONE";
            List<string> availableUrls = new List<string>();
            //iterate thru all the cards in the rescue list
            foreach (KeyValuePair<string, string> RescueCard in CurrentDB.TCGRescueList)
            {               
                string CardName = RescueCard.Value;
                string Code = RescueCard.Key;
                GlobalData.RecordLog("Card:" + CardName + "|CODE:" + Code + "|");

                if(CardName == PreviousName)
                {
                    //Simply use the already existing available urls
                    GlobalData.RecordLog("Same card as previous, use the existing available urls");
                }
                else
                {
                    //Save this card as the "previous card"
                    PreviousName = CardName;

                    //Go search the card
                    //Go to this card's PRODECK page
                    if (CurrentDB.ProdeckURLExist(CardName))
                    {
                        GlobalData.RecordLog("Prodeck URL Found!|");
                        //Go to the direct url

                        bool ProdeckPageIsValid = false;
                        try
                        {
                            Driver.GoToURL(CurrentDB.GetProdeckURL(CardName));
                            //ProdeckCardInfoPage.WaitUntilPageIsLoaded();
                            ProdeckPageIsValid = ProdeckCardInfoPage.IsPageValid();
                        }
                        catch (Exception)
                        {
                            ProdeckPageIsValid = false;
                        }
                        if (ProdeckPageIsValid)
                        {
                            //Wait
                            ProdeckCardInfoPage.WaitUntilPageIsLoaded();

                            //Extract all the Price URLS
                            //Validate if this page contains TCG prices
                            //This is going to work even if the search failed.
                            if (ProdeckCardInfoPage.PageContainsTCGPrices())
                            {
                                GlobalData.RecordLog("TCG Prices available!|");

                                //Extract the available TCG Player links
                                availableUrls = new List<string>();
                                if (ProdeckCardInfoPage.TCGPricesHasViewMore())
                                {
                                    //Click the view more and extract the links there
                                    ProdeckCardInfoPage.ClickViewMore();

                                    availableUrls = ProdeckCardInfoPage.GetPricesURLsViewMore();
                                    GlobalData.RecordLog(availableUrls.Count + " URLS extracted from view more window.|");
                                }
                                else
                                {
                                    //extract the links directly from the page.
                                    availableUrls = ProdeckCardInfoPage.GetPricesURLsFromPage();
                                    GlobalData.RecordLog(availableUrls.Count + " URLS extracted from page.|");
                                }

                                //Clear the urls that are invalid
                                List<string> removelist = new List<string>();
                                foreach (string url in availableUrls)
                                {
                                    bool PageLoadedCorrectly = false;

                                    try
                                    {
                                        Driver.GoToURL(url);
                                        PageLoadedCorrectly = TCGCardInfoPage.IsAValidPage();
                                    }
                                    catch (Exception)
                                    {
                                        PageLoadedCorrectly = false;
                                    }


                                    if (!PageLoadedCorrectly)
                                    {
                                        removelist.Add(url);
                                    }
                                }

                                foreach(string url in removelist) 
                                {
                                    availableUrls.Remove(url);
                                }
                                GlobalData.RecordLog(removelist.Count + " bad urls found.");
                            }
                            else
                            {
                                availableUrls.Clear();
                                GlobalData.RecordLog("No prices at all for this card, fix it manually...|");
                            }
                        }
                        else
                        {
                            availableUrls.Clear();
                            GlobalData.RecordLog("Prodeck URL failed, skip for now...");
                        }
                    }
                    else
                    {
                        GlobalData.RecordLog("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<NO Prodeck URL ");
                    }
                }

                //Access each extracted URL and save all that contain the code in question.
                GlobalData.RecordLog("Validating available urls, url count: " + availableUrls.Count);
                List<string> ValidURLS = new List<string>();
                foreach (string url in availableUrls)
                {
                    bool PageLoadedCorrectly = false;

                    try
                    {
                        Driver.GoToURL(url);
                        PageLoadedCorrectly = TCGCardInfoPage.IsAValidPage();
                    }
                    catch (Exception)
                    {
                        PageLoadedCorrectly = false;
                    }


                    if (PageLoadedCorrectly)
                    {
                        TCGCardInfoPage.WaitUntilPageIsLoaded();
                        string codeInPage = TCGCardInfoPage.GetCode();

                        if (codeInPage == Code)
                        {
                            //Save it
                            ValidURLS.Add(url);
                        }
                    }
                    else
                    {
                        GlobalData.RecordLog("***TCG Link Page Failed to load... Review it***|");
                    }
                }

                foreach(string url in ValidURLS)
                {
                    availableUrls.Remove(url);
                }

                //Eliminate to there is only 1 urls
                if (ValidURLS.Count == 0)
                {
                    GlobalData.RecordLog("No URLS Exist for this code, review it|");
                }
                else if (ValidURLS.Count == 1)
                {
                    GlobalData.RecordLog("Single URL extracted, <---------------------------------CARD RESCUED!!|");
                    GlobalData.TCGUrlsRescued.Add(CardName + "|" + Code + "|" + ValidURLS[0]);
                    //Write out the failed manual search cards
                    File.WriteAllLines(Directory.GetCurrentDirectory() + "\\Results Data\\TCGUrlsRecued.txt", GlobalData.TCGUrlsRescued);
                    GlobalData.RecordLog("TCGUrlsRecued.txt file overwriten!!");
                }
                else
                {
                    //Eliminate until there is one left.
                    double initialPrice = 100000;
                    string winnerURL = "NONE";
                    foreach (string url in ValidURLS)
                    {
                        bool PageLoadedCorrectly = false;
                        try
                        {
                            Driver.GoToURL(url);
                            PageLoadedCorrectly = TCGCardInfoPage.IsAValidPage();
                        }
                        catch (Exception)
                        {
                            PageLoadedCorrectly = false;
                        }

                        if (PageLoadedCorrectly)
                        {
                            TCGCardInfoPage.WaitUntilPageIsLoaded();
                            string marketpricestr = TCGCardInfoPage.GetMarketPrice();
                            double marketprice = CovertPriceToDouble(marketpricestr);

                            //lowest price wins
                            if (marketprice < initialPrice)
                            {
                                winnerURL = url;
                            }
                        }
                    }

                    //Validate after
                    if (winnerURL == "NONE")
                    {
                        GlobalData.RecordLog("NONE of the valid URLS won.. review|");
                    }
                    else
                    {
                        //save the winner
                        GlobalData.RecordLog("Winner URL found, <---------------------------------CARD RESCUED!!|");
                        GlobalData.TCGUrlsRescued.Add(CardName + "|" + Code + "|" + winnerURL);
                        //Write out the failed manual search cards
                        File.WriteAllLines(Directory.GetCurrentDirectory() + "\\Results Data\\TCGUrlsRecued.txt", GlobalData.TCGUrlsRescued);
                        GlobalData.RecordLog("TCGUrlsRecued.txt file overwriten!!");
                    }
                }

                GlobalData.RecordLog("------------------------------------------------------------------");
            }

            GlobalData.RecordLog(">>>>>>>>>>>>>>>> END OF SCRIPT <<<<<<<<<<<<<<<<<<<<<<<<<<<<");
        }
        private static void TEST_TEARDOWN(CardGroup CurrentTestGroup)
        {
            //TEST TEARDOWN
            SavePostRunFiles(CurrentTestGroup);

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
        private static void LoadTCGRescueList()
        {
            //Stream that reads the actual file.
            StreamReader SR_SaveFile = new StreamReader(
                Directory.GetCurrentDirectory() + "\\MasterURLFiles\\TCGCodesWithoutURL.txt");

            //First line contains how many links are in this file
            string Line = SR_SaveFile.ReadLine();
            int LinksAmount = Convert.ToInt32(Line);

            for (int i = 0; i < LinksAmount; i++)
            {
                //Extract the line split the name and URL
                Line = SR_SaveFile.ReadLine();
                string[] tokens = Line.Split("|");

                string cardname = tokens[0];
                string code = tokens[1];

                if (CurrentDB.TCGRescueList.ContainsKey(code))
                {
                    Console.WriteLine("Duplicate code found: " + code);
                }
                else
                {
                    //Populate the Dictionary
                    CurrentDB.TCGRescueList.Add(code, cardname);
                }
            }

            SR_SaveFile.Close();

            //Log
            GlobalData.RecordLog("Load TCG Rescue List Successful!");
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
            
            //Write out the TCG URLS that failed to load
            //Write out the failed manual search cards
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\Results Data\\CardsWithoutProdeckURLS.txt", GlobalData.CardsWithoutProdeckURL);
            GlobalData.RecordLog("CardsWithoutProdeckURLS.txt file overwriten!!");

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