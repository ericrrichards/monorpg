namespace MonoRpg.Engine.Tiled {
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
        public int FirstGid { get; set; }
        public string Source { get; set; }
    }
}