using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine {
    using global::System.Diagnostics;

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
        public int BlockingTile { get; set; }

        public int LayerCount {
            get {
                Debug.Assert(MapDef.Layers.Count %3 == 0);
                return MapDef.Layers.Count / 3;
            }
        }

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

            UVs = TextureAtlas.GenerateUVs(TileWidth, TileHeight);

            foreach (var tileSet in mapDef.TileSets) {
                if (tileSet.Name == "collision_graphic") {
                    BlockingTile = tileSet.FirstGid-1;
                    break;
                }
            }
            Debug.Assert(BlockingTile > 0);

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

        public bool IsBlocked(int layer, int tileX, int tileY) {
            try {
                var tile = GetTile(tileX, tileY, layer + 2);
                return tile == BlockingTile;
            } catch (IndexOutOfRangeException) {
                return true;
            }
        }

        private int GetTile(int x, int y, int layer=0) {
            var tiles = MapDef.Layers[layer].Data;
            var index = x + y * Width;
            if (index < 0 || index >= tiles.Count)
                throw new IndexOutOfRangeException();
            else
                return tiles[index] - 1; // Tiled uses 1 as the first ID, instead of 0 like everything else in the world does.
        }

        public void Goto(int x, int y) {
            CamX = x - System.ScreenWidth / 2;
            CamY = -y + System.ScreenHeight / 2;
        }

        public void GotoTile(int x, int y) {
            Goto(x * TileWidth + TileWidth / 2, y * TileHeight + TileHeight / 2);
        }

        public Point GetTileFoot(int x, int y) {
            return new Point(X + x * TileWidth, Y - y * TileHeight - TileHeight / 2);
        }



        public void Render(Renderer renderer) {
            RenderLayer(renderer, 0);
        }

        public void RenderLayer(Renderer renderer, int layer) {
            var layerIndex = layer * 3;

            var (left, bottom) = PointToTile(CamX - System.ScreenWidth / 2, CamY - System.ScreenHeight / 2);
            var (right, top) = PointToTile(CamX + System.ScreenWidth / 2, CamY + System.ScreenHeight / 2);


            for (var j = top; j <= bottom; j++) {
                for (var i = left; i <= right; i++) {
                    var tile = GetTile(i, j, layerIndex);
                    Rectangle uvs;
                    Sprite.Position = new Vector2(X + i * TileWidth, Y - j * TileHeight);
                    if (tile >= 0) {
                        uvs = UVs[tile];
                        Sprite.SetUVs(uvs);
                        renderer.DrawSprite(Sprite);
                    }
                    tile = GetTile(i, j, layerIndex+1);
                    if (tile >= 0) {
                        uvs = UVs[tile];
                        Sprite.SetUVs(uvs);
                        renderer.DrawSprite(Sprite);
                    }
                }
            }
        }
    }
}
