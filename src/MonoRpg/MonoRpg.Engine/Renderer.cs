using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Renderer {
        public Color ClearColor { get; set; }
        public TextAlignment VerticalAlignment { get; private set; }
        public TextAlignment HorizontalAlignment { get; private set; }

        private readonly GraphicsDevice _device;
        private readonly SpriteBatch _spriteBatch;
        private readonly Queue<IDrawCommand> _drawQueue = new Queue<IDrawCommand>();
        private readonly Content _content;

        private Vector2 Translation { get; set; }

        public void Translate(int x, int y) {
            Translation = new Vector2(x, -y);
        }

        public Renderer(GraphicsDevice device, Content content) {
            _content = content;
            _device = device;
            _spriteBatch = new SpriteBatch(_device);
            ClearColor = Color.Black;
            VerticalAlignment = TextAlignment.Top;
            HorizontalAlignment = TextAlignment.Left;
        }

        public void DrawText2D(int x, int y, string text, Color color) {
            var position = AlignText(TranslateCoords(new Vector2(x, y)), _content.DefaultFont, text);
            var command = new DrawTextCommand(text, position, _content.DefaultFont, color);
            _drawQueue.Enqueue(command);
        }

        public void DrawText2D(int x, int y, string text) {
            DrawText2D(x, y, text, Color.White);
        }

        public void DrawSprite(Sprite sprite) {
            _drawQueue.Enqueue(new DrawSpriteCommand(sprite, TranslateCoords(sprite.Position), sprite.Scale));
        }

        private Vector2 AlignText(Vector2 p, SpriteFont font, string text) {
            var size = font.MeasureString(text);
            switch (HorizontalAlignment) {
                case TextAlignment.Left:
                    break;
                case TextAlignment.Right:
                    p.X -= size.X;
                    break;
                case TextAlignment.Center:
                    p.X -= size.X / 2;
                    break;
            }
            switch (VerticalAlignment) {
                case TextAlignment.Top:
                    break;
                case TextAlignment.Center:
                    p.Y -= size.Y / 2;
                    break;
                case TextAlignment.Bottom:
                    p.Y -= size.Y;
                    break;
            }
            return p;

        }

        private Vector2 TranslateCoords(Vector2 p) {
            var x = p.X + _device.Viewport.Width / 2f;
            var y = -p.Y + _device.Viewport.Height / 2f;
            return new Vector2(x, y) + Translation;
        }
        public void SetTextAlignment(TextAlignment horizontal, TextAlignment vertical) {
            if (vertical == TextAlignment.Left || vertical == TextAlignment.Right) {
                throw new ArgumentOutOfRangeException(nameof(vertical), vertical, "Top, Center or Bottom are the only valid values.");
            }
            if (horizontal == TextAlignment.Top || horizontal == TextAlignment.Bottom) {
                throw new ArgumentOutOfRangeException(nameof(horizontal), horizontal, "Left, Center or Right are the only valid values.");
            }
            HorizontalAlignment = horizontal;
            VerticalAlignment = vertical;
        }

        public void Render() {
            _device.Clear(ClearColor);
            var pixelArt = new Queue<IDrawCommand>(_drawQueue.Where(dq => dq.PixelArt));
            var blended = new Queue<IDrawCommand>(_drawQueue.Where(dq => !dq.PixelArt));

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            while (pixelArt.Any()) {
                var command = pixelArt.Dequeue();
                command.Draw(_spriteBatch);
            }


            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);

            while (blended.Any()) {
                var command = blended.Dequeue();
                command.Draw(_spriteBatch);
            }


            _spriteBatch.End();

        }

        public void ScaleText(float scale) { }
    }
}
