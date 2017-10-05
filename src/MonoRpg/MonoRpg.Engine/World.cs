namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;

    using Microsoft.Xna.Framework;

    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.UI;

    public class World {
        private static readonly Lazy<World> Lazy = new Lazy<World>(() => new World());
        public static World Instance => Lazy.Value;

        public float Time { get; set; }
        public string TimeString => new TimeSpan(0, 0, (int)Time).ToString("hh\\:mm\\:ss");
        public int Gold { get; set; }
        public List<ItemCount> Items { get; set; }
        public List<ItemCount> KeyItems { get; set; }
        public World() {
            Time = 0f;
            Gold = 0;
            Items = new List<ItemCount>();
            KeyItems = new List<ItemCount>();
        }

        public void Update(float dt) {
            Time += dt;
        }

        public void AddItem(int itemId, int count = 1) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type != ItemType.Key);

            if (!Items.Any(i => i.ItemId == itemId)) {
                Items.Add(new ItemCount(itemId, count));
            } else {
                var item = Items.First(ic => ic.ItemId == itemId).Count += count;
            }
        }

        public void RemoveItem(int itemId, int count = 1) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type != ItemType.Key);
            var item = Items.FirstOrDefault(i => i.ItemId == itemId);
            Debug.Assert(item != null && item.Count >= count);
            item.Count -= count;
            if (item.Count == 0) {
                Items.Remove(item);
            }
        }

        public bool HasKey(int itemId) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type == ItemType.Key);
            return KeyItems.Any(i=>i.ItemId == itemId);
        }

        public void AddKey(int itemId) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type == ItemType.Key);
            Debug.Assert(!HasKey(itemId));
            KeyItems.Add(new ItemCount(itemId, 1));
        }

        public void RemoveKey(int itemId) {
            Debug.Assert(itemId < ItemDB.Items.Count && itemId > 0);
            Debug.Assert(ItemDB.Items[itemId].Type == ItemType.Key);
            Debug.Assert(HasKey(itemId));
            KeyItems.RemoveAll(i=>i.ItemId == itemId);
        }

        public static void DrawKey(Selection<ItemCount> menu, Renderer renderer, int x, int y, ItemCount item) {
            if (item != null) {
                var itemDef = ItemDB.Items[item.ItemId];
                renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
                renderer.DrawText2D(x, y, itemDef.Name);
            } else {
                renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
                renderer.DrawText2D(x + menu.SpacingX / 2, y, "--");
            }
        }

        public static void DrawItem(Selection<ItemCount> menu, Renderer renderer, int x, int y, ItemCount item) {
            
            if (item != null) {
                var itemDef = ItemDB.Items[item.ItemId];
                var sprite = Icons.Instance.Get(itemDef.Type);
                if (sprite != null) {
                    sprite.Position = new Vector2(x+6, y);
                    renderer.DrawSprite(sprite);
                }
                renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
                renderer.DrawText2D(x + 18, y, itemDef.Name);
                var right = x + menu.SpacingX - 64;
                renderer.AlignText(TextAlignment.Right, TextAlignment.Center);
                renderer.DrawText2D(right, y, item.Count.ToString());
            } else {
                renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
                renderer.DrawText2D(x + menu.SpacingX/2, y, " - ");
            }
        }
    }
}