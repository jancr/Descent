using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DescentCore.Abilites;
using DescentCore.Exceptions;

namespace DescentCore.Dice {
    ////////////////////////////////////////
    // Dice Distributions
    ////////////////////////////////////////
    // class DiceDistribution
        // private int Combinations { get; private set; }
        // private Dictionary<DiceOutcome, int> rolToCount;
        // private Dictionary<int, DiceOutcome> countToRoll;
// 
        // public DiceDistribution(Dice dice) {
// 
// 
            // // exhaust attack space
            // int attackCombinations =  Math.Power(6, dice.Attack.Count);
            // for (int i = 0; i < attackCombinations; i++) {
                // AttackDieFace attack = new AttackDieFace();
                // for (int j = 0; j < dice.Attack.Count; ++) {
                    // int index = (i / (Math.Pow(6, j))) % 6
                    // attack += attackDie[j].faces[index]
                // }
            // }
            // // exhaust defence space
            // //
            // // combine
        // }

    
    ////////////////////////////////////////
    // DieFace classes
    ////////////////////////////////////////
    abstract public class DieFace { 
    }

    public class AttackDieFace : DieFace  {
        //attribues
        private int _power = 0;
        private int _surge = 0;
        private int _range = 0;
        private bool _hit = true;

        public AttackDieFace(int power=0, int surge=0, int range=0,
                             bool hit=true) {
            this.Hit = hit;
            this.Power = power;
            this.Surge = surge;
            this.Range = range;
        }

        // proerties
        public int Power { 
            get { return this._power; }
            set { this._power = this.Hit ? value : 0; }
        }
        public int Surge { 
            get { return this._surge; }
            set { this._surge = this.Hit ? value : 0; }
        }
        public int Range { 
            get { return this._range; }
            set { this._range = this.Hit ? value : 0; }
        }
        public bool Hit { 
            get { return this._hit; }
            set {
                this._hit = value; 
                if (!value) {
                    this._power = this._surge = this._range = 0;
                }
            }
        }

        // methods
        public static AttackDieFace Mis() {
            return new AttackDieFace(0, 0, 0, false);
        }

        public static AttackDieFace operator + (AttackDieFace face1,
                                                AttackDieFace face2) {
            bool hit = face1.Hit && face2.Hit; 
            int power = face1.Power + face2.Power;
            int surge = face1.Surge + face2.Surge;
            int range = face1.Range + face2.Range;
            return new AttackDieFace(power, surge, range, hit);
        }

        public override string ToString() {
            if (this.Hit) {
                return $"Power: {this.Power}, Surge: {this.Surge}, Range {this.Range}";
            }
            return "Mis";
        }

        public AttackDieFace Clone() {
            return new AttackDieFace(this.Power, this.Surge, this.Range, 
                                     this.Hit);
        }

        public override bool Equals(object obj) {
            AttackDieFace other = obj as AttackDieFace;
            if (other != null) {
                return ((this.Hit == other.Hit) && (this.Power == other.Power) && 
                    (this.Surge == other.Surge) && (this.Range == other.Range));
            }
            return false;
        }

        public override int GetHashCode() {
            return (Hit.GetHashCode() ^ Power ^ Surge ^ Range);
        }
        
    }

    public class DefenceDieFace : DieFace {
        // attribues
        private int _shield = 0;

        // constructors
        public DefenceDieFace(int shield=0) {
            this.Shield = shield;
        }

        // properties
        public int Shield { 
            get { return this._shield; }
            set { this._shield = Math.Max(value, 0); } 
        } 

        // methods
        public void Pierce(int pierce) {
            this.Shield -= pierce;
        }
        
        public int TryPierce(int pierce) {
            return Math.Min(pierce, this.Shield);
        }

        public static DefenceDieFace operator + (DefenceDieFace die, int number) {
            return new DefenceDieFace(die.Shield + number);
        }

        public static DefenceDieFace operator - (DefenceDieFace die, int number) {
            return new DefenceDieFace(die.Shield - number);
        }

        public static DefenceDieFace operator + (DefenceDieFace die1, 
                                                 DefenceDieFace die2) {
            return new DefenceDieFace(die1.Shield + die2.Shield);
        }

