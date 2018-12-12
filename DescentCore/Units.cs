using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DescentCore.Dice;
using DescentCore.Equipment;
using DescentCore.Abilites;


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
        public virtual List<Ability> Abilities { get; set; }
        public bool Alive { get { return (this.Health == 0); } }
        
        public Unit(string name, int move, int health, AttackDice attack,
                    DefenceDice defence) { 
            Name = name;
            Move = move;
            MaxHealth = Health = health;
            this.AttackDice = attack;
            this.DefenceDice = defence;
            Abilities = new List<Ability>();
        }

        // public int MeleAttack(Unit enemy) {
            // return RangedAttack(enemy, 1);
        // }
// 
        public int Attack(Unit enemy, int range=1, bool resetAbilities=true) {
            // AttackResolver ar = new AttackResolver(this.AttackDice, enemy.DefenceDice, 
                                                   // this.Abilities);
            // int damage = ar.RollDamage(range=range);
            var dice = new Dice.Dice(this.AttackDice, enemy.DefenceDice);
            DiceOutcome roll = dice.Roll();
            int damage = this.ResolveSurges(roll, range);
            // int damage = this.RollDamage(range);
            enemy.Health -= damage;
            if (resetAbilities) {
                this.ResetAbilities();
            }
            return damage;
        }

        public void ResetAbilities() {
            this.Abilities.ForEach(a => a.Used = false);
        }
        
        public int[] DamageHistogram(int range) {
            throw new NotImplementedException("TODO!!!");
        }

        public int ResolveSurges(DiceOutcome dice, int range) {
            if (!dice.Hit) {
                return 0;
            }

            // 1. apply all "free abilities"
            foreach (Ability ability in this.Abilities) {
                if (ability.SurgePrice == 0) {
                    dice.UseAbility(ability);
                }
            }
            
            // 2. fix range if to short
            if (dice.Range < range) {
                this.IncreaseRange(dice, range);
                if (dice.Range < range) {
                    return 0;
                }
            }
            
            // 3. sort based on damage / surge
            this.UseSurges(dice);
            return dice.GetDamage(range);
        }

        protected void UseSurges(DiceOutcome dice) {
            while (dice.Surge > 0) {
                // Does Max raise an error if the list is empty or return zero???
                var rankedAbilities = 
                        from a in this.Abilities 
                        where !a.Used && dice.Surge >= a.SurgePrice
                        let ar = a.PotentialDamage(dice.Defence)
                        // most damage/surge, tiebreak on defence
                        orderby (float) ar.Damage / a.SurgePrice descending, 
                                ar.Attack.Power descending, 
                                ar.Defence.Shield descending
                        select a;
                if (!rankedAbilities.Any()) {
                    break;
                } else {
                    Ability best = rankedAbilities.First();
                    dice.UseAbility(best);
                }
            }
        }

        protected void IncreaseRange(DiceOutcome dice, int range) {
            while (dice.Surge > 0 && dice.Range < range) {
                var rangeAbilities = (from a in this.Abilities 
                                    where (!a.Used && a.Type == AbilityType.Range &&
                                            a.SurgePrice <= dice.Surge)
                                    select a).ToList();
                Ability perfect = (from a in rangeAbilities  
                                    where (a.Val + dice.Range >= range)
                                    orderby a.SurgePrice 
                                    select a).FirstOrDefault();
                if (perfect != null) {
                    dice.UseAbility(perfect);
                    break;
                } 
                Ability best = (from a in rangeAbilities  
                                 where dice.Surge >= a.SurgePrice
                                 orderby (float) a.Val / a.SurgePrice
                                 select a).FirstOrDefault();
                if (best != null) {
                    dice.UseAbility(best);
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
                         DefenceDice defence, int might, int knowledge, 
                         int willpower, int awareness) :
                         base(name, move, health, attack, defence) { 
            Might = might;
            Knowledge = knowledge;
            Willpower = willpower;
            Awareness = awareness;
        }
    }


    public class Henchmen : EliteUnit {
        public EvilArtefactItem EvilItem { get; set; } = null;
        public Henchmen(string name, int move, int health, int stamina, 
                        AttackDice attack, DefenceDice defence, int might, 
                        int knowledge, int willpower, int awareness)
                : base(name, move, health, attack, defence, might, knowledge,
                       willpower, awareness) { }
        // TODO make Henchmen equipment work
        // public Equip
    }


    public class Hero : EliteUnit {
        // attributes
        public DefenceDice BaseDefence { get; private set; }
        public int Stamina { get; private set; }
        public int MaxStamina { get; private set; }
        public int XP { get; private set; }
        public EquipedItems Equipment { get; } = new EquipedItems();
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
        public override List<Ability> Abilities {
            get {
                // TODO Hero Ability + Class Abilities
                return this.Equipment.GetAbilities();
            }
        }

        public Hero(string name, int move, int health, int stamina, 
                    DefenceDice defence, int might, int knowledge, int willpower,
                    int awareness, int xp=0) 
                : base(name, move, health, new AttackDice(), defence, might, 
                       knowledge, willpower, awareness) { 
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

