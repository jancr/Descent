using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Xamarin.Forms;

using DescentCore.Equipment;
using DescentCore.Units;

//using System.Diagnostics;
using System.Runtime.CompilerServices;

using DescentCore.Abillites;
using DescentCore.Dice;
using DescentCore.Equipment;

using DescentApp.Services;

namespace DescentApp.ViewModels
{
    public abstract class ItemViewModel<T> : INotifyPropertyChanged where T : Item {
        // INotifyPropertyChanged Interface event
        public event PropertyChangedEventHandler PropertyChanged;

        // propperty backing fields
        private ObservableCollection<string> _itemNames = new ObservableCollection<string>();
        private int selectedIndex = -1;
        protected Hero hero = DependencyService.Get<IDataStore>().Hero;
        //private T items;

        // abstract methods
        protected abstract List<T> GetItems(); 
        protected abstract T GetEquipedItem();


        // private helper fields
        protected Dictionary<string, T> nameToItem = new Dictionary<string, T>();
        protected Dictionary<T, string> itemToName = new Dictionary<T, string>();

        public ItemViewModel() {
            ItemNames.Add("No Item");
            T currentItem = GetEquipedItem();
            //WeoponItem[] items = DependencyService.Get<IDataStore>().MainHand.GetItemsAsync(true); 
            List<T> items = GetItems();
            foreach (T item in items) {
                ItemNames.Add(item.Name);
                nameToItem.Add(item.Name, item);
                itemToName.Add(item, item.Name);
            }
            if (currentItem != null) {
                for (int i = 1; i < ItemNames.Count; i++ ) {
                    if (currentItem.Name == ItemNames[i]) {
                        SelectedIndex = i;
                    }
                }
            } else {
                SelectedIndex = 0;
            }
        }


        // Binding Propperties
        public ObservableCollection<string> ItemNames {
            get {
                return _itemNames;
            }
            private set {
                if (!Equals(value, _itemNames)) {
                    _itemNames= value;
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

                    T oldItem = GetEquipedItem();
                    if (oldItem != null) {
                        hero.UnEquip(oldItem);
                    }
                    string newItemName = ItemNames[selectedIndex];
                    if (newItemName != "No Item") { 
                        T newItem = nameToItem[newItemName];
                        hero.Equip(newItem);
                    }
                    OnPropertyChanged(nameof(SelectedIndex));
                }
            }
        }
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class MainHandItemViewModel : ItemViewModel<WeoponItem> {
        protected override WeoponItem GetEquipedItem() => hero.Equipment.MainHand; 
        protected override List<WeoponItem> GetItems() => DependencyService.Get<IDataStore>().MainHand;
    }

    public class OffHandItemViewModel : ItemViewModel<ShieldItem> {
        protected override ShieldItem GetEquipedItem() => hero.Equipment.OffHand; 
        protected override List<ShieldItem> GetItems() => DependencyService.Get<IDataStore>().OffHand;
    }

    public class ArmorItemViewModel : ItemViewModel<ArmorItem> {
        protected override ArmorItem GetEquipedItem() => hero.Equipment.Armor; 
        protected override List<ArmorItem> GetItems() => DependencyService.Get<IDataStore>().Armor;
    }

    // Trinket Abstract ViewModel and Subclasses
    public class Trinket1ItemViewModel : ItemViewModel<TrinketItem> {
        protected override List<TrinketItem> GetItems() => DependencyService.Get<IDataStore>().Trinket;
        protected override TrinketItem GetEquipedItem() {
            if (hero.Equipment.Trinket.Count >= 1) {
                // TODO: there may also be a bug here
                return hero.Equipment.Trinket[0];
            }
            return null;
        }
    }

    public class Trinket2ItemViewModel : ItemViewModel<TrinketItem> {
        protected override List<TrinketItem> GetItems() => DependencyService.Get<IDataStore>().Trinket;
        protected override TrinketItem GetEquipedItem() {
            if (hero.Equipment.Trinket.Count == 2) {
                // TODO: there is a bug here, if you unequip trinket 1, then
                // count will be 1, and thus null will be returned
                return hero.Equipment.Trinket[1];
            }
            return null;
        }
    }
    //public class Trinket1ItemViewModel : TrinketItemViewModel {
    //    protected override int TrinketNumber { get { return 1; } }
    //}
    //public class Trinket2ItemViewModel : TrinketItemViewModel {
    //    protected override int TrinketNumber { get { return 2; } }
    //}
}
