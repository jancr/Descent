using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

// local imports
using DescentCore.Units;
using DescentCore.Dice;
using DescentCore.Abillites;
using DescentCore.Equipment;

namespace DescentCoreCore.UnitTest {
    // high level test, other test should have been written first!!!
    public class DescentCore_AttackResolver {

        public static Hero GetLeoric() {
            var defence = new DefenceDice(new GreyDie());
            var leoric = new Hero("Leoric", 4, 8, 5, defence, 1, 5, 3, 2, 0);

            // make item
            // var attack = new AttackDice{ new BlueDie(), new PowerDie(), new YellowDie() };
            var attack = new AttackDice("blue", "power", "yellow");
            var abillities = new Abillity[] { 
                new Abillity(3, AbillityType.Damage, 2), 
                new Abillity(1, AbillityType.Damage, 1), 
                new Abillity(2, AbillityType.Pierce, 1),
                new Abillity(2, AbillityType.Range, 1) };
            var catagories = new ItemCatagory[] { ItemCatagory.Staff, 
                                                  ItemCatagory.Magic };

            WeoponItem fancyStaff = new WeoponItem("Fancy Staff", attack,
                    HandCatagory.RangeWeopon, abillities, catagories, 2);
            leoric.Equip(fancyStaff);

            return leoric;
        }

        public static Monster GetWhiteGoblinActII() {
            // act 2 goblins
            var whiteGoblin = new Monster("White Goblin II", 5, 4, 
                    new AttackDice(new BlueDie(), new YellowDie()),
                    new DefenceDice(new GreyDie()));
            whiteGoblin.Abillities.Add(new Abillity(2, AbillityType.Range, 1));
            whiteGoblin.Abillities.Add(new Abillity(2, AbillityType.Damage, 1));
                    
            return whiteGoblin;
        }

        public static IEnumerable<object[]> 
        // public static IEnumerable<(AttackDieFace, DefenceDieFace, int, int)> 
            GetResolveSurgesData(int numTests) {
            var allData = new List<object[]> {
            // var allData = new List<(AttackDieFace, DefenceDieFace, int, int)> {

                // shield tests
                new object[] { new AttackDieFace(5, 0, 5), 
                               new DefenceDieFace(2), 3, 3 },
                new object[] { new AttackDieFace(7, 0, 5),
                               new DefenceDieFace(2), 3, 5 },
                // range tests
                new object[] { new AttackDieFace(5, 0, 4),
                               new DefenceDieFace(0), 6, 0 },
                new object[] { new AttackDieFace(5, 0, 6),
                               new DefenceDieFace(0), 6, 5 },
                new object[] { new AttackDieFace(5, 0, 8),
                               new DefenceDieFace(0), 6, 5 },
                // surges
                //  - pierce
                new object[] { new AttackDieFace(5, 1, 5),
                               new DefenceDieFace(1), 3, 5 },
                new object[] { new AttackDieFace(5, 1, 5),
                               new DefenceDieFace(2), 3, 5 },
                //  - range
                new object[] { new AttackDieFace(5, 1, 4),
                               new DefenceDieFace(0), 6, 5 },
                //  - combo
                new object[] { new AttackDieFace(5, 3, 5),
                               new DefenceDieFace(2), 3, 8 },
                new object[] { new AttackDieFace(5, 3, 5),
                               new DefenceDieFace(0), 3, 9 },

                new object[] { new AttackDieFace(5, 3, 5),
                               new DefenceDieFace(2), 7, 6 },
                new object[] { new AttackDieFace(5, 2, 5),
                               new DefenceDieFace(2), 7, 5 },
            };

            return allData.Take(numTests);
        }

        [Theory]
        // [MemberData(nameof(GetResolveSurgesData), parameters: 1)]
        // [MemberData(nameof(GetResolveSurgesData), parameters: 6)]
        [MemberData(nameof(GetResolveSurgesData), parameters: 12)]
        public void ResolveSurges(AttackDieFace attackRoll, 
                                  DefenceDieFace defenceRoll, int range,
                                  int expectedDamage) {
            Hero leoric = GetLeoric();

            // whiteGoblin = this.GetWhiteGoblinActII();
            // var ar = new AttackResolver(leoric.Abillities);
            // AttackDieFace attack = ar.ResolveSurges(attackRoll, defenceRoll, range);
            var dice = new DiceOutcome(attackRoll, defenceRoll);
            int damage = leoric.ResolveSurges(dice, range);
            Assert.Equal(expectedDamage, damage);

            if (expectedDamage != 0) {
                dice = new DiceOutcome(attackRoll, defenceRoll);
                TrinketItem damageTrinket = new TrinketItem("Free Damage", 
                        new ItemCatagory[] { ItemCatagory.Helmet },
                        new Abillity[] { new Abillity(1, AbillityType.Damage, 0) });
                leoric.Equip(damageTrinket);

                damage = leoric.ResolveSurges(dice, range);
                Assert.Equal(expectedDamage + 1, damage);
            }
        }
    }
        
    public class DescentCore_Equipment {
        [Fact]
        public void TestGearFactory() {
            Abillity[] a;
            var gf = new GearFactory("ActI");

            a = new Abillity[] {
                    new Abillity(1, AbillityType.Range, 0, 0),
                    new Abillity(1, AbillityType.Damage, 1, 0),
                    new Abillity(1, AbillityType.Pierce, 1, 0), };
            var mainHand = new WeoponItem(
                    "Iron Spear", new AttackDice("blue", "yellow"), 
                    HandCatagory.MeleeWeopon, a,
                    new ItemCatagory[] { ItemCatagory.Exotic }, 1);
            Assert.Equal(mainHand, gf.MainHand[0]);

            var offHand = new ShieldItem("Iron Shield", new Abillity[0]);
            Assert.Equal(offHand, gf.OffHand[0]);

            var armor = new ArmorItem("Chainmail", new DefenceDice("grey"),
                ItemCatagory.HeavyArmor, new Abillity[0]);
            Assert.Equal(armor, gf.Armor[0]);

            a = new Abillity[] { new Abillity(1, AbillityType.Range, 0, 0) };
            var helmet = new ItemCatagory[] { ItemCatagory.Helmet };
            var trinket = new TrinketItem("Scorpion Helmet", helmet, a);
            Assert.Equal(trinket, gf.Trinket[0]);
        }

    }
}
