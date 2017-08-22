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
        
    }
}
