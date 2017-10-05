using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine {
    using global::System.Text.RegularExpressions;

    public class Stats {
        public const string HitPoints = "hp";
        public const string MaxHitPoints = "max_hp";
        public const string MagicPoints = "mp";
        public const string MaxMagicPoints = "max_mp";
        public const string Strength = "strength";
        public const string Speed = "speed";
        public const string Intelligence = "intelligence";


        public Dictionary<string, Stat> Base { get; set; }
        public Dictionary<string, Modifier> Modifiers { get; set; }

        public Stats(params Stat[] stats) {
            Base = stats.ToDictionary(kv => kv.Id, kv => kv);
            Modifiers = new Dictionary<string, Modifier>();
        }

        public Stat GetBase(string statId) {
            if (Base.ContainsKey(statId)) {
                return Base[statId];
            }
            return new Stat(statId, 0);
        }

        public void Set(string id, float value) {
            Base[id] = new Stat(id, value);
        }

        public void AddModifier(Modifier modifier) {
            Modifiers[modifier.Name] = modifier;
        }

        public void RemoveModifier(string id) {
            Modifiers.Remove(id);
        }

        public Stat GetStat(string id) {
            var total = 0f;
            var multiplier = 0f;
            if (Base.ContainsKey(id)) {
                total = Base[id].Value;
            }

            foreach (var modifier in Modifiers.Values) {
                total += modifier.Add.Where(m => m.Id == id).Select(m => m.Value).FirstOrDefault();
                multiplier += modifier.Mult.Where(m => m.Id == id).Select(m => m.Value).FirstOrDefault();
            }

            return new Stat(id, total + total * multiplier);
        }
    }

    public class Stat {
        public string Id { get; set; }
        public float Value { get; set; }
        public Stat() { }

        public Stat(string statId, float value) {
            Id = statId;
            Value = value;
        }
    }

    public class Modifier {
        public string Name { get; set; }
        public List<Stat> Add { get; set; }
        public List<Stat> Mult { get; set; }

        public Modifier() {
            Add = new List<Stat>();
            Mult = new List<Stat>();
        }
        public Modifier(string name) : this(name, new List<Stat>(), new List<Stat>()) {
        }

        public Modifier(string name, List<Stat> add, List<Stat> mult) {
            Name = name;
            Add = add ?? new List<Stat>();
            Mult = mult ?? new List<Stat>();
        }
    }

    public class Level {
        public int NextLevelXP(int level) {
            const float exponent = 1.5f;
            var baseXP = 1000;
            return (int)Math.Floor(baseXP * Math.Pow(level, exponent));
        }
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

        public static int RollDie(int rolls, int faces, int modifier = 0) {
            var total = 0;
            for (var i = 0; i < rolls; i++) {
                total += Random.Next(1, faces);
            }
            return total + modifier;
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
                return Dice.RollDie(Rolls, Sides, Plus);
            }
        }
    }
}
