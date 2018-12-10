using System;
using System.Text.RegularExpressions;

using DescentCore.Dice;

namespace DescentCore.Abillites { 
    public enum AbillityType {
        Damage, Pierce, Range, 
        Blast, Reach, Stun  // <- TODO: Implement!
    }
    //////////////////////////////////////////////////////////////////////
    // Abillity
    //////////////////////////////////////////////////////////////////////
    public struct AbillityResult {
        public int Damage { get; private set; }
        public AttackDieFace Attack { get; private set; }
        public DefenceDieFace Defence { get; private set; }
        public AbillityResult(int damage, int attack, int defence) {
            this.Damage = damage;
            this.Attack = new AttackDieFace(attack);
            this.Defence = new DefenceDieFace(defence);
        }
    }

    public class Abillity : ICloneable {
        public int Val { get; private set; }
        public AbillityType Type { get; private set; }
        public int SurgePrice { get; private set; }
        public int StaminaPrice { get; private set; }
        public bool Used { get; set; } = false;

        // helper fields
        private Regex abillityRegex = new Regex(@"(\d+)([LS]?):(\d+)([DPR])");

        public Abillity(int val, AbillityType type=AbillityType.Damage, int surgePrice=0, 
                int staminaPrice=0) {
            this.Val = val;
            this.Type = type;
            this.SurgePrice = surgePrice;
            this.StaminaPrice = staminaPrice;  // not fully implemented
        }
        public Abillity(string abillityString) {
            Match m = abillityRegex.Match(abillityString);
            if (m.Success) {
                int price = Int32.Parse(m.Groups[1].ToString());
                string _priceString = m.Groups[2].ToString();
                if (_priceString.Length == 0) {
                    this.SurgePrice = 0;
                } else {
                    char priceType = _priceString[0];
                    if (priceType == 'L') {
                        this.SurgePrice = price;
                    } else if (priceType == 'S') {
                        throw new NotImplementedException(
                                "Stamina abillities not implemented Yet");
                    }
                }

                this.Val = Int32.Parse(m.Groups[3].ToString());
                char ValType = m.Groups[4].ToString()[0];
                switch (ValType) {
                    case 'D':
                        this.Type = AbillityType.Damage;
                        break;
                    case 'P':
                        this.Type = AbillityType.Pierce;
                        break;
                    case 'R':
                        this.Type = AbillityType.Range;
                        break;
                }
            } else {
                throw new ArgumentException(
                        $"abillityString {abillityString} is not a valid regex");
            }
        }

        public AbillityResult PotentialDamage(DefenceDieFace defence) {
            // TODO: can there be abillities that trigger multiple things?
            //       - there are a few, but it is out of the scope of the project
            // if so I should be a boolian class like EquipedItems!!!
            // and this should be a switch statment
            if (this.Type == AbillityType.Damage) {
                return new AbillityResult(this.Val, this.Val, defence.Shield);
            } else if (this.Type == AbillityType.Pierce) {
                int pierce = Math.Min(this.Val, defence.Shield);
                return new AbillityResult(pierce, 0, defence.Shield - pierce);
            }
            return new AbillityResult(0, 0, defence.Shield);
            // TODO support other effects, such as tab for surges
        }

        public Object Clone() {
            return new Abillity(this.Val, this.Type, this.SurgePrice, this.StaminaPrice);
        }

        public override string ToString() {
            return $"{this.Val} {this.Type}, Cost: {this.SurgePrice} Surges and {this.StaminaPrice} Stamina";
        }
    }
}
