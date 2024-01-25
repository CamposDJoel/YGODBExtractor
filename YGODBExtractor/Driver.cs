using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public static class Driver
    {
        /// <summary>
        /// Initializes the Chrome Driver. (Browser will open and maximize the window).
        /// </summary>
        public static void OpenBrowser()
        {
            //this "option" will allow the browser to Maximize opun launching
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            //options.EnableMobileEmulation(deviceName);
            options.AddArgument("no-sandbox");

            ChromeDriver drv = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            drv.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));

            //GlobalData.Chrome = new ChromeDriver(options);
            GlobalData.Chrome = drv;
        }

        /// <summary>
        /// Navigates to a specific URL
        /// </summary>
        /// <param name="url">Designated URL</param>
        public static void GoToURL(string url)
        {
            try 
            {
                GlobalData.Chrome.Navigate().GoToUrl(url);
                ((IJavaScriptExecutor)GlobalData.Chrome).ExecuteScript("window.resizeTo(1024, 768);");
            }
            catch(Exception)
            {
                Console.WriteLine("");
                Console.WriteLine("CHROME DRIVER FAILED - Closing and reopening again...");
                Console.WriteLine("");
                GlobalData.Chrome.Quit();
                OpenBrowser();

                GlobalData.Chrome.Navigate().GoToUrl(url);
                ((IJavaScriptExecutor)GlobalData.Chrome).ExecuteScript("window.resizeTo(1024, 768);");
            }
            
        }
    }
}
