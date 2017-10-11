using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine {
    using global::System.Diagnostics;

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

        public static Stat operator +(Stat a, Stat b) {
            Debug.Assert(a.Id == b.Id);
            return new Stat(a.Id, a.Value + b.Value);
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

    public class Levels {
        public static int NextLevelXP(int level) {
            const float exponent = 1.5f;
            var baseXP = 1000;
            return (int)Math.Floor(baseXP * Math.Pow(level, exponent));
        }
    }
}
