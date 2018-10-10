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
        public virtual AttackDice AttackDice { get; private set; }
        public virtual DefenceDice DefenceDice { get; private set; }
        // public List<AttackDie> Attack { get; private set; }
        public virtual List<Abillity> Abillities { get; set; }
        public bool Alive { get { return (this.Health == 0); } }
        
        public Unit(string name, int move, int health, AttackDice attack,
                    DefenceDice defence) { 
            Name = name;
            Move = move;
            MaxHealth = Health = health;
            this.AttackDice = attack;
            this.DefenceDice = defence;
            Abillities = new List<Abillity>();
        }

        // public int MeleAttack(Unit enemy) {
            // return RangedAttack(enemy, 1);
        // }
// 
        public int Attack(Unit enemy, int range=1, bool resetAbillities=true) {
            // AttackResolver ar = new AttackResolver(this.AttackDice, enemy.DefenceDice, 
                                                   // this.Abillities);
            // int damage = ar.RollDamage(range=range);
            var dice = new Dice.Dice(this.AttackDice, enemy.DefenceDice);
            DiceOutcome roll = dice.Roll();
            int damage = this.ResolveSurges(roll, range);
            // int damage = this.RollDamage(range);
            enemy.Health -= damage;
            if (resetAbillities) {
                this.ResetAbillities();
            }
            return damage;
        }

        public void ResetAbillities() {
            this.Abillities.ForEach(a => a.Used = false);
        }
        
        public int[] DamageHistogram(int range) {
            throw new NotImplementedException("TODO!!!");
        }

        public int ResolveSurges(DiceOutcome dice, int range) {
            if (!dice.Hit) {
                return 0;
            }
            // var dice = new DiceOutcome(attackRoll.Clone(), defenceRoll.Clone());
            Console.WriteLine("--------------------");
            Console.WriteLine($"step 0\nAttack: {dice.Attack}\nDefence: {dice.Defence}\n");

            // 1. apply all "free abillities"
            foreach (Abillity abillity in this.Abillities) {
                if (abillity.SurgePrice == 0) {
                    dice.UseAbillity(abillity);
                }
            }
            Console.WriteLine($"step 1\nAttack: {dice.Attack}\nDefence: {dice.Defence}\n");
            
            // 2. fix range if to short
            if (dice.Range < range) {
                this.IncreaseRange(dice, range);
            }
            Console.WriteLine($"step 2\nAttack: {dice.Attack}\nDefence: {dice.Defence}\n");
            
            // 3. sort based on damage / surge
            this.UseSurges(dice);

            Console.WriteLine($"step 3\nAttack: {dice.Attack}\nDefence: {dice.Defence}\n");
            Console.WriteLine($"Final: {dice.Damage}");
            return dice.Damage;
        }

        protected void UseSurges(DiceOutcome dice) {
            Console.WriteLine($"Abillities Count {this.Abillities}");
            while (dice.Surge > 0) {
                // Does Max raise an error if the list is empty or return zero???
                Console.WriteLine(" -- test ==");
                var rankedAbillities = 
                        from a in this.Abillities 
                        where !a.Used && dice.Surge >= a.SurgePrice
                        let ar = a.PotentialDamage(dice.Defence)
                        // most damage/surge, tiebreak on defence
                        orderby (float) ar.Damage / a.SurgePrice descending, 
                                ar.Attack.Power descending, 
                                ar.Defence.Shield descending
                        select a;
                if (!rankedAbillities.Any()) {
                    break;
                } else {
                    Abillity best = rankedAbillities.First();
                    Console.WriteLine($"Best abillity: {best}");
                    Console.WriteLine($"attack before {dice.Attack} - Defence Before {dice.Defence}");
                    dice.UseAbillity(best);
                    Console.WriteLine($"attack after {dice.Attack} - Defence after {dice.Defence}");
                }
            }
            Console.WriteLine($"returning {dice.Attack} and {dice.Defence}");
            // return (attack, defence);
        }

        protected void IncreaseRange(DiceOutcome dice, int range) {
            while (dice.Surge > 0 && range < dice.Range) {
                var rangeAbillities = from a in this.Abillities 
                                    where (!a.Used && a.Type == AbillityType.Range)
                                    select a;
                Abillity perfect = (from a in rangeAbillities  
                                    where (a.Val + dice.Range >= range)
                                    orderby a.SurgePrice 
                                    select a).FirstOrDefault();
                if (perfect != null) {
                    dice.UseAbillity(perfect);
                    break;
                } 
                Abillity best = (from a in rangeAbillities  
                                 where dice.Surge >= a.SurgePrice
                                 orderby (float) a.Val / a.SurgePrice
                                 select a).FirstOrDefault();
                if (best != null) {
                    dice.UseAbillity(best);
                } else {
                    dice.Hit = false;
                    break;
                }
            }
        }
    }

    public class Monster : Unit {
        public Monster(string name, int move, int health, AttackDice attack,
                       DefenceDice defence) : 
                base(name, move, health, attack, defence) { }
    }


    public class EliteUnit : Unit {
        // properties
        public int Might { get; private set; }
        public int Knowledge { get; private set; }
        public int Willpower { get; private set; }
        public int Awareness { get; private set; }
        
        public EliteUnit(string name, int move, int health, AttackDice attack,
                         DefenceDice defence, int might, int knowledge, int willpower,
                         int awareness) : base(name, move, health, attack, defence) { 
            Might = might;
            Knowledge = knowledge;
            Willpower = willpower;
            Awareness = awareness;
        }
    }

    public class Henchmen : EliteUnit {
        public EvilArtefactItem EvilItem { get; set; } = null;
        public Henchmen(string name, int move, int health, int stamina, AttackDice attack, 
                        DefenceDice defence, int might, int knowledge, int willpower,
                        int awareness) : base(name, move, health, attack, defence, might,
                                              knowledge, willpower, awareness) { }
        // TODO make Henchmen equipment work
        // public Equip
    }


    public class Hero : EliteUnit {
        // attributes
        public DefenceDice BaseDefence { get; private set; }
        public int Stamina { get; private set; }
        public int MaxStamina { get; private set; }
        public int XP { get; private set; }
        private EquipedItems Equipment { get; } = new EquipedItems();
        public override AttackDice AttackDice {
            get { return this.Equipment.MainHand.AttackDice; }
        }
        public override DefenceDice DefenceDice {
            get { 
                var defence = new DefenceDice();
                this.BaseDefence.ForEach(die => defence.Add(die));
                this.Equipment.Armor.DefenceDice.ForEach(die => defence.Add(die));
                return defence;
            }
        }
        public override List<Abillity> Abillities {
            get {
                // TODO Hero Abillity + Class Abillities
                return this.Equipment.GetAbillities();
            }
        }

        public Hero(string name, int move, int health, int stamina, DefenceDice defence,
                    int might, int knowledge, int willpower, int awareness, int xp=0) : 
                base(name, move, health, new AttackDice(), defence, might, knowledge, 
                     willpower, awareness) { 
            this.BaseDefence = defence;
            this.MaxStamina = stamina;
            this.Stamina = stamina;
            this.XP = xp;
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

