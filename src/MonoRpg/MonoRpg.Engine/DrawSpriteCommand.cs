namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    internal class DrawSpriteCommand : IDrawCommand {
        private readonly Texture2D _texture;
        private readonly Vector2 _position;

        public DrawSpriteCommand(Sprite sprite, Vector2 position) {
            _texture = sprite.Texture;
            _position = position + new Vector2(-sprite.Width/2f, -sprite.Height/2f);
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(_texture, _position, Color.White);
        }
    }
}