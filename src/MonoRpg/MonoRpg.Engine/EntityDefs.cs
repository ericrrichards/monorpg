namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;

    using Newtonsoft.Json;

    public  class EntityDefs {
        public static EntityDefs Instance;



        public Dictionary<string, string> CharacterStateTypes = new Dictionary<string, string>();
        public Dictionary<string, Type> CharacterStates => CharacterStateTypes.ToDictionary(kv => kv.Key, kv => Assembly.GetEntryAssembly().GetType(kv.Value));
        public Dictionary<string, EntityDef> Entities = new Dictionary<string, EntityDef>();
        public Dictionary<string, CharacterDef> Characters = new Dictionary<string, CharacterDef>();

        public static void Load(string file) {
            Instance = JsonConvert.DeserializeObject<EntityDefs>(File.ReadAllText(file));
        }
    }
    
}
