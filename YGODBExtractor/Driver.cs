using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    internal class Driver
    {
        /// <summary>
        /// Initializes the Chrome Driver. (Browser will open and maximize the window).
        /// </summary>
        public static void OpenBrowser()
        {
            //this "option" will allow the browser to Maximize opun launching
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            GlobalData.Chrome = new ChromeDriver(options);
        }

        /// <summary>
        /// Navigates to a specific URL
        /// </summary>
        /// <param name="url">Designated URL</param>
        public static void GoToURL(string url)
        {
            GlobalData.Chrome.Navigate().GoToUrl(url);
            ((IJavaScriptExecutor)GlobalData.Chrome).ExecuteScript("window.resizeTo(1024, 768);");
        }
    }
}
