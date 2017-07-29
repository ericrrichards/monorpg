namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Sprite {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }

        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public Sprite() {
            
        }

    }
}