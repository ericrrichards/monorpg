﻿namespace MonoRpg.Engine.RenderEngine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Text;

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

        private Renderer(GraphicsDevice device, Content content) {
            _content = content;
            _device = device;
            _spriteBatch = new SpriteBatch(_device);
            ClearColor = Color.Black;
            VerticalAlignment = TextAlignment.Top;
            HorizontalAlignment = TextAlignment.Left;
        }

        public static Renderer Create(GraphicsDevice device, Content content) {
            System.Renderer = new Renderer(device, content);
            return System.Renderer;
        }
        public void DrawText2D(int x, int y, string text, Color color, float scale, int wrap, string fontName = null) {
            var font = _content.DefaultFont;
            if (fontName != null) {
                font = _content.LoadFont(fontName);
            }
            var position = AlignText(TranslateCoords(new Vector2(x, y)), font, text, scale);
            var command = new DrawTextCommand(text, position, font, color, scale, wrap);
            _drawQueue.Enqueue(command);
        }

        public void DrawText2D(int x, int y, string text, Color color, float scale) {
            DrawText2D(x,y,text, color, scale, -1);
        }

        public void DrawText2D(int x, int y, string text, Color color) {
            DrawText2D(x,y,text, color, 1.0f);
        }

        public void DrawText2D(int x, int y, string text) {
            DrawText2D(x, y, text, Color.White);
        }

        public void DrawSprite(Sprite sprite) {
            _drawQueue.Enqueue(new DrawSpriteCommand(sprite, TranslateCoords(sprite.Position), sprite.Scale));
        }

        public void DrawRect2D(int left, int top, int right, int bottom, Color color) {
            var pos = TranslateCoords(new Vector2(left, top));
            _drawQueue.Enqueue(new DrawRectCommand(new Rectangle(pos.ToPoint(), new Point(right - left, top - bottom)), color));
        }
        public void DrawRect2D(Rectangle bounds, Color color) {
            DrawRect2D(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, color);
        }

        private Vector2 AlignText(Vector2 p, SpriteFont font, string text, float scale) {
            var size = font.MeasureString(text)*scale;
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

        public void AlignText(TextAlignment horizontal, TextAlignment vertical) {
            if (vertical == TextAlignment.Left || vertical == TextAlignment.Right) {
                throw new ArgumentOutOfRangeException(nameof(vertical), vertical, "Top, Center or Bottom are the only valid values.");
            }
            if (horizontal == TextAlignment.Top || horizontal == TextAlignment.Bottom) {
                throw new ArgumentOutOfRangeException(nameof(horizontal), horizontal, "Left, Center or Right are the only valid values.");
            }
            HorizontalAlignment = horizontal;
            VerticalAlignment = vertical;
        }

        private SamplerState GetMode(IDrawCommand command) { return command.PixelArt ? SamplerState.PointClamp : SamplerState.LinearClamp; }

        public void Render() {
            _device.SetRenderTarget(null);
            _device.Clear(ClearColor);
            
            var pixelArt = new Queue<IDrawCommand>(_drawQueue.Where(dq => dq.PixelArt));
            var blended = new Queue<IDrawCommand>(_drawQueue.Where(dq => !dq.PixelArt));

            while (_drawQueue.Any()) {
                var mode = _drawQueue.Peek().PixelArt ? SamplerState.PointClamp : SamplerState.LinearClamp;
                _spriteBatch.Begin(samplerState: mode);
                do {
                    var command = _drawQueue.Dequeue();
                    command.Draw(_spriteBatch);
                } while (_drawQueue.Any() && GetMode(_drawQueue.Peek()) == mode);
                _spriteBatch.End();
            }

            _drawQueue.Clear();
        }

        

        public Vector2 MeasureText(string text, int wrap, float scale = 1.0f) {
            if (wrap > 0) {
                text = WrapText(_content.DefaultFont, text, wrap, scale);
            }
            return _content.DefaultFont.MeasureString(text)*scale;
        }

        public static string WrapText(SpriteFont spriteFont, string text, float maxLineWidth, float scale) {
            var words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            var lineWidth = 0f;
            var spaceWidth = spriteFont.MeasureString(" ").X * scale;

            foreach (var word in words) {
                var size = spriteFont.MeasureString(word)*scale;

                if (lineWidth + size.X+spaceWidth < maxLineWidth) {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                } else {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }

        public List<string> ChunkText(string text, float maxLineWidth, float scale, SpriteFont font = null) {
            //maxLineWidth = maxLineWidth * 0.9f;
            if (font == null) {
                font = _content.DefaultFont;
            }
            var words = text.Split(' ');
            var chunks = new List<string>();
            var lineWidth = 0f;
            var spaceWidth = font.MeasureString(" ").X * scale;
            var sb = new StringBuilder();
            foreach (var word in words) {
                var size = font.MeasureString(word) * scale;

                if (lineWidth + size.X < maxLineWidth) {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                } else {
                    chunks.Add(sb.ToString());

                    sb = new StringBuilder(word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString())) {
                chunks.Add(sb.ToString());
            }

            return chunks;
        }

        public float TextHeight(string text, int wrap, float textScale) {
            return (int)Math.Ceiling(MeasureText(text, wrap, textScale).Y) + _content.DefaultFont.LineSpacing/2;
        }
    }
}
