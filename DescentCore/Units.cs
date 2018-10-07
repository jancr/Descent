using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DescentCore.Dice;
using DescentCore.Equipment;
using DescentCore.Abillites;
using DescentCore.Attack;


namespace DescentCore.Units {
    //////////////////////////////////////////////////////////////////////
    // The Base class for all Heros and Monsters
    //////////////////////////////////////////////////////////////////////
    public class Unit {
        // attributes
        private int health;

        // properties
        public int Move { get; private set; }
        public int Health {
            get { return this.health; }
            private set { 
                if (value > this.MaxHealth) {
                    this.health = this.MaxHealth;
                } else if (value <= 0) {
                    this.health = 0;
                } else {
                    this.health = value;
                }
            }
        }
        public int MaxHealth { get; private set; }
        public string Name { get; private set; }
        public virtual List<AttackDie> AttackDice { get; private set; }
        public virtual List<DefenceDie> DefenceDice { get; private set; }
        // public List<AttackDie> Attack { get; private set; }
        public List<Abillity> Abillities { get; set; }
        public bool Alive { get { return (this.Health == 0); } }
        
        public Unit(string name, int move, int health, List<AttackDie> attack,
                    List<DefenceDie> defence) { 
            Name = name;
            Move = move;
            MaxHealth = Health = health;
            AttackDice = attack;
            DefenceDice = defence;
            Abillities = new List<Abillity>();
        }

        // public int MeleAttack(Unit enemy) {
            // return RangedAttack(enemy, 1);
        // }
// 
        public int Attack(Unit enemy, int range=1) {
            AttackResolver ar = new AttackResolver(this.AttackDice, enemy.DefenceDice, 
                                                   this.Abillities);
            int damage = ar.RollDamage(range=range);
            enemy.Health -= damage;
            return damage;
        }
    }

    public class Monster : Unit {
        public Monster(string name, int move, int health, List<AttackDie> attack,
                       List<DefenceDie> defence) : 
                base(name, move, health, attack, defence) { }
    }


    public class EliteUnit : Unit {
        // properties
        public int Might { get; private set; }
        public int Knowledge { get; private set; }
        public int Willpower { get; private set; }
        public int Awareness { get; private set; }
        
        public EliteUnit(string name, int move, int health, List<AttackDie> attack,
                         List<DefenceDie> defence, int might, int knowledge, int willpower,
                         int awareness) : base(name, move, health, attack, defence) { 
            Might = might;
            Knowledge = knowledge;
            Willpower = willpower;
            Awareness = awareness;
        }
    }

    public class Henchmen : EliteUnit {
        public EvilArtefactItem EvilItem { get; set; } = null;
        public Henchmen(string name, int move, int health, int stamina, List<AttackDie> attack, 
                        List<DefenceDie> defence, int might, int knowledge, int willpower,
                        int awareness) : base(name, move, health, attack, defence, might,
                                              knowledge, willpower, awareness) { }
        // TODO make Henchmen equipment work
        // public Equip
    }


    public class Hero : EliteUnit {
        // attributes
        public List<DefenceDie> BaseDefence { get; private set; }
        public int Stamina { get; private set; }
        public int MaxStamina { get; private set; }
        public int XP { get; private set; }
        private EquipedItems Equipment { get; set; } = new EquipedItems();
        public override List<AttackDie> AttackDice {
            get { return new List<AttackDie>(this.Equipment.MainHand.AttackDice); }
        }
        public override List<DefenceDie> DefenceDice {
            get { 
                var both = this.BaseDefence.Concat(this.Equipment.Armor.DefenceDice);
                return new List<DefenceDie>(both);
            }
        }

        public Hero(string name, int move, int health, int stamina, List<DefenceDie> defence,
                    int might, int knowledge, int willpower, int awareness, int xp=0) : 
            base(name, move, health, new List<AttackDie>(), defence, might, knowledge, 
                    willpower, awareness) { 
                BaseDefence = defence;
                MaxStamina = Stamina = stamina;
                XP = xp;
            }

        public void AddClass() {
            // TODO: make class stuff
        }
        
        public void Equip(Item item) {
            Equipment.Equip(item);
        }

        public void UnEquip(Item item) {
            Equipment.UnEquip(item);
        }

        // public void Equip(EquipmentType type) {
            // Equipment.Equip(type, this);
        // }
// 
        // public void UnEquip(EquipmentType type) {
            // Equipment.UnEquip(type, this);
        // }
    }
}

