namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework.Graphics;

    internal interface IDrawCommand {
        void Draw(SpriteBatch spriteBatch);
    }
}