namespace MonoRpg.Engine.Tests {
    using NUnit.Framework;

    [TestFixture]
    public class DiceTests {
        [Test]
        public void Parse_1D6() {
            var dice = Dice.Parse("1D6");
            Assert.AreEqual(1, dice.Count);
            Assert.AreEqual(1, dice[0].Rolls);
            Assert.AreEqual(6, dice[0].Sides);
            Assert.AreEqual(0, dice[0].Plus);
        }
        [Test]
        public void Parse_2D6() {
            var dice = Dice.Parse("2D6");
            Assert.AreEqual(1, dice.Count);
            Assert.AreEqual(2, dice[0].Rolls);
            Assert.AreEqual(6, dice[0].Sides);
            Assert.AreEqual(0, dice[0].Plus);
        }
        [Test]
        public void Parse_1D6Plus10() {
            var dice = Dice.Parse("1D6+10");
            Assert.AreEqual(1, dice.Count);
            Assert.AreEqual(1, dice[0].Rolls);
            Assert.AreEqual(6, dice[0].Sides);
            Assert.AreEqual(10, dice[0].Plus);
        }
        [Test]
        public void Parse_1D61D4() {
            var dice = Dice.Parse("1D6 1D4");
            Assert.AreEqual(2, dice.Count);
            Assert.AreEqual(1, dice[0].Rolls);
            Assert.AreEqual(6, dice[0].Sides);
            Assert.AreEqual(0, dice[0].Plus);
            Assert.AreEqual(1, dice[1].Rolls);
            Assert.AreEqual(4, dice[1].Sides);
            Assert.AreEqual(0, dice[1].Plus);
        }

    }
}