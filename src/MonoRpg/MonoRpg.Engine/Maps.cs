namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;

    using MonoRpg.Engine.Tiled;

    public class Maps {
        private static readonly Lazy<Maps> _lazy = new Lazy<Maps>(() => new Maps());
        public static Maps Instance => _lazy.Value;

        private readonly Dictionary<string, Func<TiledMap>> _maps = new Dictionary<string, Func<TiledMap>>();

        public TiledMap GetMap(string mapName) {
            if (_maps.ContainsKey(mapName)) {
                return _maps[mapName]();
            }
            throw new ArgumentException($"Map not found: {mapName}", nameof(mapName));
        }

        public void AddMap(string mapFile, Dictionary<string, MapAction> actions = null, Dictionary<string, TriggerTypeDef> triggerTypes = null, List<TriggerDef> triggers = null, List<MapAction> onWake=null) {
            var path = mapFile;
            if (!File.Exists(path) && File.Exists(Path.Combine("Content", path))) {
                path = Path.Combine("Content", path);
            }
            Func<TiledMap> func = () => {
                var map = Content.LoadMap(path);
                if (actions != null) {
                    map.Actions = actions;
                }
                if (triggerTypes != null) {
                    map.TriggerTypes = triggerTypes;
                }
                if (triggers != null) {
                    map.Triggers = triggers;
                }
                if (onWake != null) {
                    map.OnWake = onWake;
                }

                return map;
            };
            _maps.Add(mapFile, func);
            _maps.Add(Path.GetFileNameWithoutExtension(mapFile), func);
        }
    }
}