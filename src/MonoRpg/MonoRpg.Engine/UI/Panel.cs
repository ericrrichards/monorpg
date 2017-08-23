using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine.UI {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Panel {
        public List<Sprite> Tiles { get; set; }
        public int TileSize { get; set; }
        public List<Rectangle> UVs { get; set; }
        public Texture2D Texture { get; private set; }

        public Panel(PanelParams parameters) {
            Texture = parameters.Texture;
            UVs = Texture.GenerateUVs(parameters.Size, parameters.Size);
            TileSize = parameters.Size;
            Tiles = new List<Sprite>();
            foreach (Rectangle r in UVs) {
                var sprite = new Sprite { Texture = Texture };
                sprite.SetUVs(r);
                Tiles.Add(sprite);
            }

        }

        public void Position(int left, int top, int right, int bottom) {
            foreach (var sprite in Tiles) {
                sprite.Scale = Vector2.One;
            }
            var hSize = TileSize / 2f;

            Tiles[0].Position = new Vector2(left+hSize, top-hSize);
            Tiles[2].Position = new Vector2(right-hSize, top-hSize);
            Tiles[6].Position = new Vector2(left+hSize, bottom+hSize);
            Tiles[8].Position = new Vector2(right-hSize, bottom+hSize);

            var widthScale = (Math.Abs(right - left)-TileSize) / TileSize;
            var centerX = (right + left) / 2;

            Tiles[1].Position = new Vector2(left+hSize*3, top-hSize);
            Tiles[1].Scale = new Vector2(widthScale, 1);

            Tiles[7].Position = new Vector2(left+hSize*3, bottom+hSize);
            Tiles[7].Scale = new Vector2(widthScale, 1);

            var heightScale = (Math.Abs(bottom - top)-TileSize) / TileSize;
            var centerY = (top + bottom) / 2;

            Tiles[3].Scale = new Vector2(1, heightScale);
            Tiles[3].Position = new Vector2(left+hSize, top-hSize*3);

            Tiles[5].Scale = new Vector2(1, heightScale);
            Tiles[5].Position = new Vector2(right-hSize, top-hSize*3);

            Tiles[4].Scale = new Vector2(widthScale, heightScale);
            Tiles[4].Position = new Vector2(left+hSize*3, top-hSize*3);

            if (left - right == 0 || top - bottom == 0) {
                foreach (var sprite in Tiles) {
                    sprite.Scale = Vector2.Zero;
                }
            }

        }

        public void Render(Renderer renderer) {
            foreach (var sprite in Tiles) {
                renderer.DrawSprite(sprite);
            }
        }
    }

    public struct PanelParams {
        public Texture2D Texture { get; set; }
        public int Size { get; set; }
        

    }
}
