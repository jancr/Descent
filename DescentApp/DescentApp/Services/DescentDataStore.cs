using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DescentCore.Abillites;
using DescentCore.Dice;
using DescentCore.Units;
using DescentCore.Equipment;

namespace DescentApp.Services {
    //public interface IItemStore<T> {
    //    Task<bool> AddItemAsync(T item);
    //    //Task<bool> UpdateItemAsync(T item);
    //    //Task<bool> DeleteItemAsync(string id);
    //    //Task<T> GetItemAsync(string id);
    //    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    //}

    //public class ItemStore<T> : IItemStore<T> {
    //    public List<T> items;
    //    public ItemStore() {
    //    }
    //    public async Task<bool> AddItemAsync(T item) {
    //        items.Add(item);
    //        return await Task.FromResult(true);
    //    }
    //    public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false) {
    //        return await Task.FromResult(items);
    //    }
    //}


    //public class MainHandStore : ItemStore<WeoponItem> {
    //    public MainHandStore() {
    //        // TODO load from file in the future!
    //        var catagories = new ItemCatagory[] { ItemCatagory.Staff, ItemCatagory.Magic };
    //        Abillity[] a;

    //        a = new Abillity[] { new Abillity(2, AbillityType.Pierce, 1),
    //                                     new Abillity(2, AbillityType.Range, 1) };
    //        WeoponItem weakStaff = new WeoponItem(
    //            "Weak Staff", new AttackDice("blue", "yellow"),
    //            HandCatagory.RangeWeopon, a, catagories, 2);

    //        a = new Abillity[] { new Abillity(3, AbillityType.Damage, 2),
    //                                     new Abillity(1, AbillityType.Damage, 1) };
    //        WeoponItem fancyStaff = new WeoponItem(
    //            "Fancy Staff", new AttackDice("blue", "power", "yellow"),
    //            HandCatagory.RangeWeopon, a, catagories, 2);

    //       items = new List<WeoponItem> { fancyStaff, weakStaff };
    //    }
    //}
    //public class DataStore<T> : IDataStore<T> {
    public interface IDataStore {
        Hero Hero { get; set; }
        List<WeoponItem> MainHand { get; }
        List<ShieldItem> OffHand { get; }
        List<ArmorItem> Armor { get; }
        List<TrinketItem> Trinket { get; }
    }

    public class FileDataStore : IDataStore {
        public Hero Hero { get; set; } // TODO: heros should be an array, so you can switch hero!
        public List<WeoponItem> MainHand { get; private set; }
        public List<ShieldItem> OffHand { get; private set; }
        public List<ArmorItem> Armor { get; private set; }
        public List<TrinketItem> Trinket { get; private set; }

        public FileDataStore() {
            var baseDefence = new DefenceDice(new GreyDie());
            Hero = new Hero("Leoric", 4, 8, 5, baseDefence, 1, 5, 3, 2, 0);

            MainHand = GetWeoponItems();
            OffHand = new List<ShieldItem> { new ShieldItem("Standard Shield", new Abillity[0]) };
            Armor = new List<ArmorItem> {
                new ArmorItem("Brown Armor", new DefenceDice(new BrownDie()), ItemCatagory.LightArmor, new Abillity[0]),
                new ArmorItem("Grey Armor", new DefenceDice(new GreyDie()), ItemCatagory.MediumArmor, new Abillity[0]),
                new ArmorItem("Black Armor", new DefenceDice(new BlackDie()), ItemCatagory.HeavyArmor, new Abillity[0])
            };
            Trinket = new List<TrinketItem> { new TrinketItem("Pretty Ring", new Abillity[0]),
                                              new TrinketItem("Ugly Ring", new Abillity[0]) };
        }

        //public Hero Hero {
        //    get { return hero; }
        //    set { hero = value; }
        //}
        //public List<WeoponItem> MainHand { get { return mainHand; } private set { mainHand = value; }
        //}

        //public List<ShieldItem> OffHand {
        //    get { return offHand; }
        //    private set { offHand = value; }
        //}

        //public List<ShieldItem> Armor {
        //    get { return armor; }
        //    private set { armor = value; }
        //}

        private List<WeoponItem> GetWeoponItems() {
            // TODO load from file in the future!
            var catagories = new ItemCatagory[] { ItemCatagory.Staff, ItemCatagory.Magic };
            Abillity[] a;

            a = new Abillity[] { new Abillity(2, AbillityType.Pierce, 1) };
            WeoponItem whip = new WeoponItem(
                "9 Tailed Whip", new AttackDice("blue", "power"),
                HandCatagory.MeleeWeopon, a, new ItemCatagory[] { ItemCatagory.Exotic }, 1);

            a = new Abillity[] { new Abillity(2, AbillityType.Pierce, 1),
                                         new Abillity(2, AbillityType.Range, 1) };
            WeoponItem weakStaff = new WeoponItem(
                "Weak Staff", new AttackDice("blue", "yellow"),
                HandCatagory.RangeWeopon, a, catagories, 2);

            a = new Abillity[] { new Abillity(3, AbillityType.Damage, 2),
                                         new Abillity(1, AbillityType.Damage, 1) };
            WeoponItem fancyStaff = new WeoponItem(
                "Fancy Staff", new AttackDice("blue", "power", "yellow"),
                HandCatagory.RangeWeopon, a, catagories, 2);

           return new List<WeoponItem> { whip, fancyStaff, weakStaff };
        }
    }
}