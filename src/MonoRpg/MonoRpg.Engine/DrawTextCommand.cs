namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    internal class DrawTextCommand : IDrawCommand {
        private readonly SpriteFont _font;
        private readonly string _text;
        private readonly Vector2 _position;
        private readonly Color _color;
        public bool PixelArt { get; set; }
        public float Scale { get; set; }

        public DrawTextCommand(string text, Vector2 position, SpriteFont font, Color color, float scale) {
            _text = text;
            _position = position;
            _color = color;
            _font = font;
            Scale = scale;
        }

        

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(_font, _text, _position, _color, 0, Vector2.Zero, new Vector2(Scale,Scale), SpriteEffects.None, 0 );
        }

        
    }
}