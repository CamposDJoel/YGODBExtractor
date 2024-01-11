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

            //Open CSI Login Page
            Driver.GoToURL("http://www.google.com");

            //Wait for the Login Page to load
            //LoginPage.WaitUntilPageIsLoaded();

            //Fillout login info
            //Element.InputText("username", GlobalData.AdminEmail);
            //Element.InputText("password", GlobalData.MasterPassword);



            //
            //Cdriver.Navigate().GoToUrl(UAT_URL);
            //Cdriver.FindElement(By.Id("username")).SendKeys(AdminEmail);
        }
    }
}
