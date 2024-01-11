using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    internal class GlobalData
    {
        //The actual Web Driver must be global to be accessible everywhere
        public static IWebDriver? Chrome;
    }
}
