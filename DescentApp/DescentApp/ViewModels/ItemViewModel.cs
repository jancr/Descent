
// core imports
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

// 3rd party imports
using Xamarin.Forms;

// local imports
using DescentCore.Equipment;
using DescentCore.Units;
using DescentApp.Services;

namespace DescentApp.ViewModels {
    public abstract class ItemViewModel<T> : INotifyPropertyChanged 
                where T : Item {
        // INotifyPropertyChanged Interface event
        public event PropertyChangedEventHandler PropertyChanged;

        // propperty backing fields
        private ObservableCollection<string> _itemNames = 
            new ObservableCollection<string>();
        private int selectedIndex = -1;
        protected Hero hero = DependencyService.Get<IDataStore>().Hero;
        //private T items;

        // abstract methods
        protected abstract List<T> GetItems();
        protected abstract T GetEquipedItem();

        // protected helper fields
        protected Dictionary<string, T> nameToItem = new Dictionary<string, T>();
        protected Dictionary<T, string> itemToName = new Dictionary<T, string>();
        protected T equipedItem;
        private bool setupFlag = true;

        public ItemViewModel() {
            ItemNames.Add("No Item");
            T equipedItem = GetEquipedItem();
            List<T> items = GetItems();
            foreach (T item in items) {
                ItemNames.Add(item.Name);
                nameToItem.Add(item.Name, item);
                itemToName.Add(item, item.Name);
            }
            if (equipedItem != null) {
                for (int i = 1; i < ItemNames.Count; i++) {
                    if (equipedItem.Name == ItemNames[i]) {
                        SelectedIndex = i;
                    }
                }
            } else {
                SelectedIndex = 0;
            }
            setupFlag = false;
        }

        // Binding Propperties
        public ObservableCollection<string> ItemNames {
            get {
                return _itemNames;
            }
            private set {
                if (!Equals(value, _itemNames)) {
                    _itemNames = value;
                    OnPropertyChanged(nameof(ItemNames));
                }
            }
        }

        public int SelectedIndex {
            get {
                return selectedIndex;
            }
            set {
                if (selectedIndex != value) {
                    selectedIndex = value;
                    SwitchItem(selectedIndex);
                    OnPropertyChanged(nameof(SelectedIndex));
                }
            }
        }

        private void SwitchItem(int newIndex) {
            if (!setupFlag) {
                // remove old item
                if (equipedItem != null) {
                    hero.UnEquip(equipedItem);
                }
                // add new item
                string newItemName = ItemNames[newIndex];
                if (newItemName != "No Item") {
                    T equipedItem = nameToItem[newItemName];
                    hero.Equip(equipedItem);
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MainHandItemViewModel : ItemViewModel<WeoponItem> {
        protected override WeoponItem GetEquipedItem() => hero.Equipment.MainHand;
        protected override List<WeoponItem> GetItems() => 
            DependencyService.Get<IDataStore>().MainHand;
    }

    public class OffHandItemViewModel : ItemViewModel<ShieldItem> {
        protected override ShieldItem GetEquipedItem() => hero.Equipment.OffHand;
        protected override List<ShieldItem> GetItems() => 
            DependencyService.Get<IDataStore>().OffHand;
    }

    public class ArmorItemViewModel : ItemViewModel<ArmorItem> {
        protected override ArmorItem GetEquipedItem() => hero.Equipment.Armor;
        protected override List<ArmorItem> GetItems() =>
            DependencyService.Get<IDataStore>().Armor;
    }

    // Trinket Abstract ViewModel and Subclasses
    public class Trinket1ItemViewModel : ItemViewModel<TrinketItem> {
        protected override List<TrinketItem> GetItems() =>
            DependencyService.Get<IDataStore>().Trinket;
        protected override TrinketItem GetEquipedItem() {
            if (hero.Equipment.Trinket.Count >= 1) {
                return hero.Equipment.Trinket[0];
            }
            return null;
        }
    }

    public class Trinket2ItemViewModel : ItemViewModel<TrinketItem> {
        protected override List<TrinketItem> GetItems() =>
            DependencyService.Get<IDataStore>().Trinket;
        protected override TrinketItem GetEquipedItem() {
            if (hero.Equipment.Trinket.Count == 2) {
                return hero.Equipment.Trinket[1];
            }
            return null;
        }
    }
}
