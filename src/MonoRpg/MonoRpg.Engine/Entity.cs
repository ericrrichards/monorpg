namespace MonoRpg.Engine {
    using global::System.Collections.Generic;
    using global::System.Diagnostics;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Entity {
        public int StartFrame { get; set; }

        public int TileY { get; set; }

        public int TileX { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Sprite Sprite { get; }
        public Texture2D Texture { get; }
        public List<Rectangle> UVs { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Layer { get; set; }

        public Entity(EntityDef def) {
            Sprite = new Sprite();
            Texture = System.Content.FindTexture(def.Texture);
            Height = def.Height;
            Width = def.Width;
            TileX = def.TileX;
            TileY = def.TileY;
            StartFrame = def.StartFrame;
            Layer = def.Layer;

            Sprite.Texture = Texture;
            UVs = Texture.GenerateUVs(Width, Height);
            SetFrame(StartFrame);
        }

        public void SetFrame(int frame) {
            Sprite.SetUVs(UVs[frame]);
        }

        public void Teleport(Map map) {
            var pos = map.GetTileFoot(TileX, TileY);
            Sprite.Position = new Vector2(pos.X, pos.Y + Height / 2);
        }

        public void SetTilePosition(int x, int y, int layer, Map map) {
            if (map.GetEntity(TileX, TileY, Layer) == this) {
                map.RemoveEntity(this);
            }

            if (map.GetEntity(x, y, layer) != null) {
                Debug.Assert(false);
            }

            TileX = x;
            TileY = y ;
            Layer = layer;
            map.AddEntity(this);
            var pos = map.GetTileFoot(TileX, TileY);
            Sprite.Position = new Vector2(pos.X, pos.Y + Height / 2);
        }
    }
}