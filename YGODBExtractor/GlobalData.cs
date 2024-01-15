using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace YGODBExtractor
{
    public static class GlobalData
    {
        //The actual Web Driver must be global to be accessible everywhere
        public static IWebDriver Chrome;

        //ULRS
        public static string KonamiDB_URL = "https://www.db.yugioh-card.com/yugiohdb/card_search.action";
        public static string Prodeck_URL = "https://ygoprodeck.com/card-database/?&num=24&offset=0";

        public static List<string> CardsThatFailedManualSearch = new List<string>();
        public static List<string> CodesWithoutTCGLink = new List<string>();

        //Loggin
        public static List<string> Log = new List<string>();

        public static string CardGroupToString(CardGroup cardGroup)
        {
            switch(cardGroup) 
            {
                case CardGroup.Aqua_Monsters: return "Aqua";
                case CardGroup.Beast_Monsters: return "Beast";
                case CardGroup.BeastWarrior_Monsters: return "Beast-Warrior";
                case CardGroup.Cyberse_Monsters: return "Cyberse";
                case CardGroup.Dinosaur_Monsters: return "Dinosaur";
                case CardGroup.DivineBeast_Monsters: return "Divine-Beast";
                case CardGroup.Dragon_Monsters: return "Dragon";
                case CardGroup.Fairy_Monsters: return "Fairy";
                case CardGroup.Fiend_Monsters: return "Fiend";
                case CardGroup.Fish_Monsters: return "Fish";
                case CardGroup.Insect_Monsters: return "Insect";
                case CardGroup.Machine_Monsters: return "Machine";
                case CardGroup.Plant_Monsters: return "Plant";
                case CardGroup.Psychic_Monsters: return "Psychic";
                case CardGroup.Pyro_Monsters: return "Pyro";
                case CardGroup.Reptile_Monsters: return "Reptile";
                case CardGroup.Rock_Monsters: return "Rock";
                case CardGroup.SeaSerpent_Monsters: return "Sea Serpent";
                case CardGroup.Spellcaster_Monsters: return "Spellcaster";
                case CardGroup.Thunder_Monsters: return "Thunder";
                case CardGroup.Warrior_Monsters: return "Warrior";
                case CardGroup.WingedBeast_Monsters: return "Winged Beast";
                case CardGroup.Wyrm_Monsters: return "Wyrm";
                case CardGroup.Zombie_Monsters: return "Zombie";
                default: return "NONE";
            }
        }
        public static void RecordLog(string message)
        {
            Console.WriteLine(message);
            Log.Add(message);
        }
    }
}
