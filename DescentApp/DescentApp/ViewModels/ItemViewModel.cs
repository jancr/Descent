using System.Collections.Generic;
using System.ComponentModel;

using DescentCore.Equipment;
using DescentCore.Units;

namespace DescentApp.ViewModels
{
    public abstract class ItemViewModel<T> : INotifyPropertyChanged where T : Item {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, T> nameToItem = new Dictionary<string, T>();
        private Dictionary<T, string> itemToName = new Dictionary<T, string>();
        //private delegate T getEquipment();
        //private Func<T> getEquipment;
        protected Hero hero;
        private int selectedIndex = -1;

        //public ItemViewModel(Func<T> equipmentSlot, T[] items, Hero hero) 
        public ItemViewModel(Hero hero, T[] items) {
            //getEquipment = equipmentSlot;
            this.hero = hero;
            //nameToItem.Add("No Item", null);
            //itemToName.Add(null, "No Item");
            ItemNames.Add("No Item");
            T currentItem = getEquipment();
            foreach (T item in items) {
                if ((currentItem != null) && (currentItem == item)) {
                    SelectedIndex = ItemNames.Count;
                }
                ItemNames.Add(item.Name);
                nameToItem.Add(item.Name, item);
                itemToName.Add(item, item.Name);
            }
        }

        // Binding Propperties
        public List<string> ItemNames { get; private set; } = new List<string>();
        public int SelectedIndex {
            get {
                return selectedIndex;
            }
            set {
                if (selectedIndex != value) {
                    selectedIndex = value;

                    T oldItem = getEquipment();
                    if (oldItem != null) {
                        hero.UnEquip(oldItem);
                    }
                    string newItemName = ItemNames[selectedIndex];
                    if (newItemName != "No Item") { 
                        T newItem = nameToItem[newItemName];
                        hero.Equip(newItem);
                    }
                }
            }
        }
        protected abstract T getEquipment();
    }

    public class MainHandItemViewModel : ItemViewModel<WeoponItem> {
        public MainHandItemViewModel(Hero hero, WeoponItem[] items) : base(hero, items) { }
        protected override WeoponItem getEquipment() { return hero.Equipment.MainHand; }
    }

    public class OffHandItemViewModel : ItemViewModel<ShieldItem> {
        public OffHandItemViewModel(Hero hero, ShieldItem[] items) : base(hero, items) { }
        protected override ShieldItem getEquipment() { return hero.Equipment.OffHand; }
    }

    public class ArmorItemViewModel : ItemViewModel<ArmorItem> {
        public ArmorItemViewModel(Hero hero, ArmorItem[] items) : base(hero, items) { }
        protected override ArmorItem getEquipment() { return hero.Equipment.Armor; }
    }

    public class TrinketItemViewModel : ItemViewModel<TrinketItem> {
        public int number;
        public TrinketItemViewModel(Hero hero, TrinketItem[] items, int trinketNumber) : base(hero, items) {
            this.number = trinketNumber;
        }
        protected override TrinketItem getEquipment() {
            if (hero.Equipment.Trinket.Count >= number) {
                return hero.Equipment.Trinket[0];
            }
            return null;
        }
    }
}
