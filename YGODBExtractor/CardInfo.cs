using OpenQA.Selenium.DevTools.V118.Network;
using OpenQA.Selenium.DevTools.V118.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YGODBExtractor
{
    public class CardInfo: IComparable<CardInfo>
    {
        public CardInfo() { }
        public CardInfo(string data)
        {
            string[] tokens = data.Split("|");

            _ID = Convert.ToInt32(tokens[0]);
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
        public CardInfo(string name, string attribute, string species, string levelranklink, string ata, string def, string pen, string url)
        {
            _Name = name;
            _Attribute = attribute;
            _Type = species;
            _LevelRankLink = levelranklink;
            _Attack = ata;
            _Defense = def;
            _Pendulum = pen;
            _KonamiURL = url;
        }

        //Accessors
        public string Name { get { return _Name; } set { _Name = value; } }
        public int ID { get { return _ID; } set { _ID = value; } }
        public string Attribute { get { return _Attribute; } set { _Attribute = value; } }
        public string Type { get { return _Type; } set { _Type = value; } }
        public string LevelRankLink {  get { return _LevelRankLink; } set { _LevelRankLink = value; } }
        public string Attack { get { return _Attack; } set { _Attack = value; } }
        public string Defense {  get { return _Defense; } set { _Defense = value; } }
        public string Pendulum { get { return _Pendulum; } set { _Pendulum = value; } }
        public string ProdeckURL { get { return _ProdeckURL; } set { _ProdeckURL = value; } }
        public string KonamiURL { get { return _KonamiURL; } set { _KonamiURL = value; } }
        public bool Obtained { get { return _Obtained; } set { _Obtained = value; } }       
        public List<Set> Sets { get { return _Sets; } }

        //Public functions
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
        public Set GetCopyOfSetCard(string setName)
        {
            int setIndex = -1;
            for(int i = 0; i < Sets.Count; i++)
            {
                if (Sets[i].Name == setName)
                {
                    setIndex = i; break;
                }
            }

            string releasedate = Sets[setIndex].ReleaseDate;
            string code = Sets[setIndex].Code;
            string name = Sets[setIndex].Name;
            string rarity = Sets[setIndex].Rarity;
            string market = Sets[setIndex].MarketPrice;
            string median = Sets[setIndex].MediamPrice;
            bool obtained = Sets[setIndex].Obtained;
            string url = Sets[setIndex].TCGPlayerURL;

            return new Set(releasedate, code, name, rarity, market, median, obtained, url);
            
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
        public void SetCardID(int id)
        {
            _ID = id;
        }
        public int GetSetsCount()
        {
            return _Sets.Count;
        }
        public void AddSet(string date, string code, string name, string rarity)
        {
            //_Sets.Insert(0, new Set(date, code, name, rarity));
            _Sets.Add(new Set(date, code, name, rarity));
        }
        public void AddProdeckURL(string url)
        {
            _ProdeckURL = url;
        }
        public void AddKonamiURL(string url)
        {
            _KonamiURL = url;
        }
        public bool HasProdeckURLAvailable()
        {
            return _ProdeckURL != "Missing" && _ProdeckURL != "Known Missing";
        }
        public bool IsProdeckURLKnownMissing()
        {
            return _ProdeckURL == "Known Missing";
        }
        public bool IsProdeckURLMissing()
        {
            return _ProdeckURL == "Missing";
        }

        public int CompareTo(CardInfo? other)
        {           
            return Name.CompareTo(other.Name);
        }

        public class SortByName : IComparer<CardInfo>
        {
            public int Compare(CardInfo c1, CardInfo c2)
            {
                string card1tag = c1.Name;
                string card2tag = c2.Name;
                return String.Compare(card1tag, card2tag);
            }
        }

        private int _ID = -1;
        private string _Name;
        private string _Attribute;
        private string _Type;
        private string _LevelRankLink;
        private string _Attack;
        private string _Defense;
        private string _Pendulum;
        private string _KonamiURL = "Missing";
        private string _ProdeckURL = "Missing";
        private bool _Obtained = false;
        private List<Set> _Sets = new List<Set>();
    }

    public class Set
    {
        public Set() { }
        public Set(string date, string code, string name, string rarity, string market, string medium, bool obtained, string url)
        {
            _ReleaseDate = date;
            _Code = code;
            _Name = name;
            _Rarity = rarity;
            _MarketPrice = market;
            _MediamPrice = medium;
            _Obtained = obtained;
            _TCGPlayerURL = url;
        }
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
        public void AddTCGUrl(string url)
        {
            _TCGPlayerURL= url;
        }

        public bool HasTCGURLAvailable()
        {
            return _TCGPlayerURL != "Missing" && _TCGPlayerURL != "Known Missing";
        }
        public bool IsTCGPlayerURLKnownMissing()
        {
            return _TCGPlayerURL == "Known Missing";
        }
        public bool IsTCGPlayerURLMissing()
        {
            return _TCGPlayerURL == "Missing";
        }

        public string ReleaseDate { get { return _ReleaseDate; } set { _ReleaseDate = value; } } 
        public string Code { get { return _Code; } set { _Code = value; } } 
        public string Name { get { return _Name; } set { _Name = value; } } 
        public string Rarity { get { return _Rarity; } set { _Rarity = value; } } 
        public string MarketPrice { get { return _MarketPrice; } set { _MarketPrice = value; } } 
        public string MediamPrice { get { return _MediamPrice; } set { _MediamPrice = value; } } 
        public bool Obtained {  get { return _Obtained; } set { _Obtained = value; } }
        public string TCGPlayerURL { get { return _TCGPlayerURL; } set { _TCGPlayerURL = value; } }

        private string _ReleaseDate = "00/00/0000";
        private string _Code = "XXXX-EN000";
        private string _Name = "Invalid";
        private string _Rarity = "Unknown";
        private string _MarketPrice = "$0.00";
        private string _MediamPrice = "$0.00";
        private bool _Obtained = false;
        private string _TCGPlayerURL = "Missing";
    }
}
