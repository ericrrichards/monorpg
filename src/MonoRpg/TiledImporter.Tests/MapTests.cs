using NUnit.Framework;
using System;

namespace TiledImporter.Tests {
    using System.Xml.Linq;

    [TestFixture]
    public class MapTests {
        [Test]
        public void Map_NullElement_ThrowsNullArgumentException() {
            Assert.Throws<ArgumentNullException>(() => new Map(null));
        }

        [Test]
        public void Map_BadElement_ThrowsArgumentOutOfRange() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Map(new XElement("foo")));

        }

        private readonly XElement _xml = XElement.Parse("<map version=\"1.0\" tiledversion=\"1.0.3\" orientation=\"orthogonal\" renderorder=\"right-down\" " + 
            "width=\"64\" height=\"64\" tilewidth=\"16\" tileheight=\"16\" nextobjectid=\"1\"/>");
        [Test]
        public void Map_Xml() {
            var map = new Map(_xml);
            Assert.AreEqual("1.0", map.Version);
            Assert.AreEqual("1.0.3", map.TiledVersion);
            Assert.AreEqual(Orientation.Orthagonal, map.Orientation);
            Assert.AreEqual(RenderOrder.RightDown, map.RenderOrder);
            Assert.AreEqual(64, map.Width);
            Assert.AreEqual(64, map.Height);
            Assert.AreEqual(16, map.TileWidth);
            Assert.AreEqual(16, map.TileHeight);
        }
        private readonly XElement _xml2 = XElement.Parse("<map version=\"1.0\" tiledversion=\"1.0.3\" " +
                                                        "width=\"64\" height=\"64\" tilewidth=\"16\" tileheight=\"16\" nextobjectid=\"1\"/>");
        [Test]
        public void Map_Xml_DefaultEnumValues() {
            var map = new Map(_xml2);
            Assert.AreEqual("1.0", map.Version);
            Assert.AreEqual("1.0.3", map.TiledVersion);
            Assert.AreEqual(Orientation.Orthagonal, map.Orientation);
            Assert.AreEqual(RenderOrder.RightDown, map.RenderOrder);
            Assert.AreEqual(64, map.Width);
            Assert.AreEqual(64, map.Height);
            Assert.AreEqual(16, map.TileWidth);
            Assert.AreEqual(16, map.TileHeight);
        }

        [Test]
        public void LoadCSV() {
            var xml = XDocument.Load(GetType().Assembly.GetManifestResourceStream("TiledImporter.Tests.arena.tmx"));
            var map = new Map(xml.Element("map"));
            Assert.AreEqual(586, map.GetTile(0,0, 0));
        }
        [Test]
        public void LoadXML() {
            var xml = XDocument.Load(GetType().Assembly.GetManifestResourceStream("TiledImporter.Tests.arena2.tmx"));
            var map = new Map(xml.Element("map"));
            Assert.AreEqual(586, map.GetTile(0, 0, 0));
        }
        [Test]
        public void LoadBase64() {
            var xml = XDocument.Load(GetType().Assembly.GetManifestResourceStream("TiledImporter.Tests.arena3.tmx"));
            var map = new Map(xml.Element("map"));
            Assert.AreEqual(586, map.GetTile(0, 0, 0));
        }
        [Test]
        public void LoadGZip() {
            var xml = XDocument.Load(GetType().Assembly.GetManifestResourceStream("TiledImporter.Tests.arena4.tmx"));
            var map = new Map(xml.Element("map"));
            Assert.AreEqual(586, map.GetTile(0, 0, 0));
        }
        [Test]
        public void LoadZLib() {
            var xml = XDocument.Load(GetType().Assembly.GetManifestResourceStream("TiledImporter.Tests.arena5.tmx"));
            var map = new Map(xml.Element("map"));
            Assert.AreEqual(586, map.GetTile(0, 0, 0));
        }
    }
    
}
