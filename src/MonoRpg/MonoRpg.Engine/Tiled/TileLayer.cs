namespace MonoRpg.Engine.Tiled {
    using global::System.Collections.Generic;

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
}