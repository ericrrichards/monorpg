namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;

    using JetBrains.Annotations;

    using Microsoft.Xna.Framework;

    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.Tiled;

    public class Map {
        private int Left { get; }
        private int Top { get; }

        public int CamX { get; private set; }
        public int CamY { get; private set; }

        private TiledMap MapDef { get; }
        private Sprite Sprite { get; }

        private int WidthInTiles { get; }
        private int HeightInTiles { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public int LayerCount => MapDef.Layers.Count / 3;

        private int WidthInPixels => WidthInTiles * TileWidth;
        private int HeightInPixels => HeightInTiles * TileHeight;
        private List<Rectangle> UVs { get; }
        private int BlockingTile { get; }

        private Dictionary<int, Dictionary<int, Trigger>> Triggers { get; }
        private Dictionary<int, Dictionary<int, Entity>> Entities { get; }

        private Dictionary<string, Action<Entity>> Actions { get; }
        private Dictionary<string, Trigger> TriggerTypes { get; }


        public List<Character> NPCs { get; }
        public Dictionary<string, Character> NpcById { get; }

        public Map(TiledMap mapDef) {
            MapDef = mapDef;
            var baseTexture = System.Content.FindTexture(mapDef.TileSets[0].Image);
            Sprite = new Sprite();
            WidthInTiles = mapDef.Width;
            HeightInTiles = mapDef.Height;
            TileWidth = mapDef.TileWidth;
            TileHeight = mapDef.TileHeight;

            Triggers = new Dictionary<int, Dictionary<int, Trigger>>();
            Entities = new Dictionary<int, Dictionary<int, Entity>>();
            NPCs = new List<Character>();
            NpcById = new Dictionary<string, Character>();



            Sprite.Texture = baseTexture;

            Left = -System.Screen.HalfWidth + TileWidth / 2;
            Top = System.Screen.HalfHeight - TileHeight / 2;

            UVs = baseTexture.GenerateUVs(TileWidth, TileHeight);

            foreach (var tileSet in mapDef.TileSets) {
                if (tileSet.Name == "collision_graphic") {
                    BlockingTile = tileSet.FirstGid;
                    break;
                }
            }
            Debug.Assert(BlockingTile > 0);

            Actions = new Dictionary<string, Action<Entity>>();
            foreach (var mapDefAction in mapDef.Actions) {
                Debug.Assert(Engine.Actions.ActionFuncs.ContainsKey(mapDefAction.Value.ID));
                void Action(Entity entity) => Engine.Actions.ActionFuncs[mapDefAction.Value.ID](this, mapDefAction.Value.Params, entity);
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
                AddTrigger(triggerDef);
            }



            foreach (var mapAction in mapDef.OnWake) {
                var action = Engine.Actions.ActionFuncs[mapAction.ID];
                action(this, mapAction.Params, null);
            }

        }



        private (int x, int y) PointToTile(int x, int y) {

            x = Math.Min(Left + WidthInPixels - 1, Math.Max(Left, x + TileWidth / 2));
            y = Math.Max(Top - HeightInPixels + 1, Math.Min(Top, y - TileHeight / 2));

            var tileX = (int)Math.Floor((double)(x - Left) / TileWidth);
            var tileY = (int)Math.Floor((double)(Top - y) / TileHeight);

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
            return tiles[index]; // Tiled uses 1 as the first ID, instead of 0 like everything else in the world does.
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

        private int CoordToIndex(int x, int y) {
            var index = x + y * WidthInTiles;
            return index;
        }

        public void Goto(int x, int y) {
            SetCameraPosition(x - System.Screen.HalfWidth, -y + System.Screen.HalfHeight);
        }

        public void SetCameraPosition(int x, int y) {
            CamX = x;
            CamY = y;
        }

        public void GotoTile(int x, int y) {
            Goto(x * TileWidth + TileWidth / 2, y * TileHeight + TileHeight / 2);
        }

        public Point GetTileFoot(int x, int y) {
            return new Point(Left + x * TileWidth, Top - y * TileHeight - TileHeight / 2);
        }



        public void Render(Renderer renderer) {
            RenderLayer(renderer, 0);
        }

        public void RenderLayer(Renderer renderer, int layer, Entity hero = null) {
            var layerIndex = layer * 3;

            var (left, bottom) = PointToTile(CamX - System.Screen.HalfWidth, CamY - System.Screen.HalfHeight);
            var (right, top) = PointToTile(CamX + System.Screen.HalfWidth, CamY + System.Screen.HalfHeight);


            for (var j = top; j <= bottom; j++) {
                for (var i = left; i <= right; i++) {
                    var tile = GetTile(i, j, layerIndex);
                    Rectangle uvs;
                    Sprite.Position = new Vector2(Left + i * TileWidth, Top - j * TileHeight);
                    if (tile > 0) {
                        uvs = UVs[tile - 1];
                        Sprite.SetUVs(uvs);
                        renderer.DrawSprite(Sprite);
                    }
                    tile = GetTile(i, j, layerIndex + 1);
                    if (tile > 0) {
                        uvs = UVs[tile - 1];
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

        public void WriteTile(WriteTileArgs args) {
            var layer = args.Layer;
            var detail = args.Detail;
            layer = layer * 3;

            var x = args.X;
            var y = args.Y;
            var tile = args.Tile;
            var collision = BlockingTile;
            if (!args.Collision) {
                collision = 0;
            }
            var index = CoordToIndex(x, y);
            var tiles = MapDef.Layers[layer].Data;
            tiles[index] = tile;

            tiles = MapDef.Layers[layer + 1].Data;
            tiles[index] = detail;

            tiles = MapDef.Layers[layer + 2].Data;
            tiles[index] = collision;
        }

        public void RemoveTrigger(int x, int y, int layer = 0) {
            Debug.Assert(Triggers[layer] != null);
            var triggers = Triggers[layer];
            var index = CoordToIndex(x, y);
            Debug.Assert(triggers.ContainsKey(index));
            triggers.Remove(index);
        }
        public void AddTrigger(TriggerDef triggerDef) {
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
    }

    public class WriteTileArgs {
        public int Layer = 0;
        public int Detail = 0;
        public int X;
        public int Y;
        public int Tile;
        public bool Collision;

    }
}
