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
        public static Item EmptyItem => Item.EmptyItem;

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

    public class ItemCount {
        public ItemCount(int itemId, int count) {
            ItemId = itemId;
            Count = count;
        }

        public int ItemId { get; set; }
        public int Count { get; set; }
    }
}
