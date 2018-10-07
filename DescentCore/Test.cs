using System;
using System.Collections.Generic;

// local imports
using DescentCore.Units;
using DescentCore.Dice;
using DescentCore.Abillites;
using DescentCore.Equipment;
using DescentCore.Attack;

namespace DescentCore.Test {
    class Test {
        static void Main(string[] args) {
            Console.WriteLine("TESTING TESTING...");

            TestUnits();
        }

        static void TestUnits() {
            // act 2 goblins
            var whiteGoblin = new Monster("White Goblin II", 5, 4, 
                    new List<AttackDie> { new BlueDie(), new YellowDie() },
                    new List<DefenceDie> { new GreyDie() });
            whiteGoblin.Abillities.Add(new Abillity(2, AbillityType.Range, 1));
            whiteGoblin.Abillities.Add(new Abillity(2, AbillityType.Damage, 1));
                    
            var redGoblin = new Monster("Red Goblin II", 5, 6, 
                    new List<AttackDie> { new BlueDie(), new YellowDie(), new YellowDie() },
                    new List<DefenceDie> { new GreyDie() });
            redGoblin.Abillities.Add(new Abillity(3, AbillityType.Range, 1));
            redGoblin.Abillities.Add(new Abillity(2, AbillityType.Damage, 1));

            
            // TODO 
            //   1. make Hero
            Hero leoric = GetHero();
            //   2. hero attack monster
            //      2a. he needs items for this to work
            //
            int damage = leoric.Attack(whiteGoblin, 3);
            // var ar = new AttackResolver(leoric.AttackDice, whiteGoblin.DefenceDice, 
                                        // leoric.Abillities);
            // var leoricAttack = new AttackDieFace(5, 2, 5);
            // var goblinDefence = new DefenceDieFace(2);
            // Console.WriteLine($"Leoric Attack: {leoricAttack}");
            // Console.WriteLine($"goblin Defence: {goblinDefence}");
            // Console.WriteLine($"Expectation: 5 + 3 - 2 = 6");
            // var attack = ar.ResolveSurges(leoricAttack, goblinDefence, 3);
            // Console.WriteLine($"Damage: {attack.Power}");
            //   3. monster attack hero
            //      3a. elite monster equip item
            //   4. Make naive "Exhaust algorithmn"
            //   5. collaps Dice to make algorithmn better
            //   ...
            //  10 Make Monsters and Heros loadable from "flat files"
            //  so I don't have to create them in "code"
            //   ...
            //   20. Make Xamarian App
        }

        public static Hero GetHero() {
            var defence = new List<DefenceDie>() { new GreyDie() };
            var leoric = new Hero("Leoric", 4, 8, 5, defence, 1, 5, 3, 2, 0);

            // make item
            AttackDie[] attack = { new BlueDie(), new PowerDie(), new YellowDie() };
            var abillities = new Abillity[] { new Abillity(3, AbillityType.Damage, 2),
                                              new Abillity(2, AbillityType.Range, 1) };
            var catagories = new ItemCatagory[] { ItemCatagory.Staff, ItemCatagory.Magic };

            WeoponItem fancyStaff = new WeoponItem(attack, HandCatagory.RangeWeopon, abillities, catagories, 2);
            leoric.Equip(fancyStaff);

            return leoric;
        }
    }
}
