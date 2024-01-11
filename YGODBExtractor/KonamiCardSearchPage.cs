using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YGODBExtractor
{
    public static class KonamiCardSearchPage
    {
        //Common element IDs
        public static string ID_SearchButton = "submit_area";
        public static string ID_GroupBanner = "ctype_set";
        public static string ID_CookiesAcceptButton = "onetrust-accept-btn-handler";
        
        
        public static void WaitUntilPageIsLoaded()
        {
            Element.WaitUntilElementIsVisble(ID_SearchButton);
            Element.WaitUntilElementIsVisble(ID_GroupBanner);
        }

        public static void AcceptCookiesBanner()
        {
            //Try to accept cockies
            try
            {
                Element.Click(ID_CookiesAcceptButton);
            }
            catch (Exception)
            {
                GlobalData.Log.Add("No Cookies Banner was displayed. It wasnt clear out.");
            }
        }
    }
}
