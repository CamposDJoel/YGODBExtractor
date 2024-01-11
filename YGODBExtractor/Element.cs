using OpenQA.Selenium;
using System;
using System.Collections.Generic;
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

        public static void ClickByID(string elementID) 
        {
            GlobalData.Chrome.FindElement(By.Id(elementID)).Click();
        }
        public static void ClickByXpath(string xpath)
        {
            GlobalData.Chrome.FindElement(By.XPath(xpath)).Click();
        }

        public static string GetText(string xpath)
        {
            return GlobalData.Chrome.FindElement(By.XPath(xpath)).Text;
        }
    }
}