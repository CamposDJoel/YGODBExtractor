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

            
        }
    }
}
