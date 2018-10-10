using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

// local imports
using DescentCore.Units;
using DescentCore.Dice;
using DescentCore.Abillites;
using DescentCore.Equipment;
using DescentCore.Attack;

namespace DescentCoreCore.UnitTest {
    // high level test, other test should have been written first!!!
    public class DescentCore_AttackResolver {

        public static Hero GetLeoric() {
            var defence = new DefenceDice(new GreyDie());
            var leoric = new Hero("Leoric", 4, 8, 5, defence, 1, 5, 3, 2, 0);

            // make item
            // var attack = new AttackDice{ new BlueDie(), new PowerDie(), new YellowDie() };
            var attack = new AttackDice("blue", "power", "yellow");
            var abillities = new Abillity[] { new Abillity(3, AbillityType.Damage, 2),
                                              new Abillity(1, AbillityType.Damage, 1),
                                              new Abillity(2, AbillityType.Pierce, 1),
                                              new Abillity(2, AbillityType.Range, 1) };
            var catagories = new ItemCatagory[] { ItemCatagory.Staff, ItemCatagory.Magic };

            WeoponItem fancyStaff = new WeoponItem("fancy Staff", attack, HandCatagory.RangeWeopon, abillities, catagories, 2);
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
                    
            // var redGoblin = new Monster("Red Goblin II", 5, 6, 
                    // new List<AttackDie> { new BlueDie(), new YellowDie(), new YellowDie() },
                    // new List<DefenceDie> { new GreyDie() });
            // redGoblin.Abillities.Add(new Abillity(3, AbillityType.Range, 1));
            // redGoblin.Abillities.Add(new Abillity(2, AbillityType.Damage, 1));
            return whiteGoblin;
        }

        public static IEnumerable<object[]> 
        // public static IEnumerable<(AttackDieFace, DefenceDieFace, int, int)> 
            GetResolveSurgesData(int numTests) {
            var allData = new List<object[]> {
            // var allData = new List<(AttackDieFace, DefenceDieFace, int, int)> {

                // shield tests
                new object[] { new AttackDieFace(5, 0, 5), new DefenceDieFace(2), 3, 3 },
                new object[] { new AttackDieFace(7, 0, 5), new DefenceDieFace(2), 3, 5},
                // range tests
                new object[] { new AttackDieFace(5, 0, 4), new DefenceDieFace(0), 6, 0},
                new object[] { new AttackDieFace(5, 0, 6), new DefenceDieFace(0), 6, 5},
                new object[] { new AttackDieFace(5, 0, 8), new DefenceDieFace(0), 6, 5},
                // surges
                //  - pierce
                new object[] { new AttackDieFace(5, 1, 5), new DefenceDieFace(1), 3, 5},
                new object[] { new AttackDieFace(5, 1, 5), new DefenceDieFace(2), 3, 5},
                //  - range
                new object[] { new AttackDieFace(5, 1, 4), new DefenceDieFace(0), 6, 5},
                //  - combo
                new object[] { new AttackDieFace(5, 3, 5), new DefenceDieFace(2), 3, 8},
                new object[] { new AttackDieFace(5, 3, 5), new DefenceDieFace(0), 3, 9},
                new object[] { new AttackDieFace(5, 3, 5), new DefenceDieFace(2), 7, 6},
                new object[] { new AttackDieFace(5, 2, 5), new DefenceDieFace(2), 7, 5},
            };

            return allData.Take(numTests);
        }

        [Theory]
        // [MemberData(nameof(GetResolveSurgesData), parameters: 1)]
        // [MemberData(nameof(GetResolveSurgesData), parameters: 6)]
        [MemberData(nameof(GetResolveSurgesData), parameters: 12)]
        public void ResolveSurges(AttackDieFace attackRoll, DefenceDieFace defenceRoll, 
                               int range, int expectedDamage) {
            Hero leoric = GetLeoric();
            // whiteGoblin = this.GetWhiteGoblinActII();
            // var ar = new AttackResolver(leoric.Abillities);
            // AttackDieFace attack = ar.ResolveSurges(attackRoll, defenceRoll, range);
            var dice = new DiceOutcome(attackRoll, defenceRoll);
            leoric.ResolveSurges(dice, range);
            Console.WriteLine($"Damage: {dice.Damage}, Expected {expectedDamage}");
            Assert.Equal(dice.Damage, expectedDamage);

        }

        // static void TestUnits() {
// 
            // 
            // // TODO 
            // //   1. make Hero
            // Hero leoric = GetHero();
            // //   2. hero attack monster
            // //      2a. he needs items for this to work
            // //
            // // int damage = leoric.Attack(whiteGoblin, 3);
            // var ar = new AttackResolver(leoric.AttackDice, whiteGoblin.DefenceDice, 
                                        // leoric.Abillities);
            // var leoricAttack = new AttackDieFace(5, 2, 5);
            // var goblinDefence = new DefenceDieFace(2);
            // Console.WriteLine($"Leoric Attack: {leoricAttack}");
            // Console.WriteLine($"goblin Defence: {goblinDefence}");
            // Console.WriteLine($"Expectation: 5 + 3 - 2 = 6");
            // var attack = ar.ResolveSurges(leoricAttack, goblinDefence, 3);
            // Console.WriteLine($"Damage: {attack.Power}");
            // //   3. monster attack hero
            // //      3a. elite monster equip item
            // //   4. Make naive "Exhaust algorithmn"
            // //   5. collaps Dice to make algorithmn better
            // //   ...
            // //  10 Make Monsters and Heros loadable from "flat files"
            // //  so I don't have to create them in "code"
            // //   ...
            // //   20. Make Xamarian App
        // }
// 
        // public static Hero GetHero() {
            // var defence = new List<DefenceDie>() { new GreyDie() };
            // var leoric = new Hero("Leoric", 4, 8, 5, defence, 1, 5, 3, 2, 0);
// 
            // // make item
            // AttackDie[] attack = { new BlueDie(), new PowerDie(), new YellowDie() };
            // var abillities = new Abillity[] { new Abillity(3, AbillityType.Damage, 2),
                                              // new Abillity(2, AbillityType.Range, 1) };
            // var catagories = new ItemCatagory[] { ItemCatagory.Staff, ItemCatagory.Magic };
// 
            // WeoponItem fancyStaff = new WeoponItem(attack, HandCatagory.RangeWeopon, abillities, catagories, 2);
            // leoric.Equip(fancyStaff);
// 
            // return leoric;
        // }
    }
}
