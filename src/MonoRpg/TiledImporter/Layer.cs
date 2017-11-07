namespace TiledImporter {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml.Linq;

    using Ardalis.GuardClauses;

    internal class Layer {
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        internal Data Data { get; }

        public Layer(XElement layerElement) {
            Guard.Against.Null(layerElement, nameof(layerElement));
            Guard.Against.WrongXmlElement(layerElement, nameof(layerElement), "layer");
            Name = layerElement.Attribute("name")?.Value;
            Width = layerElement.ParseAttribute("width");
            Height = layerElement.ParseAttribute("height");
            Data = new Data(layerElement.Element("data"));

        }

        internal int GetTile(int x, int y) {
            return Data.Tiles[GetIndex(x, y)];
        }

        public void SetTile(int x, int y, int gid) {
            Data.SetTile(GetIndex(x,y), gid);
        }

        private int GetIndex(int x, int y) {
            Guard.Against.OutOfRange(x, nameof(x), 0, Width-1);
            Guard.Against.OutOfRange(y, nameof(y), 0, Height-1);
            return y * Width + x;
        }
    }

    internal class Data {
        public Encoding Encoding { get; }
        public Compression Compression { get; }
        private readonly List<int> _tiles;
        public IReadOnlyList<int> Tiles => new ReadOnlyCollection<int>(_tiles);
        public Data(XElement element) {
            Guard.Against.Null(element, nameof(element));
            Guard.Against.WrongXmlElement(element, nameof(element), "data");
            Encoding = element.ParseAttribute<Encoding>("encoding");
            Compression = element.ParseAttribute<Compression>("compression");
            if (Encoding != Encoding.XML) {
                _tiles = ParseTiles(element.Value);
            } else {
                _tiles = element.Elements("tile").Select(e => new Tile(e)).Select(t => t.GID).ToList();
            }

        }

        internal void SetTile(int index, int gid) {
            Guard.Against.OutOfRange(index, nameof(index), 0, _tiles.Count-1);
            _tiles[index] = gid;
        }
        
        private List<int> ParseTiles(string elementValue) {
            Guard.Against.NullOrEmpty(elementValue, nameof(elementValue));
            List<int> tiles;

            if (Encoding == Encoding.XML) {
                throw new InvalidOperationException("Encoding is XML");
            }
            if (Encoding == Encoding.CSV) {
                var values = elementValue.Replace("\r", string.Empty).Replace("\n", string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                tiles = values.Select(v => Convert.ToInt32(v)).ToList();
            } else {
                var base64 = Convert.FromBase64String(elementValue);
                switch (Compression) {
                    case Compression.None:
                        tiles = ConvertBytesToTileIds(base64);
                        break;
                    case Compression.GZip:
                        tiles = DecompressTileData(base64, ms => new GZipStream(ms, CompressionMode.Decompress));
                        break;
                    case Compression.ZLib:
                        tiles = DecompressTileData(base64,
                                                   ms => {
                                                       // skip the first two bytes as in: http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html
                                                       ms.ReadByte();
                                                       ms.ReadByte();
                                                       var s = new DeflateStream(ms, CompressionMode.Decompress);
                                                       return s;
                                                   });
                        break;
                    default:
                        throw new InvalidOperationException("Invalid compression type: " + Compression);
                }
            }

            return tiles;
        }

        private static List<int> DecompressTileData(byte[] base64, Func<MemoryStream, Stream> decompress) {
            using (var ms = new MemoryStream(base64))
            using (var gz = decompress(ms))
            using (var decompressed = new MemoryStream()) {
                gz.CopyTo(decompressed);
                var data = decompressed.ToArray();
                return ConvertBytesToTileIds(data);
            }
        }

        private static List<int> ConvertBytesToTileIds(byte[] base64) {
            var tiles = new List<int>();
            for (var i = 0; i < base64.Length; i += 4) {
                tiles.Add(BitConverter.ToInt32(base64, i));
            }
            return tiles;
        }
        

        private class Tile {
            public int GID { get; }

            public Tile(XElement element) {
                Guard.Against.Null(element, nameof(element));
                Guard.Against.WrongXmlElement(element, nameof(element), "tile");
                GID = element.ParseAttribute("gid");
            }
        }
    }

    
}