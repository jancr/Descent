using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DescentCore.Dice;
using DescentCore.Equipment;
using DescentCore.Abillites;

namespace DescentCore.Attack {
    public class AttackResolver {
        private List<AttackDie> attackDice;
        private List<DefenceDie> defenceDice;
        private List<Abillity> abillities;

        public AttackResolver(List<Abillity> abillities) {
            this.attackDice = new List<AttackDie>();
            this.defenceDice = new List<DefenceDie>();
            this.abillities = abillities.Select(a => (Abillity)a.Clone()).ToList();
        }

        public AttackResolver(List<AttackDie> attack, List<DefenceDie> defence, List<Abillity> abillities) {
            this.attackDice = attack;
            this.defenceDice = defence;
            this.abillities = abillities.Select(a => (Abillity)a.Clone()).ToList();
        }

        public int RollDamage(int range) {
            AttackDieFace attack = new AttackDieFace();
            foreach (AttackDie attackDie in attackDice) {
                attack += attackDie.Roll();
            }
            Console.WriteLine($"Total Attack: {attack}");
            DefenceDieFace defence = new DefenceDieFace();
            foreach (DefenceDie defenceDie in defenceDice) {
                defence += defenceDie.Roll();
            }
            Console.WriteLine($"Total Defence: {defence}");
            attack = ResolveSurges(attack, defence, range);
            return attack.Power;
        }

        public int[] DamageHistogram(int range) {
            throw new NotImplementedException("TODO!!!");
        }

        public AttackDieFace ResolveSurges(AttackDieFace attackRoll, DefenceDieFace defenceRoll,
                                           int range) {
            if (!attackRoll.Hit) {
                return attackRoll;
            }
            Console.WriteLine("--------------------");
            Console.WriteLine($"step 0\nAttack: {attackRoll}\n Defence: {defenceRoll}\n");
            AbillityResult abillityResult;
            DefenceDieFace defence = defenceRoll.Clone();
            AttackDieFace attack = attackRoll.Clone();

            // 1. apply all "free abillities"
            foreach (Abillity abillity in abillities) {
                if (abillity.SurgePrice == 0) {
                    abillityResult = abillity.PotentialDamage(defence);
                    attack.Power += abillityResult.Damage;
                    defence = abillityResult.Defence;
                }
            }
            Console.WriteLine($"step 1\nAttack: {attackRoll}\n Defence: {defenceRoll}\n");
            
            // 2. fix range if to short
            if (attack.Range < range) {
                attack = IncreaseRange(attack, range);
                if (!attack.Hit) {
                    return attack;
                }
            }
            Console.WriteLine($"step 2\nAttack: {attackRoll}\n Defence: {defenceRoll}\n");
            
            // 3. sort based on damage / surge
            attack = UseSurges(attack, defence);
            //
            // reset abillities
            this.abillities.ForEach(a => a.Used = false);

            attack.Power -= defence.Shield;
            return attack;
        }

        protected AttackDieFace UseSurges(AttackDieFace attack, DefenceDieFace defence) {
            while (attack.Surge != 0) {
                // Does Max raise an error if the list is empty or return zero???
                Abillity best = (from a in abillities 
                                 where !a.Used && attack.Surge >= a.SurgePrice
                                 let ar = a.PotentialDamage(defence)
                                // most damage/surge, tiebreak on defence
                                 orderby ar.Damage / a.SurgePrice descending,
                                         ar.Defence descending
                                 select a).FirstOrDefault();

                if (best == null) {
                    return attack;
                } else {
                    AbillityResult abillityResult = best.PotentialDamage(defence);
                    attack.Power += abillityResult.Damage;
                    defence = abillityResult.Defence;
                    best.Used = true;
                }
            }
            return attack;
        }


        protected AttackDieFace IncreaseRange(AttackDieFace attack, int range) {
            // bestAbillity, an abillity that makes the shot
            // effectiveAbillity, the abillity with most bang for the buck
            // there could be two abillities
            // 1) 2 surges -> 3 range
            // 2) 1 surge -> 1 range
            // if you only need 1 range 2 is best, if you need 2 or 3 then 1) is best
            // if you need 4, then 1) has the most range per surge
            Abillity bestAbillity = null;
            Abillity effectiveAbillity = null;
            var rangeAbillities = from a in abillities where a.Type == AbillityType.Range select a;
            while (attack.Surge > 0 && range < attack.Range) {
                foreach (var abillity in rangeAbillities) {
                    if (!abillity.Used) {
                        if (abillity.Val + attack.Range >= range) {
                            if (bestAbillity == null) {
                                bestAbillity = abillity;
                            } else if (abillity.SurgePrice < bestAbillity.SurgePrice) {
                                bestAbillity = abillity;
                            }
                        }
                        if ((((float) abillity.Val) / abillity.SurgePrice) >
                            ((float) effectiveAbillity.Val / effectiveAbillity.SurgePrice)) {
                            effectiveAbillity = abillity;
                        }
                    }
                }
                if (bestAbillity != null) {
                    return attack + new AttackDieFace(0, -bestAbillity.SurgePrice, bestAbillity.Val);
                } else if (effectiveAbillity != null) {
                    attack += new AttackDieFace(0, -effectiveAbillity.SurgePrice, 
                                             effectiveAbillity.Val);
                    effectiveAbillity.Used = true;
                } else {
                    // not enugh range abillities
                    return AttackDieFace.Mis();
                }
            }
            // spent to manny surges
            return AttackDieFace.Mis();
        }
    }
}
