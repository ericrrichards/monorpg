using System;
using System.Collections.Generic;
using System.Diagnostics;

using MonoRpg.Engine.Tiled;

namespace MonoRpg.Engine {
    public static class Actions {
        public static readonly Dictionary<string, Action<Map, MapActionParameters, Entity>> ActionFuncs = new Dictionary<string, Action<Map, MapActionParameters, Entity>> {
            {"AddNPC", AddNPC },
            {"Teleport", Teleport },
            {"RunScript", RunScript }
        };

        public static void AddNPC(Map map, MapActionParameters args, Entity arg3) {
            var npc = args as AddNPCParams;
            if (npc == null) {
                return;
            }
            Debug.Assert(npc.Character != "hero");
            Debug.Assert(EntityDefs.Instance.Characters.ContainsKey(npc.Character));
            var charDef = EntityDefs.Instance.Characters[npc.Character];
            var character = new Character(charDef, map);

            var x = npc.X ?? character.Entity.TileX;
            var y = npc.Y ?? character.Entity.TileY;
            var layer = npc.Layer ?? character.Entity.Layer;

            character.Entity.SetTilePosition(x, y, layer, map);
            Debug.Assert(!map.NpcById.ContainsKey(npc.Id));
            character.Id = npc.Id;
            map.NpcById[npc.Id] = character;
            map.NPCs.Add(character);

        }

        public static void Teleport(Map map, MapActionParameters args, Entity entity) {
            var teleport = args as TeleportParams;
            if (teleport == null) {
                return;
            }
            
            entity.SetTilePosition(teleport.X, teleport.Y, entity.Layer, map);
        }

        public static void RunScript(Map map, MapActionParameters args, Entity entity) {
            var scriptArgs = args as RunScriptArgs;
            Debug.Assert(scriptArgs!=null);
            scriptArgs.Script(map, scriptArgs.TriggerDef, entity);
        }
    }
}