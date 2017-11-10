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
        private int LeftTile { get; }
        private int TopTile { get; }

        public int CamX { get; private set; }
        public int CamY { get; private set; }

        private TiledMap MapDef { get; }
        private Sprite Sprite { get; }

        private int WidthInTiles { get; }
        private int HeightInTiles { get; }
        public int TileWidth { get; }
        private int HalfTileWidth => TileWidth / 2;
        public int TileHeight { get; }
        private int HalfTileHeight => TileHeight / 2;
        private int LayerCount => MapDef.MapDef.LayerCount / 3;

        private int WidthInPixels => WidthInTiles * TileWidth;
        private int HeightInPixels => HeightInTiles * TileHeight;
        private List<Rectangle> UVs { get; }
        private int BlockingTile { get; }

        private Dictionary<int, Dictionary<int, Trigger>> Triggers { get; }
        private Dictionary<int, Dictionary<int, Entity>> Entities { get; }

        private Dictionary<string, Action<Entity>> Actions { get; }
        private Dictionary<string, Trigger> TriggerTypes { get; }

        private List<Character> NPCs { get; }
        private Dictionary<string, Character> NpcById { get; }

        public Map(TiledMap mapDef) {
            MapDef = mapDef;
            var baseTexture = System.Content.FindTexture(mapDef.MapDef.TileSets[0].GetImageFilename());
            Sprite = new Sprite();
            WidthInTiles = mapDef.MapDef.Width;
            HeightInTiles = mapDef.MapDef.Height;
            TileWidth = mapDef.MapDef.TileWidth;
            TileHeight = mapDef.MapDef.TileHeight;

            Triggers = new Dictionary<int, Dictionary<int, Trigger>>();
            Entities = new Dictionary<int, Dictionary<int, Entity>>();
            NPCs = new List<Character>();
            NpcById = new Dictionary<string, Character>();



            Sprite.Texture = baseTexture;

            LeftTile = System.Screen.Bounds.Left + HalfTileWidth;
            TopTile = System.Screen.Bounds.Top - HalfTileHeight;

            UVs = baseTexture.GenerateUVs(TileWidth, TileHeight);

            BlockingTile = (mapDef.MapDef.TileSets.FirstOrDefault(ts => ts.Name == "collision_graphic")?.FirstGID).GetValueOrDefault();
            
            Debug.Assert(BlockingTile > 0);
            
            Actions = new Dictionary<string, Action<Entity>>();
            foreach (var mapDefAction in mapDef.Actions) {
                Actions[mapDefAction.Key] = entity => Engine.Actions.ActionFuncs[mapDefAction.Value.ID](this, mapDefAction.Value.Params, entity);
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
                Engine.Actions.ActionFuncs[mapAction.ID](this, mapAction.Params, null);
            }

        }



        private (int x, int y) PointToTile(int x, int y) {

            x = Math.Min(LeftTile + WidthInPixels - 1, Math.Max(LeftTile, x + HalfTileWidth));
            y = Math.Max(TopTile - HeightInPixels + 1, Math.Min(TopTile, y - HalfTileHeight));

            var tileX = (int)Math.Floor((double)(x - LeftTile) / TileWidth);
            var tileY = (int)Math.Floor((double)(TopTile - y) / TileHeight);

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
            return MapDef.MapDef.GetTile(x, y, layer);
        }

        [CanBeNull]
        public Trigger GetTrigger(int layer, int x, int y) {
            if (!Triggers.ContainsKey(layer)) {
                return null;
            }
            var triggers = Triggers[layer];
            var index = CoordToIndex(x, y);
            return triggers.ContainsKey(index) ? triggers[index] : null;
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
            Goto(x * TileWidth + HalfTileWidth, y * TileHeight + HalfTileHeight);
        }

        public Point GetTileFoot(int x, int y) {
            return new Point(LeftTile + x * TileWidth, TopTile - y * TileHeight - HalfTileHeight);
        }



        public void Render(Renderer renderer, Entity hero = null) {
            renderer.Translate(-CamX, -CamY);
            for (var layer = 0; layer < LayerCount; layer++) {
                if (layer == hero?.Layer) {
                    RenderLayer(renderer, layer, hero);
                } else {
                    RenderLayer(renderer, layer);
                }
            }
            renderer.Translate(0, 0);
        }

        private void RenderLayer(Renderer renderer, int layer, Entity hero = null) {
            var layerIndex = layer * 3;

            var (left, bottom) = PointToTile(CamX - System.Screen.HalfWidth, CamY - System.Screen.HalfHeight);
            var (right, top) = PointToTile(CamX + System.Screen.HalfWidth, CamY + System.Screen.HalfHeight);


            for (var y = top; y <= bottom; y++) {
                for (var x = left; x <= right; x++) {
                    DrawTile(renderer, x, y, layerIndex);
                    DrawTile(renderer, x, y, layerIndex + 1);

                }
            }
            DrawEntities(renderer, layer, hero);
        }

        private void DrawEntities(Renderer renderer, int layer, Entity hero) {
            var entLayer = Entities.ContainsKey(layer) ? Entities[layer] : new Dictionary<int, Entity>();
            var drawList = new List<Entity>();
            if (hero != null) {
                drawList.Add(hero);
            }

            drawList.AddRange(entLayer.Values);
            drawList = drawList.OrderBy(e => e.TileY).ToList();
            foreach (var entity in drawList) {
                entity.Render(renderer);
            }
        }

        private void DrawTile(Renderer renderer, int x, int y, int layer) {
            var tile = GetTile(x, y, layer);
            if (tile <= 0)
                return;
            Sprite.Position = new Vector2(LeftTile + x * TileWidth, TopTile - y * TileHeight);
            var uvs = UVs[tile - 1];
            Sprite.SetUVs(uvs);
            renderer.DrawSprite(Sprite);
        }

        public Entity GetEntity(int x, int y, int layer) {
            if (!Entities.ContainsKey(layer)) {
                return null;
            }
            var index = CoordToIndex(x, y);
            return Entities[layer].ContainsKey(index) ? Entities[layer][index] : null;
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

        public void AddCharacter(Character character) {
            Debug.Assert(!NpcById.ContainsKey(character.Id));
            NpcById[character.Id] = character;
            NPCs.Add(character);
        }

        public IReadOnlyList<Character> Characters => NPCs.AsReadOnly();

        public Character GetNpc(string id) {
            Debug.Assert(NpcById.ContainsKey(id));
            return NpcById[id];
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
            layer = layer * 3;
            MapDef.MapDef.WriteTile(args.X, args.Y, layer, args.Tile);
            MapDef.MapDef.WriteTile(args.X, args.Y, layer+1, args.Detail);
            MapDef.MapDef.WriteTile(args.X, args.Y, layer+2, args.Collision ? BlockingTile : 0);
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
