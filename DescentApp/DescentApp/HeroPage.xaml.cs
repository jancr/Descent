using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DescentCore.Equipment;
using DescentCore.Units;
using DescentCore.Abillites;
using DescentCore.Dice;

namespace DescentApp {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HeroPage : ContentPage {
        static Hero hero; // TODO bind the correct way!
        private Dictionary<string, HandItem> nameToMainHand;

		public HeroPage () {
            InitializeComponent();
            var defence = new DefenceDice(new GreyDie());
            hero = new Hero("Leoric", 4, 8, 5, defence, 1, 5, 3, 2, 0);

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

            var emptyMainHand = new WeoponItem(
                "No Item", new AttackDice(), HandCatagory.MeleeWeopon,
                new Abillity[0], new ItemCatagory[0], 1);

            nameToMainHand = new Dictionary<string, HandItem> {
                { emptyMainHand.Name, emptyMainHand }, { weakStaff.Name, weakStaff },
                { fancyStaff.Name, fancyStaff }
            };

            foreach (string weoponName in nameToMainHand.Keys) {
                // there is a bug here ?!?!?
                mainHandPicker.Items.Add(weoponName);
            }

		}

        void OnChangeMainHand(object sender, EventArgs e) {
            // TODO: exceptions if equiping 2H weopon and shield
            HandItem newWeoponItem = nameToMainHand[mainHandPicker.Items[mainHandPicker.SelectedIndex]];
            HandItem oldWeoponItem = hero.Equipment.MainHand;
            if (oldWeoponItem != null) {
                hero.UnEquip(oldWeoponItem);
            }
            hero.Equip(newWeoponItem);
        }
	}
}