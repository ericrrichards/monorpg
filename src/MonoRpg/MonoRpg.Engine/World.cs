namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;

    public class World {
        public float Time { get; set; }
        public string TimeString => new TimeSpan(0, 0, (int)Time).ToString("hh\\:mm\\:ss");
        public int Gold { get; set; }
        public List<ItemCount> Items { get; set; }
        public List<int?> KeyItems { get; set; }
        public World() {
            Time = 0f;
            Gold = 0;
            Items = new List<ItemCount>();
            KeyItems = new List<int?>();
        }

        public void Update(float dt) {
            Time += dt;
        }

        public void AddItem(int itemId, int count = 1) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type!= ItemType.Key);

            if (!Items.Any(i=>i.ItemId ==itemId)) {
                Items.Add(new ItemCount(itemId, count));
            } else {
                var item = Items.First(ic => ic.ItemId == itemId).Count += count;
            }
        }

        public void RemoveItem(int itemId, int count = 1) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type != ItemType.Key);
            var item = Items.FirstOrDefault(i=>i.ItemId==itemId);
            Debug.Assert(item!=null && item.Count >= count);
            item.Count -= count;
        }

        public bool HasKey(int itemId) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type == ItemType.Key);
            return KeyItems.Contains(itemId);
        }

        public void AddKey(int itemId) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type == ItemType.Key);
            Debug.Assert(!HasKey(itemId));
            KeyItems.Add(itemId);
        }

        public void RemoveKey(int itemId) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type == ItemType.Key);
            Debug.Assert(HasKey(itemId));
            KeyItems.Remove(itemId);
        }
    }
}