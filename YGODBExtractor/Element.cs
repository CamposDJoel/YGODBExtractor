using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public static class Element
    {
        /// <summary>
        /// Waits 2 seconds and verifies element can be found (is visible)
        /// </summary>
        /// <param name="elementID">Designated element ID</param>
        public static void WaitUntilElementIsVisble(string xpath)
        {
            GlobalData.Chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            GlobalData.Chrome.FindElement(By.XPath(xpath));
        }

        /// <summary>
        /// Inputs text into an element.
        /// </summary>
        /// <param name="elementID">Designeted element ID.</param>
        /// <param name="text">The text to be inputted.</param>
        public static void InputText(string elementID, string text)
        {
            GlobalData.Chrome.FindElement(By.Id(elementID)).SendKeys(text);
        }
        public static void ClickByXpath(string xpath)
        {
            GlobalData.Chrome.FindElement(By.XPath(xpath)).Click();
        }
        public static string GetText(string xpath)
        {
            return GlobalData.Chrome.FindElement(By.XPath(xpath)).Text;
        }
        public static int GetElementCount(string xpath)
        {
            return GlobalData.Chrome.FindElements(By.XPath(xpath)).Count;
        }
        public static string GetElementAttribute(string  xpath, string attributeName) 
        {
            return GlobalData.Chrome.FindElement(By.XPath(xpath)).GetAttribute(attributeName);
        }
        public static IWebElement ScrollToView(string xpath)
        {
            var element = GlobalData.Chrome.FindElement(By.XPath(xpath));
            ScrollToView(element);
            return element;

            static void ScrollToView(IWebElement element)
            {
                if (element.Location.Y > 200)
                {
                    ScrollTo(0, element.Location.Y - 100); // Make sure element is in the view but below the top navigation pane
                }

            }

            static void ScrollTo(int xPosition = 0, int yPosition = 0)
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)GlobalData.Chrome; js.ExecuteScript(String.Format("window.scrollTo({0}, {1})", xPosition, yPosition));
            }
        }    
    }
}