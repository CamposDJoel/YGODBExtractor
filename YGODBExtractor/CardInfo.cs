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
            int priceIterator = iterator + (setsAmountInt * 4);
            for(int x = 0; x < setsAmountInt; x++) 
            {
                string date = tokens[iterator];
                string code = tokens[iterator + 1];
                string name = tokens[iterator + 2];
                string rarity = tokens[iterator + 3];
                string market = tokens[priceIterator];
                string mediam = tokens[priceIterator + 1];
                _Sets.Add(new Set(date, code, name, rarity, market, mediam));
                iterator += 4;
                priceIterator += 2;
            }
        }

        public string Name
        {
            get { return _Name; }
        }
        public int SetsCount
        {
            get { return _Sets.Count; }
        }
        public List<Set> Sets { get { return _Sets; } }
        public CardInfo GetCopy()
        {
            string info = GetMasterInfoLine();
            return new CardInfo(info);
        }
        public CardInfo GetCopyWithoutSets()
        {
            string info = GetMasterInfoLine();
            CardInfo copy = new CardInfo(info);
            copy.Sets.Clear();
            return copy;
        }
        public string GetMasterInfoLine()
        {
            string line = "";

            StringBuilder sb = new StringBuilder();

            sb.Append(_ID);
            sb.Append("|");

            sb.Append(_Name);
            sb.Append("|");

            sb.Append(_Attribute);
            sb.Append("|");

            sb.Append(_Type);
            sb.Append("|");

            sb.Append(_LevelRankLink);
            sb.Append("|");

            sb.Append(_Attack);
            sb.Append("|");

            sb.Append(_Defense);
            sb.Append("|");

            sb.Append(_Pendulum);
            sb.Append("|");

            sb.Append(_Sets.Count.ToString());
            sb.Append("|");

            foreach (Set set in _Sets) 
            {
                sb.Append(set.ReleaseDate);
                sb.Append("|");
                sb.Append(set.Code);
                sb.Append("|");
                sb.Append(set.Name);
                sb.Append("|");
                sb.Append(set.Rarity);
                sb.Append("|");
            }

            foreach (Set set in _Sets)
            {
                sb.Append(set.MarketPrice);
                sb.Append("|");
                sb.Append(set.MediamPrice);
                sb.Append("|");
            }


            line = sb.ToString();
            return line;
        }
        public void AddSet(string date, string code, string name, string rarity)
        {
            _Sets.Add(new Set(date, code, name, rarity));
        }

        private string _ID = "IDMISSING";
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

        public Set(string date, string code, string name, string rarity)
        {
            _ReleaseDate = date;
            _Code = code;
            _Name = name;
            _Rarity = rarity;
        }

        public void OverridePrices(string market, string median)
        {
            _MarketPrice = market;
            _MediamPrice = median;
        }

        public string ReleaseDate { get { return _ReleaseDate; } } 
        public string Code { get { return _Code; } } 
        public string Name { get { return _Name; } } 
        public string Rarity { get { return _Rarity; } } 
        public string MarketPrice { get { return _MarketPrice; } } 
        public string MediamPrice { get { return _MediamPrice; } } 

        private string _ReleaseDate = "00/00/0000";
        private string _Code = "XXXX-EN000";
        private string _Name = "Invalid";
        private string _Rarity = "Unknown";
        private string _MarketPrice = "$0.00";
        private string _MediamPrice = "$0.00";
    }
}