        public static DefenceDieFace operator - (DefenceDieFace die1,
                                                 DefenceDieFace die2) {
            return new DefenceDieFace(die1.Shield - die2.Shield);
        }
        public override string ToString() {
            return $"Shilds: {this.Shield}";
        }

        public DefenceDieFace Clone() {
            return new DefenceDieFace(this.Shield);
        }

        public override bool Equals(object obj) {
            DefenceDieFace other = obj as DefenceDieFace;
            if (other != null) {
                return this.Shield == other.Shield;
            }
            return false;
        }

        public override int GetHashCode() {
            return this.Shield;
        }
        
    }
    

    ////////////////////////////////////////
    // die class and subclasses
    ////////////////////////////////////////
    public abstract class Die<T> where T: DieFace {
        public T[] Faces { get; protected set; }
        protected Random rand = new Random();

        public T Roll() {
            return Faces[rand.Next(0, Faces.Length)];
        }

        public override bool Equals(object obj) {
            Die<T> other = obj as Die<T>;
            if (other != null) {
                return this.Faces.SequenceEqual(other.Faces);
            }
            return false;
        }

        public override int GetHashCode() {
            int hash = 0;
            foreach(T face in this.Faces) { 
                hash ^= face.GetHashCode();
            }
            return hash;
        }
        // public ToDistrubution() {
            // raise NotImplemented();
        // }
    }
    public abstract class AttackDie: Die<AttackDieFace> { }
    public abstract class DefenceDie: Die<DefenceDieFace> { }


    // Attack Dice
    public class PowerDie: AttackDie {
        public PowerDie() {
            this.Faces = new AttackDieFace[] {
                new AttackDieFace(1),
                new AttackDieFace(2),
                new AttackDieFace(2),
                new AttackDieFace(2),
                new AttackDieFace(3),
                new AttackDieFace(3, 1)
            };
        }
    }

    public class YellowDie: AttackDie {
        public YellowDie() {
            this.Faces = new AttackDieFace[] {
                new AttackDieFace(0, 1, 1),
                new AttackDieFace(1, 0, 1),
                new AttackDieFace(1, 0, 2),
                new AttackDieFace(1, 0, 1),
                new AttackDieFace(2, 0, 0),
                new AttackDieFace(2, 1, 0)
            };
        }
    }

    public class GreenDie: AttackDie {
        public GreenDie() {
            this.Faces = new AttackDieFace[] {
                new AttackDieFace(1, 0, 0),
                new AttackDieFace(0, 1, 0),
                new AttackDieFace(0, 1, 1),
                new AttackDieFace(1, 0, 1),
                new AttackDieFace(1, 1, 0),
                new AttackDieFace(1, 1, 1)
            };
        }
    }

    public class BlueDie: AttackDie {
        public BlueDie() {
            this.Faces = new AttackDieFace[] {
                new AttackDieFace(0, 0, 0, false),
                new AttackDieFace(2, 1, 2),
                new AttackDieFace(2, 0, 3),
                new AttackDieFace(2, 0, 4),
                new AttackDieFace(1, 0, 5),
                new AttackDieFace(1, 1, 6)
            };
        }
    }

    // Defence Dice
    public class BrownDie: DefenceDie {
        public BrownDie() {
            this.Faces = new DefenceDieFace[] {
                new DefenceDieFace(0), new DefenceDieFace(0), 
                new DefenceDieFace(0), new DefenceDieFace(1),
                new DefenceDieFace(1), new DefenceDieFace(2)
            };
        }
    }

    public class GreyDie: DefenceDie {
        public GreyDie() {
            this.Faces = new DefenceDieFace[] {
                new DefenceDieFace(0), new DefenceDieFace(1),
                new DefenceDieFace(1), new DefenceDieFace(1),
                new DefenceDieFace(2), new DefenceDieFace(3),
            };
        }
    }

    public class BlackDie: DefenceDie {
        public BlackDie() {
            this.Faces = new DefenceDieFace[] {
                new DefenceDieFace(0), new DefenceDieFace(2),
                new DefenceDieFace(2), new DefenceDieFace(2),
                new DefenceDieFace(3), new DefenceDieFace(4)
            };
        }
    }

    ////////////////////////////////////////
    // Dice class and subclasses
    // Dice are classes of List<Die>
    ////////////////////////////////////////
    public class AttackDice : List<AttackDie> {
        public AttackDice() { }

