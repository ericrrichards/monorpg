using System.Collections.Generic;
namespace MonoRpg.Engine.Tiled {
    using global::System.IO;
    using global::System.Linq;

    public class TiledMap {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public List<TileLayer> Layers { get; set; }
        public List<TileSet> TileSets { get; set; }
        public List<MapAction> OnWake { get; set; }

        public TiledMap() {
            OnWake = new List<MapAction>();
        }
    }

    public struct MapAction {
        public string ID { get; set; }
        public MapActionParameters Params { get; set; }
    }

    public class MapActionParameters {
        
    }

    public class AddNPCParams : MapActionParameters {
        public string Character { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Layer { get; set; }
    }
}
