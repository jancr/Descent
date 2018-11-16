using System;
using System.Collections.Generic;
using System.ComponentModel;

using DescentCore.Equipment;
using DescentCore.Units;
using DescentCore.Abillites;
using DescentCore.Dice;

namespace DescentApp.ViewModels {

    public class HeroViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        static Hero hero; // TODO bind the correct way!
        public MainHandItemViewModel mainHandItem;
        public OffHandItemViewModel offHandItem;
        public ArmorItemViewModel armorItem;
        public TrinketItemViewModel trinketItem;

        // Constructor
        public HeroViewModel() {
            hero = loadHero();

            mainHandItem = new MainHandItemViewModel(hero, getWeopons());
            //offHandItem = new OffHandItemViewModel(hero, getShields());
            // TODO armor and trinkets
        }
        private Hero loadHero() {
            var defence = new DefenceDice(new GreyDie());
            return new Hero("Leoric", 4, 8, 5, defence, 1, 5, 3, 2, 0);
        }

        private ShieldItem[] getShields() {
            // shields usally trigger +1 defence if used for blocking  but the
            // game implementation only uses the attackers abillities to modify
            // the dice so the test shields have no abillities
            // TODO load from file in the future!
            return new ShieldItem[] { new ShieldItem("Standard Shield", new Abillity[0]) };
        }

        private WeoponItem[] getWeopons() {
            // TODO load from file in the future!
            var catagories = new ItemCatagory[] { ItemCatagory.Staff, ItemCatagory.Magic };
            Abillity[] a;

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

            return new WeoponItem[] { fancyStaff, weakStaff };
        }
    }

    //    public int MainHandSelectedIndex {
    //        get {
    //            return mainHandSelectedIndex;
    //        }
    //        set {
    //            if (mainHandSelectedIndex != value) {
    //                mainHandSelectedIndex = value;

    //                // trigger some action to take such as updating other labels or fields
    //                //OnPropertyChanged(nameof(CountriesSelectedIndex));
                    
    //                HandItem newWeoponItem = nameToMainHand[MainHandItems[mainHandSelectedIndex]];
    //                HandItem oldWeoponItem = hero.Equipment.MainHand;
              
    //                if (oldWeoponItem != null) {
    //                    hero.UnEquip(oldWeoponItem);
    //                }
    //                hero.Equip(newWeoponItem);
    //            }
    //        }
    //    }
    //}
}