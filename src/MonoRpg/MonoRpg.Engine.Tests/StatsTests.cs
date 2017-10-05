using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine.Tests {
    using NUnit.Framework;

    [TestFixture]
    public class StatsTests {
        private static readonly Func<Stats> Stats = ()=>new Stats(new Stat(Engine.Stats.Strength, 10));

        [Test]
        public void BaseStength() {
            Assert.AreEqual(10, Stats().GetStat(Engine.Stats.Strength).Value);
        }

        [Test]
        public void StrengthWithOneAddModifier() {
            var stats = Stats();
            stats.AddModifier(new Modifier("magicHat", new List<Stat>{new Stat(Engine.Stats.Strength, 5)}, null ));
            Assert.AreEqual(15, stats.GetStat(Engine.Stats.Strength).Value);
        }

        [Test]
        public void StengthWithTwoAddModifiers() {
            var stats = Stats();
            stats.AddModifier(new Modifier("magicHat", new List<Stat> { new Stat(Engine.Stats.Strength, 5) }, null));
            stats.AddModifier(new Modifier("magicSword", new List<Stat> { new Stat(Engine.Stats.Strength, 5) }, null));
            Assert.AreEqual(20, stats.GetStat(Engine.Stats.Strength).Value);

        }

        [Test]
        public void StrengthWithMultModifier() {
            var stats = Stats();
            stats.AddModifier(new Modifier("bravery", null, new List<Stat>{new Stat(Engine.Stats.Strength, 0.1f)}));
            Assert.AreEqual(11, stats.GetStat(Engine.Stats.Strength).Value);
        }
        [Test]
        public void StengthWithTwoAddOneMultModifiers() {
            var stats = Stats();
            stats.AddModifier(new Modifier("magicHat", new List<Stat> { new Stat(Engine.Stats.Strength, 5) }, null));
            stats.AddModifier(new Modifier("magicSword", new List<Stat> { new Stat(Engine.Stats.Strength, 5) }, null));
            stats.AddModifier(new Modifier("bravery", null, new List<Stat> { new Stat(Engine.Stats.Strength, 0.1f) }));
            Assert.AreEqual(22, stats.GetStat(Engine.Stats.Strength).Value);

        }
        [Test]
        public void StengthWithOneMultDebuffModifiers() {
            var stats = Stats();
            stats.AddModifier(new Modifier("curse", null, new List<Stat> { new Stat(Engine.Stats.Strength, -0.5f) }));
            Assert.AreEqual(5, stats.GetStat(Engine.Stats.Strength).Value);

        }
    }
}
