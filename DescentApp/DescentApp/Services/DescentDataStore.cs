using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DescentCore.Abilites;
using DescentCore.Dice;
using DescentCore.Units;
using DescentCore.Equipment;

namespace DescentApp.Services {
    public interface IDataStore {
        Hero Hero { get; set; }
        List<WeoponItem> MainHand { get; }
        List<ShieldItem> OffHand { get; }
        List<ArmorItem> Armor { get; }
        List<TrinketItem> Trinket { get; }
    }

    public class FileDataStore : IDataStore {
        // does not work because moves the static text files around :(
        public Hero Hero { get; set; } // TODO: heros should be an array, so you can switch hero!
        public List<WeoponItem> MainHand { get; private set; }
        public List<ShieldItem> OffHand { get; private set; }
        public List<ArmorItem> Armor { get; private set; }
        public List<TrinketItem> Trinket { get; private set; }

        public FileDataStore() {
            GearFactory gf = new GearFactory("ActI", false);
            MainHand = gf.MainHand;
            OffHand = gf.OffHand;
            Armor = gf.Armor;
            Trinket = gf.Trinket;
            var baseDefence = new DefenceDice(new GreyDie());
            Hero = new Hero("Leoric", 4, 8, 5, baseDefence, 1, 5, 3, 2, 0);

        }
    }

    public class StaticDataStore : IDataStore {
        // static data store to test that the app works
        public Hero Hero { get; set; }
        public List<WeoponItem> MainHand { get; private set; }
        public List<ShieldItem> OffHand { get; private set; }
        public List<ArmorItem> Armor { get; private set; }
        public List<TrinketItem> Trinket { get; private set; }

        public StaticDataStore() {
            var baseDefence = new DefenceDice(new GreyDie());
            Hero = new Hero("Leoric", 4, 8, 5, baseDefence, 1, 5, 3, 2, 0);
            MainHand = GetWeoponItems();
            OffHand = new List<ShieldItem> { new ShieldItem("Standard Shield", new Ability[0]) };
            Armor = new List<ArmorItem> {
                new ArmorItem("Brown Armor", new DefenceDice(new BrownDie()), 
                    ItemCatagory.LightArmor, new Ability[0]),
                new ArmorItem("Grey Armor", new DefenceDice(new GreyDie()),
                    ItemCatagory.MediumArmor, new Ability[0]),
                new ArmorItem("Black Armor", new DefenceDice(new BlackDie()), 
                    ItemCatagory.HeavyArmor, new Ability[0])
            };
            var c = new ItemCatagory[] { ItemCatagory.Ring };
            Trinket = new List<TrinketItem> {
                new TrinketItem("Pretty Ring", c, new Ability[0]),
                new TrinketItem("Ugly Ring", c, new Ability[0])
            };
        }

        private List<WeoponItem> GetWeoponItems() {
            // TODO load from file in the future!
            var catagories = new ItemCatagory[] { ItemCatagory.Staff, ItemCatagory.Magic };
            Ability[] a;

            a = new Ability[] { new Ability(2, AbilityType.Pierce, 1) };
            WeoponItem whip = new WeoponItem(
                "9 Tailed Whip", new AttackDice("blue", "power"),
                HandCatagory.MeleeWeopon, a, new ItemCatagory[] { ItemCatagory.Exotic }, 1);

            a = new Ability[] { new Ability(2, AbilityType.Pierce, 1),
                                         new Ability(2, AbilityType.Range, 1) };
            WeoponItem weakStaff = new WeoponItem(
                "Weak Staff", new AttackDice("blue", "yellow"),
                HandCatagory.RangeWeopon, a, catagories, 2);

            a = new Ability[] { new Ability(3, AbilityType.Damage, 2),
                                         new Ability(1, AbilityType.Damage, 1) };
            WeoponItem fancyStaff = new WeoponItem(
                "Fancy Staff", new AttackDice("blue", "power", "yellow"),
                HandCatagory.RangeWeopon, a, catagories, 2);

            return new List<WeoponItem> { whip, fancyStaff, weakStaff };
        }
    }
}
