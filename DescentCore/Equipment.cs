// core imports
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// local imports
using DescentCore.Dice;
using DescentCore.Abilites;
using DescentCore.Exceptions;

namespace DescentCore.Equipment {
    //////////////////////////////////////////////////////////////////////
    // enums, constants etc:
    //////////////////////////////////////////////////////////////////////
    // there are more types, remember to add
    public enum ItemCatagory {
        Bow, Exotic, Blade, Book, Staff, Shield, Hammer, // hands
        LightArmor, MediumArmor, HeavyArmor, Cloak,  // armor
        Trinket, Magic, Rune, Axe, Helmet, Ring
    }
    public enum HandCatagory {
        MeleeWeopon, RangeWeopon, Shield
    }
    public enum EquipmentType {
        Hand, Armor, Trinket, Evil
    }
    
    //////////////////////////////////////////////////////////////////////
    // EquipedItems
    //////////////////////////////////////////////////////////////////////
    public class EquipedItems {
        public int UsedHands {
            get {
                int hands = 0;
                if (MainHand != null) {
                    hands += MainHand.Hands;
                } if (OffHand != null ) {
                    hands += OffHand.Hands;
                }
                return hands;
            }
        }
        public WeoponItem MainHand { get; private set; } = null;
        public ShieldItem OffHand { get; private set; } = null;
        public ArmorItem Armor { get; private set; } = null;
        public List<TrinketItem> Trinket { get; private set; }

        public List<Ability> GetAbilities() {
            var gearAbilities = new List<Ability>();
            foreach(Item item in this.IterItems()) {
                // Console.WriteLine($"T: {item.Name}");
                foreach(Ability itemAbility in item.Abilities) {
                    // Console.WriteLine($" - Ability: {itemAbility}");
                    gearAbilities.Add(itemAbility);
                }
            }
            return gearAbilities;
        }

        public IEnumerable<Item> IterItems() {
            if (this.MainHand != null) 
                yield return this.MainHand;
            if (this.OffHand != null) 
                yield return this.OffHand;
            if (this.Armor != null) 
                yield return this.Armor;
            foreach(TrinketItem trinket in Trinket)
                yield return trinket;
        }

        public EquipedItems() {
            this.Trinket = new List<TrinketItem>();
        }

        public void Equip(Item item) {
            switch(item.Type) {
                case (EquipmentType.Hand):
                    HandItem hand = item as HandItem;
                    if (hand.Hands + this.UsedHands > 2) {
                        throw new EquipmentException("Hands are already full");
                    }
                    WeoponItem weopon = item as WeoponItem;
                    if (weopon != null) {
                        this.MainHand = weopon;
                        break;
                    } 
                    ShieldItem shield = item as ShieldItem;
                    if (shield != null) {
                        this.OffHand = shield;
                        break;
                    } 
                    throw new EquipmentException("Weopon is not a shield or " +
                            "weopon but of EquipmentTypeHand");
                case (EquipmentType.Armor):
                    if (this.Armor != null) {
                        throw new EquipmentException("Armor already equiped!");
                    }
                    this.Armor = (ArmorItem) item;
                    break;
                case(EquipmentType.Trinket):
                    if (this.Trinket.Count > 2) {
                        throw new EquipmentException(
                                "Can not equip more than 2 Trinkets");
                    }
                    this.Trinket.Add((TrinketItem) item);
                    break;
            }
        }

