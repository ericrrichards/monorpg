using System.Collections.Generic;
namespace MonoRpg.Engine.Tiled {

    

    public class TiledMap {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public List<TileLayer> Layers { get; set; }
        public List<TileSet> TileSets { get; set; }

    }

    public class TileLayer {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<int> Data { get; set; }
        public float Opacity { get; set; }
    }

    public class TileSet {
        public int Columns { get; set; }
        public int Rows { get; set; }
        public string Image { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string Name { get; set; }
        public int Margin { get; set; }
        public int Spacing { get; set; }
        public int TileCount { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
    }
}
