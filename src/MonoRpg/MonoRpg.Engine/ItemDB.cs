using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine {
    using global::System.IO;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class ItemDB {
        public static List<Item> Items = new List<Item> {
            EmptyItem
        };
        public static Item EmptyItem { get => Item.EmptyItem; }

        public static void Initialize(params Item[] items) {
            Items = new List<Item> {
                EmptyItem
            };
            foreach (var item in items) {
                Items.Add(item);
            }
        }

        public static void Initialize(string filename) {
            
            var items = JsonConvert.DeserializeObject<Item[]>(File.ReadAllText(filename));
            Initialize(items);
        }
    }


    public class Item {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Special { get; set; }
        public ItemStats Stats { get; set; }
        public ItemType Type { get; set; }

        public Item() {
            Stats = new ItemStats();
        }

        public static readonly Item EmptyItem = new Item {
            Name = string.Empty,
            Description = string.Empty,
            Special = string.Empty,
            Type = ItemType.None,
            Stats = new ItemStats {
                Strength = 0,
                Speed = 0,
                Intelligence = 0,
                Attack = 0,
                Defense = 0,
                Magic = 0,
                Resist = 0
            }
        };
    }

    public class ItemStats {
        public int Strength { get; set; }
        public int Speed { get; set; }
        public int Intelligence { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Magic { get; set; }
        public int Resist { get; set; }
    }

    public enum ItemType {
        None=-1,
        Accessory=1,
        Useable=0,
        Weapon=2,
        Armor=3,
        Key=4,
    }

    public class ItemCount {
        public ItemCount(int itemId, int count) {
            ItemId = itemId;
            Count = count;
        }

        public int ItemId { get; set; }
        public int Count { get; set; }
    }
}