        public AttackDice(params string[] names) {
            foreach (string name in names) {
                switch (name) {
                    case "blue":
                        this.Add(new BlueDie());
                        break;
                    case "power": 
                    case "red":
                        this.Add(new PowerDie());
                        break;
                    case "yellow":
                        this.Add(new YellowDie());
                        break;
                    case "green":
                        this.Add(new GreenDie());
                        break;
                    default:
                        throw new DieException("Invalid Die");
                }
            }
        }

        public AttackDice(params AttackDie[] dice) {
            foreach (var die in dice) {
                this.Add(die);
            }
        }

        public AttackDieFace Roll() {
            var total = new AttackDieFace();
            this.ForEach(die => total += die.Roll());
            return total;
        }
        
        public override string ToString() {
            return $"{base.ToString()}: {this}";
        }
    }

    public class DefenceDice : List<DefenceDie> {
        // private static Dictionary<string, DefenceDie> 
            // nameMapper = new Dictionary<string, DefenceDie>  {
            // { "brown", BrownDie }, { "grey", GrenDie }, { "black". BlackDie }
        // };
        public DefenceDice() { }
        public DefenceDice(params string[] names) {
            foreach (string name in names) {
                switch (name) {
                    case "brown":
                        this.Add(new BrownDie());
                        break;
                    case "grey":
                        this.Add(new GreyDie());
                        break;
                    case "black":
                        this.Add(new BlackDie());
                        break;
                    case "":
                        break;
                    default:
                        throw new DieException("Invalid Die");
                }
            }
        }

        public DefenceDice(params DefenceDie[] dice) {
            foreach (var die in dice) {
                this.Add(die);
            }
        }

        public DefenceDieFace Roll() {
            var total = new DefenceDieFace();
            this.ForEach(die => total += die.Roll());
            return total;
        }

        public override string ToString() {
            return $"{base.ToString()}: {this}";
        }
    }

    public class Dice {
        public AttackDice Attack  { get; private set; }
        public DefenceDice Defence { get; private set; }
        public Dice(AttackDice attackDice, DefenceDice defenceDice) {
            this.Attack = attackDice;
            this.Defence = defenceDice;
        }

        public DiceOutcome Roll() {
            return new DiceOutcome(this.RollAttack(), this.RollDefence());
        }

        public AttackDieFace RollAttack() { return this.Attack.Roll(); }
        public DefenceDieFace RollDefence() { return this.Defence.Roll(); }
    }
    
    public class DiceOutcome : DieFace {
        public AttackDieFace Attack { get; set; }
        public DefenceDieFace Defence { get; set; }
        public int Power { get {return this.Attack.Power;}
                           set {this.Attack.Power = value;} }
        public int Surge { get {return this.Attack.Surge;} 
                           set {this.Attack.Surge = value;} }
        public int Range { get {return this.Attack.Range;}
                           set {this.Attack.Range = value;} }
        public bool Hit { get {return this.Attack.Hit;}
                          set {this.Attack.Hit = value;} }
        public int Shield { get {return this.Defence.Shield;} 
                            set {this.Defence.Shield = value;} }

        public DiceOutcome(AttackDieFace attack, DefenceDieFace defence) {
            this.Attack = attack;
            this.Defence = defence;
        }

        public int GetDamage(int range=1) {
            if (this.Hit && this.Range >= range) {
                return Math.Max(this.Power - this.Shield, 0);
            }
            return 0;
        }

        public void Mis() {
            this.Attack = AttackDieFace.Mis();
        }

        public void UseAbility(Ability ability) {
            if (ability.SurgePrice > this.Surge) {
                throw new AbilityException("Not Enugh Surges to Use Ability");
            }
            this.Surge -= ability.SurgePrice;
            ability.Used = true;
            switch(ability.Type) {
                case AbilityType.Damage:
                    this.Power += ability.Val;
                    break;
                case AbilityType.Pierce:
                    this.Shield -= ability.Val;
                    break;
                case AbilityType.Range:
                    this.Range += ability.Val;
                    break;
            }
        }

        public DiceOutcome Clone() {
            return new DiceOutcome(this.Attack.Clone(), this.Defence.Clone());
        }
    }
}

