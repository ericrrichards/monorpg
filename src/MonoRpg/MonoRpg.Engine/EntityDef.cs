namespace MonoRpg.Engine {
    public struct EntityDef {
        public string Texture { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int StartFrame { get; set; }
        public int Layer { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}