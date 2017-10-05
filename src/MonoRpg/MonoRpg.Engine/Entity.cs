namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using MonoRpg.Engine.RenderEngine;

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
        public Dictionary<string, Entity> Children { get; set; }

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
            X = def.X;
            Y = def.Y;
            Children = new Dictionary<string,Entity>();
        }

        public void AddEntity(string id, Entity entity) {
            Debug.Assert(!Children.ContainsKey(id));
            Children.Add(id,entity);
        }

        public void RemoveChild(string id) {
            Debug.Assert(Children.ContainsKey(id));
            Children.Remove(id);
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
            X = pos.X;
            Y = pos.Y;
        }

        public void Render(Renderer renderer) {
            renderer.DrawSprite(Sprite);
            foreach (var child in Children.Select(kv=>kv.Value)) {
                var sprite = child.Sprite;
                Debug.Print("{0}, {1}", X + child.X, Y + child.Y);
                sprite.Position = new Vector2(X + child.X, Y + child.Y);
                renderer.DrawSprite(sprite);
            }
        }
    }
}