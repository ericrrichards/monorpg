namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Text.RegularExpressions;

    public static class Growth {
        public static readonly Dice Fast = new Dice("3d2");
        public static readonly Dice Med = new Dice("1d3");
        public static readonly Dice Slow = new Dice("1d2");

        public static IReadOnlyDictionary<string, Dice> Curves = new Dictionary<string, Dice> {
            ["fast"] = Fast,
            ["med"] = Med,
            ["slow"] = Slow
        };
    }

    public class Dice {
        

        private readonly List<Die> _dice;
        private static readonly Regex DieRegex = new Regex(@"(\d+)[D|d](\d+)[\+]?(\d+)?");
        private static readonly Random Random = new Random();

        public Dice(string diceStr) {
            _dice = Parse(diceStr);
        }

        public static List<Die> Parse(string diceStr) {
            
            var allDice = new List<Die>();

            var matches = DieRegex.Matches(diceStr);
            if (matches.Count > 0) {
                foreach (Match match in matches) {
                    if (match.Success) {
                        var rolls = Convert.ToInt32(match.Groups[1].Value);
                        var sides = Convert.ToInt32(match.Groups[2].Value);
                        var plus = match.Groups[3].Success ? Convert.ToInt32(match.Groups[3].Value) : 0;
                        allDice.Add(new Die(rolls, sides, plus));
                    }
                }
            }
            return allDice;
        }

        public int Roll() {
            return _dice.Sum(die => die.Roll());
        }

        public class Die {
            public int Rolls { get; }
            public int Sides { get; }
            public int Plus { get; }
            public Die(int rolls, int sides, int plus) {
                Rolls = rolls;
                Sides = sides;
                Plus = plus;
            }

            public int Roll() {
                return RollDie(Rolls, Sides, Plus);
            }

            public static int RollDie(int rolls, int faces, int modifier = 0) {
                var total = 0;
                for (var i = 0; i < rolls; i++) {
                    total += Random.Next(1, faces);
                }
                return total + modifier;
            }
        }
    }
}