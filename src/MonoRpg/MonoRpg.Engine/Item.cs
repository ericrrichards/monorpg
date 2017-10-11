namespace MonoRpg.Engine {
    using global::System.Collections.Generic;

    public class Item {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Special { get; set; }
        //public List<Modifier> Stats { get; set; }
        public ItemType Type { get; set; }

        public Item() {
            //Stats = new List<Modifier>();
        }

        public static readonly Item EmptyItem = new Item {
            Name = string.Empty,
            Description = string.Empty,
            Special = string.Empty,
            Type = ItemType.None,
            //Stats = new List<Modifier>()
        };
    }
}