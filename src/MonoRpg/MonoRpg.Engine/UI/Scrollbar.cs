namespace MonoRpg.Engine.UI {
    using global::System;
    using global::System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using MonoRpg.Engine.RenderEngine;

    public class Scrollbar {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public Texture2D Texture { get; private set; }
        public float Value { get; set; }
        public Sprite UpSprite { get; set; }
        public Sprite DownSprite { get; set; }
        public Sprite BackgroundSprite { get; set; }
        public Sprite CaretSprite { get; set; }
        public float CaretSize { get; set; }
        public int TileHeight { get; set; }
        public List<Rectangle> UVs { get; set; }
        public int LineHeight { get; set; }
        public float Start { get; set; }

        public Scrollbar(Texture2D texture, int height) {
            X = 0;
            Y = 0;
            Height = height;
            Texture = texture;
            Value = 0;
            UpSprite = new Sprite { Texture = texture };
            DownSprite = new Sprite { Texture = texture };
            BackgroundSprite = new Sprite { Texture = texture };
            CaretSprite = new Sprite { Texture = texture };
            CaretSize = 1;


            var texWidth = texture.Width;
            var texHeight = texture.Height;
            TileHeight = texHeight / 4;

            UVs = texture.GenerateUVs(texWidth, TileHeight);
            UpSprite.SetUVs(UVs[0]);
            CaretSprite.SetUVs(UVs[1]);
            BackgroundSprite.SetUVs(UVs[2]);
            DownSprite.SetUVs(UVs[3]);

            LineHeight = Height - TileHeight * 2;
            SetPosition(0, 0);
        }

        public void SetPosition(int x, int y) {
            X = x;
            Y = y;

            var top = y + Height / 2;
            var bottom = y - Height / 2;
            var halfTileHeight = TileHeight / 2;

            UpSprite.Position = new Vector2(x, top-halfTileHeight);
            DownSprite.Position = new Vector2(x, bottom + halfTileHeight);

            BackgroundSprite.Scale= new Vector2(1, LineHeight/ (float)TileHeight);
            BackgroundSprite.Position = new Vector2(X, top-TileHeight*3/2);
            SetNormalValue(Value);
        }

        public void SetNormalValue(float value) {
            Value = value;
            CaretSprite.Scale = new Vector2(1, CaretSize);
            var caretHeight = TileHeight * CaretSize;

            Start = Y + LineHeight / 2 - TileHeight/2;
            Start -= (LineHeight - caretHeight) * value;
            CaretSprite.Position = new Vector2(X, Start);
        }

        public void Render(Renderer renderer) {
            renderer.DrawSprite(UpSprite);
            renderer.DrawSprite(BackgroundSprite);
            renderer.DrawSprite(DownSprite);
            renderer.DrawSprite(CaretSprite);
        }

        public void SetScrollCaretScale(float normalValue) {
            CaretSize = LineHeight * normalValue / TileHeight;
            CaretSize = MathHelper.Max(1, CaretSize);
        }

        
    }
}