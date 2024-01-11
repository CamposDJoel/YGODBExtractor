using OpenQA.Selenium.DevTools.V118.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public class CardInfo
    {
        public CardInfo(string data)
        {
            string[] tokens = data.Split("|");

            _ID = tokens[0];
            _Name = tokens[1];
            _Attribute = tokens[2];
            _Type = tokens[3];
            _LevelRankLink = tokens[4];
            _Attack = tokens[5];
            _Defense = tokens[6];
            _Pendulum = tokens[7];
            string setsAmount = tokens[8];
            int setsAmountInt = Convert.ToInt32(setsAmount);

            int iterator = 9;
            for(int x = 0; x < setsAmountInt; x++) 
            {
                string date = tokens[iterator];
                string code = tokens[iterator + 1];
                string name = tokens[iterator + 2];
                string rarity = tokens[iterator + 3];
                string market = tokens[iterator + 4];
                string mediam = tokens[iterator + 5];
                _Sets.Add(new Set(date, code, name, rarity, market, mediam));
            }
        }

        public int SetsCount
        {
            get { return _Sets.Count; }
        }

        private string _ID;
        private string _Name;
        private string _Attribute;
        private string _Type;
        private string _LevelRankLink;
        private string _Attack;
        private string _Defense;
        private string _Pendulum;
        private List<Set> _Sets = new List<Set>();
    }

    public class Set
    {
        public Set(string date, string code, string name, string rarity, string market, string medium) 
        {
            _ReleaseDate = date;
            _Code = code;
            _Name = name;
            _Rarity = rarity;
            _MarketPrice = market;
            _MediamPrice = medium;
        }

        private string _ReleaseDate = "00/00/0000";
        private string _Code = "XXXX-EN000";
        private string _Name = "Invalid";
        private string _Rarity = "Unknown";
        private string _MarketPrice = "$0.00";
        private string _MediamPrice = "$0.00";
    }
}
