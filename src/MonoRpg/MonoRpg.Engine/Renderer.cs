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
        public TextAlignment VerticAlignment { get; private set; }
        public TextAlignment HorizontalAlignment { get; private set; }

        private readonly GraphicsDevice _device;
        private readonly SpriteBatch _spriteBatch;
        private readonly Queue<IDrawCommand> _drawQueue = new Queue<IDrawCommand>();
        private readonly Content _content;

        public Renderer(GraphicsDevice device, Content content) {
            _content = content;
            _device = device;
            _spriteBatch = new SpriteBatch(_device);
            ClearColor = Color.Black;
            VerticAlignment = TextAlignment.Top;
            HorizontalAlignment = TextAlignment.Left;
        }

        public void DrawText2D(int x, int y, string text) {
            var position = AlignText(TranslateCoords(new Vector2(x,y)), _content.DefaultFont, text);
            var command = new DrawTextCommand(text, position, _content.DefaultFont );
            _drawQueue.Enqueue(command);
        }

        public void DrawSprite(Sprite sprite) {
            _drawQueue.Enqueue(new DrawSpriteCommand(sprite, TranslateCoords(sprite.Position)));
        }

        private static Vector2 AlignText(Vector2 p, SpriteFont font, string text) {
            var size = font.MeasureString(text);
            return p - size / 2;
        }

        private Vector2 TranslateCoords(Vector2 p) {
            var x = p.X + _device.Viewport.Width / 2f;
            var y = -p.Y + _device.Viewport.Height / 2f;
            return new Vector2(x,y);
        }
        public void SetTextAlignment(TextAlignment horizontal, TextAlignment vertical) {
            if (vertical == TextAlignment.Left || vertical == TextAlignment.Right) {
                throw new ArgumentOutOfRangeException(nameof(vertical), vertical, "Top, Center or Bottom are the only valid values.");
            }
            if (horizontal == TextAlignment.Top || horizontal == TextAlignment.Bottom) {
                throw new ArgumentOutOfRangeException(nameof(horizontal), horizontal, "Left, Center or Right are the only valid values.");
            }
            HorizontalAlignment = horizontal;
            VerticAlignment = vertical;
        }

        public void Render() {
            _device.Clear(ClearColor);

            _spriteBatch.Begin();

            while (_drawQueue.Any()) {
                var command = _drawQueue.Dequeue();
                command.Draw(_spriteBatch);
            }


            _spriteBatch.End();
            
        }

        
    }
}
