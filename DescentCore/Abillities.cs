using System;
// using System.Collections;
// using System.Collections.Generic;

using DescentCore.Dice;

namespace DescentCore.Abillites { 
    public enum AbillityType {
        Damage, Pierce, Blast, Range
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
        // TODO: refactor into propperties
        public int Val { get; private set; }
        public AbillityType Type { get; private set; }
        public int SurgePrice { get; private set; }
        public int StaminaPrice { get; private set; }
        public bool Used { get; set; }

        public Abillity(int val, AbillityType type=AbillityType.Damage, int surgePrice=0, 
                int staminaPrice=0) {
            this.Val = val;
            this.Type = type;
            this.SurgePrice = surgePrice;
            this.Used = false;
            this.StaminaPrice = staminaPrice;  // not fully implemented
        }

        public AbillityResult PotentialDamage(DefenceDieFace defence) {
            // TODO: can there be abillities that trigger multiple things?
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
