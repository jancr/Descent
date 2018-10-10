using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DescentCore.Abillites;
using DescentCore.Exceptions;

namespace DescentCore.Dice {
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
        
        public AttackDieFace(int power=0, int surge=0, int range=0, bool hit=true) {
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

        public static AttackDieFace operator + (AttackDieFace face1, AttackDieFace face2) {
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
            return new AttackDieFace(this.Power, this.Surge, this.Range, this.Hit);
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

        public static DefenceDieFace operator + (DefenceDieFace die1, DefenceDieFace die2) {
            return new DefenceDieFace(die1.Shield + die2.Shield);
        }

        public static DefenceDieFace operator - (DefenceDieFace die1, DefenceDieFace die2) {
            return new DefenceDieFace(die1.Shield - die2.Shield);
        }
        public override string ToString() {
            return $"Shilds: {this.Shield}";
        }

        public DefenceDieFace Clone() {
            return new DefenceDieFace(this.Shield);
        }
    }
    

    ////////////////////////////////////////
    // die class and subclasses
    ////////////////////////////////////////
    public abstract class Die {
        protected Random rand = new Random();
        // public Roll();

        // public ToDistrubution() {
            // raise NotImplemented();
        // }
    }

    public abstract class AttackDie: Die {
        public AttackDieFace[] faces;

        public AttackDieFace Roll() {
            return faces[rand.Next(0, faces.Length)];
        }
    }

    public abstract class DefenceDie: Die {
        public DefenceDieFace[] faces;

        public DefenceDieFace Roll() {
            return faces[rand.Next(0, faces.Length)];
        }
    }


    // Attack Dice
    public class PowerDie: AttackDie {
        public PowerDie() {
            this.faces = new AttackDieFace[] {
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
            this.faces = new AttackDieFace[] {
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
            this.faces = new AttackDieFace[] {
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
            this.faces = new AttackDieFace[] {
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
            this.faces = new DefenceDieFace[] {
                new DefenceDieFace(0), new DefenceDieFace(0), new DefenceDieFace(0),
                new DefenceDieFace(1), new DefenceDieFace(1), new DefenceDieFace(2)
            };
        }
    }

    public class GreyDie: DefenceDie {
        public GreyDie() {
            this.faces = new DefenceDieFace[] {
                new DefenceDieFace(0), new DefenceDieFace(1), new DefenceDieFace(1),
                new DefenceDieFace(1), new DefenceDieFace(2), new DefenceDieFace(3),
            };
        }
    }

    public class BlackDie: DefenceDie {
        public BlackDie() {
            this.faces = new DefenceDieFace[] {
                new DefenceDieFace(0), new DefenceDieFace(2), new DefenceDieFace(2),
                new DefenceDieFace(2), new DefenceDieFace(3), new DefenceDieFace(4)
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
        public int Power { get {return this.Attack.Power;} set {this.Attack.Power = value;} }
        public int Surge { get {return this.Attack.Surge;} set {this.Attack.Surge = value;} }
        public int Range { get {return this.Attack.Range;} set {this.Attack.Range = value;} }
        public bool Hit { get {return this.Attack.Hit;} set {this.Attack.Hit = value;} }
        public int Shield { get {return this.Defence.Shield;} 
                            set {this.Defence.Shield = value;} }
        public int Damage { get { return Math.Max(this.Power - this.Shield, 0); } }

        public DiceOutcome(AttackDieFace attack, DefenceDieFace defence) {
            this.Attack = attack;
            this.Defence = defence;
        }

        public void UseAbillity(Abillity abillity) {
            if (abillity.SurgePrice > this.Surge) {
                throw new AbillityException("Not Enugh Surges to Use Abillity");
            }
            this.Surge -= abillity.SurgePrice;
            abillity.Used = true;
            switch(abillity.Type) {
                case AbillityType.Damage:
                    this.Power += abillity.Val;
                    break;
                case AbillityType.Pierce:
                    this.Shield -= abillity.Val;
                    break;
                case AbillityType.Range:
                    this.Range += abillity.Val;
                    break;
            }
        }

        public DiceOutcome Clone() {
            return new DiceOutcome(this.Attack.Clone(), this.Defence.Clone());
        }
    }
}

