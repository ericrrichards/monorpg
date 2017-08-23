namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Sprite {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }

        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public Rectangle? SourceRectangle { get; private set; }
        public Vector2 Scale { get; set; }
        public bool PixelArt { get; set; }

        public Sprite() {
            Scale = Vector2.One;
            
        }

        public void SetUVs(Rectangle sourceRect) { SourceRectangle = sourceRect; }
    }
}