using System.Collections.Generic;
namespace MonoRpg.Engine.Tiled {
    using global::System;
    using global::System.IO;
    using global::System.Linq;

    public class TiledMap {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public List<TileLayer> Layers { get; set; }
        public List<TileSet> TileSets { get; set; }
        public List<MapAction> OnWake { get; set; }
        public Dictionary<string, MapAction> Actions { get; set; }
        public Dictionary<string, TriggerTypeDef> TriggerTypes { get; set; }
        public List<TriggerDef> Triggers { get; set; }

        public TiledMap() {
            OnWake = new List<MapAction>();
            Actions = new Dictionary<string, MapAction>();
            TriggerTypes = new Dictionary<string, TriggerTypeDef>();
            Triggers = new List<TriggerDef>();
        }
    }

    public struct MapAction {
        public string ID { get; set; }
        public MapActionParameters Params { get; set; }

        public static MapAction RunScript(Action<Map, TriggerDef, Entity> script, TriggerDef def) {
            return new MapAction {
                ID = "RunScript",
                Params = new RunScriptArgs {
                    Script = script,
                    TriggerDef = def
                }
            };
        }

        public static MapAction AddNpc(string id, string character, int x, int y) {
            return new MapAction {
                ID = "AddNPC",
                Params = new AddNPCParams {
                    Character = character,
                    Id = id,
                    X = x,
                    Y = y
                }
            };
        }
    }

    public class MapActionParameters {

    }

    public class AddNPCParams : MapActionParameters {
        public string Character { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Layer { get; set; }
        public string Id { get; set; }
    }

    public class TeleportParams : MapActionParameters {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class RunScriptArgs : MapActionParameters {
        public Action<Map, TriggerDef, Entity> Script { get; set; }
        public TriggerDef TriggerDef { get; set; }
    }

    public struct TriggerTypeDef {
        public string OnEnter { get; set; }
        public string OnExit { get; set; }
        public string OnUse { get; set; }
    }

    public struct TriggerDef {
        public string Trigger { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Layer { get; set; }

        public TriggerDef(string triggerTypeName, int x, int y, int layer = 0) {
            Trigger = triggerTypeName;
            X = x;
            Y = y;
            Layer = layer;
        }
    }
}
