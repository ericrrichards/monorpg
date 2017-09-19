namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;

    using MonoRpg.Engine.Tiled;

    public static class MapDB {
        public static Dictionary<string, Func<TiledMap>> Maps = new Dictionary<string, Func<TiledMap>>();

        public static void AddMap(string mapFile, Dictionary<string, MapAction> actions = null, Dictionary<string, TriggerTypeDef> triggerTypes = null, List<TriggerDef> triggers = null) {
            var path = mapFile;
            if (!File.Exists(path) && File.Exists(Path.Combine("Content", path))) {
                path = Path.Combine("Content", path);
            }
            Maps.Add(mapFile,
                     () => {
                         var map = System.Content.LoadMap(path);
                         if (actions != null) {
                             map.Actions = actions;
                         }
                         if (triggerTypes != null) {
                             map.TriggerTypes = triggerTypes;
                         }
                         if (triggers != null) {
                             map.Triggers = triggers;
                         }
                         return map;
                     });
        }
    }
}