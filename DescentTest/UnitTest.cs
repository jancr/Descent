using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

// local imports
using DescentCore.Units;
using DescentCore.Dice;
using DescentCore.Abilites;
using DescentCore.Equipment;

namespace DescentTest.UnitTest {
    public class DescentCore_AttackResolver {

        public static Hero GetLeoric() {
            var defence = new DefenceDice(new GreyDie());
            var leoric = new Hero("Leoric", 4, 8, 5, defence, 1, 5, 3, 2, 0);

            var attack = new AttackDice("blue", "power", "yellow");
            var abilities = new Ability[] { 
                new Ability(3, AbilityType.Damage, 2), 
                new Ability(1, AbilityType.Damage, 1), 
                new Ability(2, AbilityType.Pierce, 1),
                new Ability(2, AbilityType.Range, 1) };
            var catagories = new ItemCatagory[] { ItemCatagory.Staff, 
                                                  ItemCatagory.Magic };

            WeoponItem fancyStaff = new WeoponItem("Fancy Staff", attack,
                    HandCatagory.RangeWeopon, abilities, catagories, 2);
            leoric.Equip(fancyStaff);

            return leoric;
        }

        public static Monster GetWhiteGoblinActII() {
            // act 2 goblins
            var whiteGoblin = new Monster("White Goblin II", 5, 4, 
                    new AttackDice(new BlueDie(), new YellowDie()),
                    new DefenceDice(new GreyDie()));
            whiteGoblin.Abilities.Add(new Ability(2, AbilityType.Range, 1));
            whiteGoblin.Abilities.Add(new Ability(2, AbilityType.Damage, 1));
                    
            return whiteGoblin;
        }

        public static IEnumerable<object[]> GetResolveSurgesData(int numTests) {
            var allData = new List<object[]> {

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
        [MemberData(nameof(GetResolveSurgesData), parameters: 12)]
        public void ResolveSurges(AttackDieFace attackRoll, 
                                  DefenceDieFace defenceRoll, int range,
                                  int expectedDamage) {
            Hero leoric = GetLeoric();

            var dice = new DiceOutcome(attackRoll, defenceRoll);
            int damage = leoric.ResolveSurges(dice, range);
            Assert.Equal(expectedDamage, damage);

            if (expectedDamage != 0) {
                dice = new DiceOutcome(attackRoll, defenceRoll);
                TrinketItem damageTrinket = new TrinketItem("Free Damage", 
                        new ItemCatagory[] { ItemCatagory.Helmet },
                        new Ability[] { new Ability(1, AbilityType.Damage, 0) });
                leoric.Equip(damageTrinket);

                damage = leoric.ResolveSurges(dice, range);
                Assert.Equal(expectedDamage + 1, damage);
            }
        }
    }
        
    public class DescentCore_Equipment {
        [Fact]
        public void TestAbillity() {
            var ability1 = new Ability(3, AbilityType.Damage, 2); 
            var ability2 = new Ability(3, AbilityType.Damage, 2);
            Assert.Equal(ability1, ability2);
        }

        [Fact]
        public void TestDie() {
            var grey1 = new GreyDie();
            var grey2 = new GreyDie();
            Assert.Equal(grey1, grey2);
        }

        [Fact]
        public void TestArmor() {
            var armor1 = new ArmorItem("Chainmail", new DefenceDice("grey"),
                ItemCatagory.HeavyArmor, new Ability[0]);
            var armor2 = new ArmorItem("Chainmail", new DefenceDice("grey"),
                ItemCatagory.HeavyArmor, new Ability[0]);
            Assert.Equal(armor1, armor2);
        }

        [Fact]
        public void TestWeopon() {
            Ability[] a = new Ability[] {
                    new Ability(1, AbilityType.Range, 0, 0),
                    new Ability(1, AbilityType.Damage, 1, 0),
                    new Ability(1, AbilityType.Pierce, 1, 0), };
            var mainHand1 = new WeoponItem(
                    "Iron Spear", new AttackDice("blue", "yellow"), 
                    HandCatagory.MeleeWeopon, a,
                    new ItemCatagory[] { ItemCatagory.Exotic }, 1);
            var mainHand2 = new WeoponItem(
                    "Iron Spear", new AttackDice("blue", "yellow"), 
                    HandCatagory.MeleeWeopon, a,
                    new ItemCatagory[] { ItemCatagory.Exotic }, 1);
            Assert.Equal(mainHand1, mainHand2);
        }

        [Fact]
        public void TestTrinket() {
            Ability[] a = new Ability[] { new Ability(1, AbilityType.Range, 0, 0) };
            var helmet = new ItemCatagory[] { ItemCatagory.Helmet };
            var trinket1 = new TrinketItem("Scorpion Helmet", helmet, a);
            var trinket2 = new TrinketItem("Scorpion Helmet", helmet, a);
            Assert.Equal(trinket1, trinket2);
        }

        [Fact]
        public void TestGearFactory() {
            Ability[] a;
            var gf = new GearFactory("ActI");

            a = new Ability[] {
                    new Ability(1, AbilityType.Range, 0, 0),
                    new Ability(1, AbilityType.Damage, 1, 0),
                    new Ability(1, AbilityType.Pierce, 1, 0), };
            var mainHand = new WeoponItem(
                    "Iron Spear", new AttackDice("blue", "yellow"), 
                    HandCatagory.MeleeWeopon, a,
                    new ItemCatagory[] { ItemCatagory.Exotic }, 1);
            Assert.Equal(mainHand, gf.MainHand[0]);

            var offHand = new ShieldItem("Iron Shield", new Ability[0]);
            Assert.Equal(offHand, gf.OffHand[0]);

            var armor = new ArmorItem("Chainmail", new DefenceDice("grey"),
                ItemCatagory.HeavyArmor, new Ability[0]);

            a = new Ability[] { new Ability(1, AbilityType.Range, 0, 0) };
            var helmet = new ItemCatagory[] { ItemCatagory.Helmet };
            var trinket = new TrinketItem("Scorpion Helmet", helmet, a);
            Assert.Equal(trinket, gf.Trinket[0]);
        }

    }
}
