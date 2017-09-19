namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;

    using MonoRpg.Engine.Tiled;

    public static class MapDB {
        public static Dictionary<string, Func<TiledMap>> Maps = new Dictionary<string, Func<TiledMap>>();

        public static void AddMap(string mapFile) {
            var path = mapFile;
            if (!File.Exists(path) && File.Exists(Path.Combine("Content", path))) {
                path = Path.Combine("Content", path);
            }
            Maps.Add(mapFile, () => System.Content.LoadMap(path));
        }
    }
}