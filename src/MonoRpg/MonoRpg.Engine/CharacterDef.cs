namespace MonoRpg.Engine {
    using global::System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public struct CharacterDef {
        public string Entity { get; set; }
        public Dictionary<Animations, List<int>> Animations { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Facing Facing { get; set; }
        public List<string> Controller { get; set; }
        public string State { get; set; }
    }
}