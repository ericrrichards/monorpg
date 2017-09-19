namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;

    using JetBrains.Annotations;

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

        public Dictionary<int, Dictionary<int, Trigger>> Triggers { get; set; }
        public Dictionary<int, Dictionary<int, Entity>> Entities { get; set; }
        public List<Character> NPCs { get; set; }
        public Dictionary<string, Action<Entity>> Actions { get; set; }
        public Dictionary<string, Trigger> TriggerTypes { get; set; }

        public int LayerCount {
            get {
                Debug.Assert(MapDef.Layers.Count % 3 == 0);
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

            Triggers = new Dictionary<int, Dictionary<int, Trigger>>();
            Entities = new Dictionary<int, Dictionary<int, Entity>>();
            NPCs = new List<Character>();
            



            Sprite.Texture = TextureAtlas;

            X = -System.ScreenWidth / 2 + TileWidth / 2;
            Y = System.ScreenHeight / 2 - TileHeight / 2;

            WidthInPixels = Width * TileWidth;
            HeightInPixels = Height * TileHeight;

            UVs = TextureAtlas.GenerateUVs(TileWidth, TileHeight);

            foreach (var tileSet in mapDef.TileSets) {
                if (tileSet.Name == "collision_graphic") {
                    BlockingTile = tileSet.FirstGid - 1;
                    break;
                }
            }
            Debug.Assert(BlockingTile > 0);

            Actions = new Dictionary<string, Action<Entity>>();
            foreach (var mapDefAction in mapDef.Actions) {
                Debug.Assert(global::MonoRpg.Engine.Actions.ActionFuncs.ContainsKey(mapDefAction.Value.ID));
                void Action(Entity entity) => global::MonoRpg.Engine.Actions.ActionFuncs[mapDefAction.Value.ID](this, mapDefAction.Value.Params, entity);
                Actions[mapDefAction.Key] = Action;
            }
            TriggerTypes = new Dictionary<string, Trigger>();
            foreach (var triggerType in mapDef.TriggerTypes) {
                Action<Entity> enter = null;
                Action<Entity> exit = null;
                Action<Entity> use = null;
                if (triggerType.Value.OnEnter != null) {
                    enter = Actions[triggerType.Value.OnEnter];
                }
                if (triggerType.Value.OnExit != null) {
                    exit = Actions[triggerType.Value.OnExit];
                }
                if (triggerType.Value.OnUse != null) {
                    use = Actions[triggerType.Value.OnUse];
                }
                TriggerTypes[triggerType.Key] = new Trigger(enter, exit, use);

            }

            foreach (var triggerDef in mapDef.Triggers) {
                var x = triggerDef.X;
                var y = triggerDef.Y;
                var layer = triggerDef.Layer;

                if (!Triggers.ContainsKey(layer)) {
                    Triggers[layer] = new Dictionary<int, Trigger>();
                }

                var targetLayer = Triggers[layer];
                Debug.Assert(TriggerTypes.ContainsKey(triggerDef.Trigger));
                var trigger = TriggerTypes[triggerDef.Trigger];
                targetLayer[CoordToIndex(x, y)] = trigger;
            }



            foreach (var mapAction in mapDef.OnWake) {
                var action = global::MonoRpg.Engine.Actions.ActionFuncs[mapAction.ID];
                action(this, mapAction.Params, null);
            }

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
                var entity = GetEntity(tileX, tileY, layer);
                return tile == BlockingTile || entity != null;
            } catch (IndexOutOfRangeException) {
                return true;
            }
        }

        private int GetTile(int x, int y, int layer = 0) {
            var tiles = MapDef.Layers[layer].Data;
            var index = CoordToIndex(x, y);
            if (index < 0 || index >= tiles.Count)
                throw new IndexOutOfRangeException();
            return tiles[index] - 1; // Tiled uses 1 as the first ID, instead of 0 like everything else in the world does.
        }

        [CanBeNull]
        public Trigger GetTrigger(int layer, int x, int y) {
            if (!Triggers.ContainsKey(layer)) {
                return null;
            }
            var triggers = Triggers[layer];
            var index = CoordToIndex(x, y);
            if (triggers.ContainsKey(index)) {
                return triggers[index];
            }
            return null;
        }

        public int CoordToIndex(int x, int y) {
            var index = x + y * Width;
            return index;
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

        public void RenderLayer(Renderer renderer, int layer, Entity hero = null) {
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
                    tile = GetTile(i, j, layerIndex + 1);
                    if (tile >= 0) {
                        uvs = UVs[tile];
                        Sprite.SetUVs(uvs);
                        renderer.DrawSprite(Sprite);
                    }
                }
                var entLayer = Entities.ContainsKey(layer) ? Entities[layer] : new Dictionary<int, Entity>();
                var drawList = new List<Entity>();
                if (hero != null) {
                    drawList.Add(hero);
                }

                foreach (var entity in entLayer) {
                    drawList.Add(entity.Value);
                }
                drawList = drawList.OrderBy(e => e.TileY).ToList();
                foreach (var entity in drawList) {
                    entity.Render(renderer);
                    //renderer.DrawSprite(entity.Sprite);
                }
            }
        }

        public Entity GetEntity(int x, int y, int layer) {
            if (!Entities.ContainsKey(layer)) {
                return null;
            }
            var index = CoordToIndex(x, y);
            if (!Entities[layer].ContainsKey(index)) {
                return null;
            }
            return Entities[layer][index];
        }

        public void AddEntity(Entity entity) {
            if (!Entities.ContainsKey(entity.Layer)) {
                Entities[entity.Layer] = new Dictionary<int, Entity>();
            }
            var layer = Entities[entity.Layer];
            var index = CoordToIndex(entity.TileX, entity.TileY);
            Debug.Assert(!layer.ContainsKey(index) || layer[index] == entity);
            layer[index] = entity;
        }

        public void RemoveEntity(Entity entity) {
            Debug.Assert(Entities.ContainsKey(entity.Layer));
            var layer = Entities[entity.Layer];
            var index = CoordToIndex(entity.TileX, entity.TileY);
            Debug.Assert(entity == layer[index]);
            layer.Remove(index);
        }
    }
}
