using System;
using System.Text.RegularExpressions;

using DescentCore.Dice;

namespace DescentCore.Abilites { 
    public enum AbilityType {
        Damage, Pierce, Range, 
        Blast, Reach, Stun  // <- TODO: Implement!
    }
    //////////////////////////////////////////////////////////////////////
    // Ability
    //////////////////////////////////////////////////////////////////////
    public struct AbilityResult {
        public int Damage { get; private set; }
        public AttackDieFace Attack { get; private set; }
        public DefenceDieFace Defence { get; private set; }
        public AbilityResult(int damage, int attack, int defence) {
            this.Damage = damage;
            this.Attack = new AttackDieFace(attack);
            this.Defence = new DefenceDieFace(defence);
        }
    }

    public class Ability : ICloneable {
        public int Val { get; private set; }
        public AbilityType Type { get; private set; }
        public int SurgePrice { get; private set; }
        public int StaminaPrice { get; private set; }
        public bool Used { get; set; } = false;

        public override bool Equals(object obj) {
            Ability other = obj as Ability;
            if (other != null) {
                if ((this.Val == other.Val) && (this.Type == other.Type) &&
                        (this.SurgePrice == other.SurgePrice) &&
                        (this.StaminaPrice == other.StaminaPrice)) {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() {
            return (Val.GetHashCode() ^ Type.GetHashCode() ^
                    SurgePrice.GetHashCode() ^ StaminaPrice.GetHashCode() ^
                    Used.GetHashCode());
        }

        // helper fields
        private Regex abilityRegex = new Regex(@"(\d+)([LS]?):(\d+)([DPR])");

        public Ability(int val, AbilityType type=AbilityType.Damage, int surgePrice=0, 
                int staminaPrice=0) {
            this.Val = val;
            this.Type = type;
            this.SurgePrice = surgePrice;
            this.StaminaPrice = staminaPrice;  // not fully implemented
        }

        public Ability(string abilityString) {
            Match m = abilityRegex.Match(abilityString);
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
                                "Stamina abilities not implemented Yet");
                    }
                }

                this.Val = Int32.Parse(m.Groups[3].ToString());
                char ValType = m.Groups[4].ToString()[0];
                switch (ValType) {
                    case 'D':
                        this.Type = AbilityType.Damage;
                        break;
                    case 'P':
                        this.Type = AbilityType.Pierce;
                        break;
                    case 'R':
                        this.Type = AbilityType.Range;
                        break;
                }
            } else {
                throw new ArgumentException(
                        $"abilityString {abilityString} is not a valid regex");
            }
        }

        public AbilityResult PotentialDamage(DefenceDieFace defence) {
            // TODO: can there be abilities that trigger multiple things?
            //       - there are a few, but it is out of the scope of the project
            // if so I should be a boolian class like EquipedItems!!!
            // and this should be a switch statment
            if (this.Type == AbilityType.Damage) {
                return new AbilityResult(this.Val, this.Val, defence.Shield);
            } else if (this.Type == AbilityType.Pierce) {
                int pierce = Math.Min(this.Val, defence.Shield);
                return new AbilityResult(pierce, 0, defence.Shield - pierce);
            }
            return new AbilityResult(0, 0, defence.Shield);
            // TODO support other effects, such as tab for surges
        }

        public Object Clone() {
            return new Ability(this.Val, this.Type, this.SurgePrice, this.StaminaPrice);
        }

        public override string ToString() {
            return $"{this.Val} {this.Type}, Cost: {this.SurgePrice} Surges and {this.StaminaPrice} Stamina";
        }
    }
}
