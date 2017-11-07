using System;

namespace TiledImporter
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml.Linq;

    using Ardalis.GuardClauses;

    public class Map {
        public string Version { get; }
        public string TiledVersion { get;  }
        public Orientation Orientation { get; }
        public RenderOrder RenderOrder { get; }
        public int Width { get; }
        public int Height { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public IReadOnlyList<TileSet> TileSets { get; }
        private IReadOnlyList<Layer> Layers { get; }

        public Map(XElement mapElement) {
            Guard.Against.Null(mapElement, nameof(mapElement));
            Guard.Against.WrongXmlElement(mapElement, nameof(mapElement), "map");

            Version = mapElement.Attribute("version")?.Value;
            TiledVersion = mapElement.Attribute("tiledversion")?.Value;
            Orientation = mapElement.ParseAttribute<Orientation>("orientation");
            RenderOrder = mapElement.ParseAttribute<RenderOrder>("renderorder");
            Width = mapElement.ParseAttribute("width");
            Height = mapElement.ParseAttribute("height");
            TileWidth = mapElement.ParseAttribute("tilewidth");
            TileHeight = mapElement.ParseAttribute("tileheight");
            TileSets = new ReadOnlyCollection<TileSet>(mapElement.Elements("tileset").Select(e => new TileSet(e)).ToList());
            Layers = new ReadOnlyCollection<Layer>(mapElement.Elements("layer").Select(e=>new Layer(e)).ToList());
        }

        public int GetTile(int x, int y, int layer) {
            Guard.Against.OutOfRange(x, nameof(x), 0, Width-1);
            Guard.Against.OutOfRange(y, nameof(y), 0, Height-1);
            Guard.Against.OutOfRange(layer, nameof(layer), 0, Layers.Count-1);
            return Layers[layer].GetTile(x, y);
        }
    }

    
}

