using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using MonoRpg.Engine.Tiled;

    public class Map {
        public int X { get; set; }
        public int Y { get; set; }

        public int CamX { get; set; }
        public int CamY { get; set; }

        public TiledMap MapDef { get; set; }
        public Texture2D TextureAtlas { get; set; }
        public Sprite Sprite { get; set; }
        public TileLayer Layer { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public List<int> Tiles { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public int WidthInPixels { get; set; }
        public int HeightInPixels { get; set; }
        public List<Rectangle> UVs { get; set; }

        public Map(TiledMap mapDef) {
            MapDef = mapDef;
            TextureAtlas = System.Content.FindTexture(mapDef.TileSets[0].Image);
            Sprite = new Sprite();
            Layer = mapDef.Layers[0];
            Width = Layer.Width;
            Height = Layer.Height;
            Tiles = Layer.Data;
            TileWidth = mapDef.TileSets[0].TileWidth;
            TileHeight = mapDef.TileSets[0].TileHeight;

            Sprite.Texture = TextureAtlas;

            X = -System.ScreenWidth / 2 + TileWidth / 2;
            Y = System.ScreenHeight / 2 - TileHeight / 2;

            WidthInPixels = Width * TileWidth;
            HeightInPixels = Height * TileHeight;

            UVs = TextureAtlas.GenerateUVs( TileWidth, TileHeight );
        }

        private (int x, int y) PointToTile(int x, int y) {
            x += TileWidth / 2;
            y -= TileHeight / 2;

            x = Math.Max(X, x);
            y = Math.Min(Y, y);
            x = Math.Min(X + WidthInPixels - 1, x);
            y = Math.Max(Y - HeightInPixels + 1, y);

            var tileX = (int)Math.Floor((double)(x - X) / TileWidth);
            var tileY = (int)Math.Floor((double)(Y - y) / TileHeight);

            return (tileX, tileY);
        }
        private int GetTile( int x, int y) {
            return Tiles[x + y * Width] - 1; // Tiled uses 1 as the first ID, instead of 0 like everything else in the world does.
        }

        public void Goto(int x, int y) {
            CamX = x - System.ScreenWidth / 2;
            CamY = -y  + System.ScreenHeight / 2;
        }

        public void GotoTile(int x, int y) {
            Goto(x * TileWidth + TileWidth/2, y*TileHeight + TileHeight/2);
        }



        public void Render(Renderer renderer) {
            var (left, bottom) = PointToTile(CamX - System.ScreenWidth / 2, CamY - System.ScreenHeight / 2);
            var (right, top )= PointToTile(CamX + System.ScreenWidth / 2, CamY + System.ScreenHeight / 2);

            for (var j = top; j <= bottom; j++) {
                for (var i = left; i <= right; i++) {
                    var tile = GetTile(i, j);
                    var uvs = UVs[tile];
                    Sprite.SetUVs(uvs);
                    Sprite.Position = new Vector2(X + i* TileWidth, Y - j * TileHeight);
                    renderer.DrawSprite(Sprite);
                }
            }
        }
    }
}
