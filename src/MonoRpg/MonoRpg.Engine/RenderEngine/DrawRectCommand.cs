namespace MonoRpg.Engine.RenderEngine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class DrawRectCommand : IDrawCommand {
        private Rectangle Rectangle { get; }
        private Color Color { get; }
        public bool PixelArt { get; set; }

        public DrawRectCommand(Rectangle rectangle, Color color) {
            Rectangle = rectangle;
            Color = color;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(System.Content.DummyTexture, Rectangle, Color);
        }
    }
}