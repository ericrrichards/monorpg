namespace MonoRpg.Engine {
    using global::System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    internal class DrawTextCommand : IDrawCommand {
        private readonly SpriteFont _font;
        private readonly string _text;
        private readonly Vector2 _position;
        private readonly Color _color;
        public bool PixelArt { get; set; }
        public float Scale { get; set; }
        public int Wrap { get; set; }

        public DrawTextCommand(string text, Vector2 position, SpriteFont font, Color color, float scale, int wrap) {
            _text = text;
            _position = position;
            _color = color;
            _font = font;
            Scale = scale;
            Wrap = wrap;
        }

        

        public void Draw(SpriteBatch spriteBatch) {
            if (Wrap == -1) {
                spriteBatch.DrawString(_font, _text, _position, _color, 0, Vector2.Zero, new Vector2(Scale, Scale), SpriteEffects.None, 0);
            } else {
                var text = WrapText(_font, _text, Wrap, Scale);
                spriteBatch.DrawString(_font, text, _position, _color, 0, Vector2.Zero, new Vector2(Scale, Scale), SpriteEffects.None, 0);
            }
        }

        private string WrapText(SpriteFont spriteFont, string text, float maxLineWidth, float scale) {
            var words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            var lineWidth = 0f;
            var spaceWidth = spriteFont.MeasureString(" ").X * scale;

            foreach (var word in words) {
                var size = spriteFont.MeasureString(word)*scale;

                if (lineWidth + size.X < maxLineWidth) {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                } else {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}