        public void UnEquip(Item item) {
            switch(item.Type) {
                case (EquipmentType.Hand):
                    WeoponItem weopon = item as WeoponItem;
                    if ((weopon != null) && (weopon == this.MainHand)) {
                        this.MainHand = null;
                        break;
                    } 
                    ShieldItem shield = item as ShieldItem;
                    if ((shield != null) && (shield == this.OffHand)) {
                        this.OffHand = null;
                        break;
                    } 
                    throw new EquipmentException("Weopon is not a shield or" +
                            "weopon but of EquipmentTypeHand");
                case (EquipmentType.Armor):
                    if ((ArmorItem) item == this.Armor) {
                        this.Armor = null;
                        break;
                    }
                    throw new EquipmentException("Armor Not Equiped!");
                case(EquipmentType.Trinket):
                    // int length = this.Trinket.length;
                    // TODO: does this throw an error???
                    this.Trinket.Remove((TrinketItem) item);
                    break;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////
    // Equipment
    //////////////////////////////////////////////////////////////////////
    public class GearFactory {
        public List<WeoponItem> MainHand { get; private set; }
        public List<ShieldItem> OffHand { get; private set; }
        public List<ArmorItem> Armor { get; private set; }
        public List<TrinketItem> Trinket { get; private set; }
        public GearFactory(string source="ActI") {
            Armor = IterArmors(source).ToList();
            MainHand = IterMainHand(source).ToList();
            OffHand = IterOffHand(source).ToList();
            Trinket = IterTrinket(source).ToList();
        }
        public IEnumerable<string[]> IterFile(string source, string fileName) {
            string line;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                                       "Data", "Items", source, fileName);
            using (var reader = new StreamReader(path)) {
                reader.ReadLine();  // skip header
                while ((line = reader.ReadLine()) != null) {
                    if (line[0] != '#') {
                        yield return line.TrimEnd('\r', '\n').Split('\t');
                    }
                }
            }
        }

        private IEnumerable<ArmorItem> IterArmors(string source) {
            foreach(string[] tabs in IterFile(source, "armor.tsv")) {
                string name = tabs[0];
                DefenceDice defence = new DefenceDice(tabs[1].Split(','));
                ItemCatagory armorType = ParseCatagories(tabs[2])[0];
                Ability[] abilities = ParseAbilities(tabs[3]);
                yield return new ArmorItem(name, defence, armorType, abilities);
            }
        }

        private IEnumerable<WeoponItem> IterMainHand(string source) {
            foreach(string[] tabs in IterFile(source, "main_hand.tsv")) {
                string name = tabs[0];
                ItemCatagory[] catagories = ParseCatagories(tabs[1]);
                HandCatagory type = (HandCatagory)Enum.Parse(
                        typeof(HandCatagory), tabs[2]);
                int hands = Int32.Parse(tabs[3]);
                AttackDice attack = new AttackDice(tabs[4].Split(','));
                Ability[] abilities = ParseAbilities(tabs[5]);
                yield return new WeoponItem(name, attack, type, abilities, 
                                            catagories, hands);
            }
        }

        private IEnumerable<ShieldItem> IterOffHand(string source) {
            foreach(string[] tabs in IterFile(source, "off_hand.tsv")) {
                yield return new ShieldItem(tabs[0], ParseAbilities(tabs[1]));
            }
        }

        private IEnumerable<TrinketItem> IterTrinket(string source) {
            foreach(string[] tabs in IterFile(source, "trinket.tsv")) {
                string name = tabs[0];
                ItemCatagory[] catagories = ParseCatagories(tabs[1]);
                Ability[] abilities = ParseAbilities(tabs[2]);
                yield return new TrinketItem(name, catagories, abilities);
            }
        }

        private ItemCatagory[] ParseCatagories(string catagories) {
            return (from c in catagories.Split(',')
                    select (ItemCatagory)Enum.Parse(typeof(ItemCatagory), c)
                    ).ToArray();
        }

        private Ability[] ParseAbilities(string abilityString) {
            if (abilityString == "") {
                return new Ability[0];
            }
            var abilities = new List<Ability>();
            foreach (string a in abilityString.Split(',')) {
                abilities.Add(new Ability(a));
            }
            return abilities.ToArray();

        }
    }

    public abstract class Item {
        // TODO implement some interfaces
        public Ability[] Abilities { get; private set; } = new Ability[0];

        public ItemCatagory[] Catagories { get; private set; }
        public EquipmentType Type { get; private set; }
        public string Name { get; private set; }

        public Item(string name, EquipmentType type, Ability[] abilities,
                    ItemCatagory[] catagories) {
            this.Name = name;
            this.Type = type;
            this.Abilities = abilities;
            this.Catagories = catagories;
        }
        public override string ToString() {
            return $"{base.ToString()}: {{ Name = {Name}, Type = {Type}," +
                   $"Abilities = {Abilities}" + $"Categories = {Catagories} }}";
        }
        public override bool Equals(object obj) {
            return this.ToString() == obj.ToString();
        }
    }

    public class EvilArtefactItem : Item {
        public EvilArtefactItem(string name, Ability[] abilities,
                                ItemCatagory[] catagories) 
            : base(name, EquipmentType.Evil, abilities, catagories) { }
    }

    public class TrinketItem : Item {
        public TrinketItem(string name, ItemCatagory[] catagories, 
            Ability[] abilities) : base(name, EquipmentType.Trinket,
            abilities, catagories) { }
    }

    public class ArmorItem : Item {
        public DefenceDice DefenceDice { get; private set; }

        public ArmorItem(string name, DefenceDice defence, 
                         ItemCatagory armorType, Ability[] abilities)
            : base (name, EquipmentType.Armor, abilities, 
                    new ItemCatagory[1] { armorType }) {
            this.DefenceDice = defence;
        }
        public override string ToString() {
            return $"{base.ToString().TrimEnd('}')} DefenceDice = {DefenceDice}";
        }
    }

    ////////////////////////////////////////
    // Hand Items
    ////////////////////////////////////////
    public abstract class HandItem : Item {
        public int Hands { get; private set; }
        public HandCatagory HandType { get; private set; }

        public HandItem(string name, Ability[] abilities, 
                        ItemCatagory[] catagories, HandCatagory handType, 
                        int hands) : base(name, EquipmentType.Hand, 
                                          abilities, catagories) {
            this.HandType = handType;
            this.Hands = 1;
        }
        public override string ToString() {
            return $"{base.ToString().TrimEnd('}')} HandType = {HandType} " +
                   $"Hands = {Hands}";
        }
    }

    public class ShieldItem : HandItem {
        public ShieldItem(string name, Ability[] abilities) 
            : base(name, abilities, new ItemCatagory[1] { ItemCatagory.Shield },
                    HandCatagory.Shield, 1) { }
    }

    public class WeoponItem : HandItem {
        public AttackDice AttackDice { get; private set; }

        public WeoponItem(string name, AttackDice attack, HandCatagory weoponType, 
                Ability[] abilities, ItemCatagory[] catagories, int hands) 
                : base(name, abilities, catagories, weoponType, hands) {
            this.AttackDice = attack;
        }
        public override string ToString() {
            return $"{base.ToString().TrimEnd('}')} AttackDice = {AttackDice}";
        }
    }
}

