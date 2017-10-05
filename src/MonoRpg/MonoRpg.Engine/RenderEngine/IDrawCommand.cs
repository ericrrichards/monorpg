namespace MonoRpg.Engine.RenderEngine {
    using Microsoft.Xna.Framework.Graphics;

    internal interface IDrawCommand {
        void Draw(SpriteBatch spriteBatch);
        bool PixelArt { get; set; }
    }
}