﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public static class CurrentDB
    {
        public static List<CardInfo> CardInfoList = new List<CardInfo>();
        public static List<string> CardNamesList = new List<string>();

        public static Dictionary<string, string> ProdeckURLs = new Dictionary<string, string>();
        public static Dictionary<string, string> TCGPlayerURLs = new Dictionary<string, string>();

        public static CardInfo GetCard(string name)
        {
            int index = CardNamesList.IndexOf(name);
            return CardInfoList[index];
        }
        public static bool CardExist(string cardName)
        {
            return CardNamesList.Contains(cardName);
        }

        public static bool ProdeckURLExist(string cardName)
        {
            return ProdeckURLs.ContainsKey(cardName);
        }
        public static string GetProdeckURL(string cardName) 
        {
            return ProdeckURLs[cardName];
        }
    }
}