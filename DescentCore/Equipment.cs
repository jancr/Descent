using System;
using System.Collections;
using System.Collections.Generic;

using DescentCore.Dice;
using DescentCore.Abillites;

namespace DescentCore.Equipment {
    //////////////////////////////////////////////////////////////////////
    // enums, constants etc:
    //////////////////////////////////////////////////////////////////////
    // there are more types, remember to add
    public enum ItemCatagory {
        Bow, Exotic, Blade, Book, Magic, Staff, Trinket,
        LightArmor, MediumArmor, HeavyArmor, Shield
    }
    public enum HandCatagory {
        MeleeWeopon, RangeWeopon, Shield
    }
    public enum EquipmentType {
        Hand, Armor, Trinket, Evil
    }
    
    //////////////////////////////////////////////////////////////////////
    // Exceptions
    //////////////////////////////////////////////////////////////////////
    public class EquipmentException: Exception { 
        public EquipmentException() { }
        public EquipmentException(string message) : base(message) { }
        public EquipmentException(string message, Exception inner) : base(message, inner) { }
    }


    //////////////////////////////////////////////////////////////////////
    // EquipedItems
    //////////////////////////////////////////////////////////////////////
    public class EquipedItems {
        private int usedHands = 0;
        public WeoponItem MainHand { get; private set; } = null;
        public ShieldItem OffHand { get; private set; } = null;
        public ArmorItem Armor { get; private set; } = null;
        public List<TrinketItem> Trinket { get; private set; }

        public EquipedItems() {
            this.Trinket = new List<TrinketItem>();
        }

        public void Equip(Item item) {
            switch(item.Type) {
                case (EquipmentType.Hand):
                    HandItem hand = item as HandItem;
                    if (hand.Hands + this.usedHands > 2) {
                        throw new EquipmentException("Hands are already full");
                    }
                    this.usedHands += hand.Hands;

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
                    throw new EquipmentException("Weopon is not a shield or weopon but of EquipmentTypeHand");
                case (EquipmentType.Armor):
                    if (this.Armor != null) {
                        throw new EquipmentException("Armor already equiped!");
                    }
                    this.Armor = (ArmorItem) item;
                    break;
                case(EquipmentType.Trinket):
                    if (this.Trinket.Count > 2) {
                        throw new EquipmentException("Can not equip more than 2 Trinkets");
                    }
                    this.Trinket.Add((TrinketItem) item);
                    break;
            }
        }

        public void UnEquip(Item item) {
            switch(item.Type) {
                case (EquipmentType.Hand):
                    WeoponItem weopon = item as WeoponItem;
                    if (weopon == this.MainHand) {
                        this.usedHands -= this.MainHand.Hands;
                        this.MainHand = null;
                        break;
                    } 
                    ShieldItem shield = item as ShieldItem;
                    if (shield == this.OffHand) {
                        this.usedHands -= this.OffHand.Hands;
                        this.OffHand = null;
                        break;
                    } 
                    throw new EquipmentException("Weopon is not a shield or weopon but of EquipmentTypeHand");
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
    public abstract class Item {
        // TODO implement some interfaces
        public Abillity[] Abillities { get; private set; } = new Abillity[0];

        public ItemCatagory[] Catagories { get; private set; }
        public EquipmentType Type { get; private set; }


        public Item(EquipmentType type, Abillity[] abillities, ItemCatagory[] catagories) {
            this.Type = type;
            this.Abillities = abillities;
            this.Catagories = catagories;
        }
    }

    public class EvilArtefactItem : Item {
        public EvilArtefactItem(Abillity[] abillities, ItemCatagory[] catagories) 
            : base(EquipmentType.Evil, abillities, catagories) { }
    }

    public class TrinketItem : Item {
        public TrinketItem(Abillity[] abillities) : base(EquipmentType.Trinket, abillities, 
                new ItemCatagory[1] { ItemCatagory.Trinket }) { }
    }

    public class ArmorItem : Item {
        public DefenceDie[] DefenceDice { get; private set; }

        public ArmorItem(DefenceDie[] defence, ItemCatagory armorType, Abillity[] abillities)
                : base (EquipmentType.Armor, abillities, new ItemCatagory[1] { armorType }) {
            this.DefenceDice = defence;
        }
    }

    ////////////////////////////////////////
    // Hand Items
    ////////////////////////////////////////
    public abstract class HandItem : Item {
        public int Hands { get; private set; }
        public HandCatagory HandType { get; private set; }

        public HandItem(Abillity[] abillities, ItemCatagory[] catagories, 
                HandCatagory handType, int hands) : base(EquipmentType.Hand, abillities, catagories) {
            this.HandType = handType;
            this.Hands = 1;
        }
    }

    public class ShieldItem : HandItem {
        public ShieldItem(Abillity[] abillities) : base(abillities, 
                new ItemCatagory[1] { ItemCatagory.Shield }, HandCatagory.Shield, 1) { }
    }

    public class WeoponItem : HandItem {
        public AttackDie[] AttackDice { get; private set; }

        public WeoponItem(AttackDie[] attack, HandCatagory weoponType, Abillity[] abillities, 
                ItemCatagory[] catagories, int hands) 
                : base(abillities, catagories, weoponType, hands) {
            this.AttackDice = attack;
        }
    }
}

