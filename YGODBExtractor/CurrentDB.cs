﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public static class CurrentDB
    {
        public static Dictionary<string, CardInfo> MasterDB = new Dictionary<string, CardInfo>();
        public static List<CardInfo> CardInfoList = new List<CardInfo>();

        public static List<string> CardInfoInTxt = new List<string>();

        public static Dictionary<string, string> ProdeckURLs = new Dictionary<string, string>();
        public static Dictionary<string, string> TCGPlayerURLs = new Dictionary<string, string>();
        public static Dictionary<string, string> TCGRescueList = new Dictionary<string, string>();


        public static Dictionary<string, string> KnownMissingTCGURLsList = new Dictionary<string, string>();
        public static List<string> KnownMissingProdeckURLsList = new List<string>();

        public static void AddNewCard(CardInfo card)
        {
            CardInfoList.Add(card);
            MasterDB.Add(card.Name, card);

            //gotta sort the list
            CardInfoList.Sort(new CardInfo.SortByName());
        }
        public static CardInfo GetCard(string name)
        {
            return MasterDB[name];
        }
        public static bool CardExist(string cardName)
        {
            return MasterDB.ContainsKey(cardName);
        }

        public static bool ProdeckURLExist(string cardName)
        {
            return ProdeckURLs.ContainsKey(cardName);
        }
        public static string GetProdeckURL(string cardName) 
        {
            return ProdeckURLs[cardName];
        }
        public static bool ProdeckURLIsKnownMissing(string cardName)
        {
            return KnownMissingProdeckURLsList.Contains(cardName);
        }

        public static bool TCGPlayerURLExistg(string code)
        {
            return TCGPlayerURLs.ContainsKey(code);
        }
        public static string GetTCGPlayerURL(string code)
        {
            return TCGPlayerURLs[code];
        }
        public static bool TCGURLIsKnownMissing(string code) 
        {
            return KnownMissingTCGURLsList.ContainsKey(code);
        }
    }
}