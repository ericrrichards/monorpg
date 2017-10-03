namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    internal struct DrawSpriteCommand : IDrawCommand {
        private readonly Texture2D _texture;
        private readonly Vector2 _position;
        private readonly Rectangle? _sourceRect;
        private readonly Vector2 _scale;
        private readonly Color _color;
        public bool PixelArt { get; set; }

        public DrawSpriteCommand(Sprite sprite, Vector2 position) : this(sprite, position, Vector2.One) {
            
        }

        public DrawSpriteCommand(Sprite sprite, Vector2 position, Vector2 scale) {
            _texture = sprite.Texture;
            var width = sprite.SourceRectangle != null ? sprite.SourceRectangle.Value.Width : sprite.Width;
            var height = sprite.SourceRectangle != null ? sprite.SourceRectangle.Value.Height : sprite.Height;
            _position = position + new Vector2(-width / 2f, -height / 2f);
            _sourceRect = sprite.SourceRectangle;
            _scale = scale;
            _color = sprite.Color;
            PixelArt = sprite.PixelArt;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(_texture, _position, _sourceRect, _color, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }

        
    }
}