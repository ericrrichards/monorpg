namespace MonoRpg.Engine.GameStates {
    using global::System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    using MonoRpg.Engine.UI;

    public class ItemMenuState : State {
        public InGameMenu Parent { get; set; }
        public StateStack Stack { get; set; }
        public StateMachine StateMachine { get; private set; }
        public Layout Layout { get; private set; }
        public Scrollbar Scrollbar { get; private set; }
        public Selection<ItemCount>[] ItemMenus { get; set; }
        public Selection<ItemCount> KeyItemMenu { get; set; }
        public Selection<ItemCount> ItemMenu { get; set; }
        public Selection<string> CategoryMenu { get; set; }

        public bool InCategoryMenu { get; set; }

        public List<Panel> Panels { get; set; }
        private Selection<ItemCount> SelectedMenu => ItemMenus[CategoryMenu.GetIndex()];

        public ItemMenuState(InGameMenu parent) : base("items") {

            Parent = parent;
            Stack = parent.Stack;
            StateMachine = parent.StateMachine;

            Layout = new Layout().Contract("screen", 118, 40)
                                 .SplitHorizontal("screen", "top", "bottom", 0.12f, 2)
                                 .SplitVertical("top", "title", "category", 0.6f, 2)
                                 .SplitHorizontal("bottom", "mid", "inv", 0.14f, 2);
            Panels = new List<Panel> {
                Layout.CreatePanel("title"),
                Layout.CreatePanel("category"),
                Layout.CreatePanel("mid"),
                Layout.CreatePanel("inv")
            };

            Scrollbar = new Scrollbar(System.Content.FindTexture("scrollbar.png"), 228);

            ItemMenu = new Selection<ItemCount>(
                                                System.Renderer,
                                                new SelectionArgs<ItemCount>(World.Instance.Items) {
                                                    SpacingX = 256,
                                                    Columns = 2,
                                                    DisplayRows = 8,
                                                    SpacingY = 28,
                                                    Rows = 20,
                                                    RenderItem = (renderer, x, y, item) => World.DrawItem(ItemMenu, renderer, x, y, item)
                                             });
            KeyItemMenu = new Selection<ItemCount>(System.Renderer, new SelectionArgs<ItemCount>(World.Instance.KeyItems) {
                SpacingY = 28,
                SpacingX = 256,
                Columns = 2,
                DisplayRows = 8,
                Rows = 20,
                RenderItem = (renderer, x, y, item) => World.DrawKey(KeyItemMenu, renderer, x, y, item)
            });
            CategoryMenu = new Selection<string>(System.Renderer, new SelectionArgs<string>(new List<string>{"Use", "Key Items"}) {
                OnSelection = (index, item) => OnCategorySelect(index, item),
                SpacingX = 150,
                Columns = 2,
                Rows = 1
            });
            ItemMenus = new[] { ItemMenu, KeyItemMenu };
            InCategoryMenu = true;

            KeyItemMenu.HideCursor();
            ItemMenu.HideCursor();
        }

        

        private void OnCategorySelect(int index, string item) {
            CategoryMenu.HideCursor();
            InCategoryMenu = false;
            if (item == "Use") {
                ItemMenu.ShowCursor();
            } else {
                KeyItemMenu.ShowCursor();
            }
        }

        

        public override void Render(Renderer renderer) {
            foreach (var panel in Panels) {
                panel.Render(renderer);
            }

            var titleX = Layout.CenterX("title");
            var titleY = Layout.CenterY("title");
            renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            renderer.DrawText2D(titleX, titleY, "Items", Color.White, 1.5f);

            renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
            var categoryX = Layout.Left("category") + 5;
            var categoryY = Layout.CenterY("category");
            CategoryMenu.TextScale = 1.5f;
            CategoryMenu.Position = new Vector2(categoryX, categoryY);
            CategoryMenu.Render(renderer);

            var descX = Layout.Left("mid") + 10;
            var descY = Layout.CenterY("mid");

            var menu = SelectedMenu;

            if (!InCategoryMenu) {
                var description = string.Empty;
                var selectedItem = menu.SelectedItem;
                if (selectedItem != null) {
                    var itemId = selectedItem.ItemId;
                    var itemDef = ItemDB.Items[itemId];
                    if (itemDef != null) {
                        description = itemDef.Description;
                    }
                }
                renderer.DrawText2D(descX, descY, description);
            }
            var itemX = Layout.Left("inv") - 6;
            var itemY = Layout.Top("inv") - 20;
            menu.Position = new Vector2(itemX, itemY);
            menu.Render(renderer);

            var scrollX = Layout.Right("inv")-14;
            var scrollY = Layout.CenterY("inv");
            Scrollbar.SetPosition(scrollX, scrollY);
            Scrollbar.Render(renderer);

        }

        public override bool Update(float dt) {
            var menu = SelectedMenu;

            if (InCategoryMenu) {
                if (System.Keys.WasReleased(Keys.Back) || System.Keys.WasReleased(Keys.Escape)) {
                    StateMachine.Change("frontmenu");
                }
                CategoryMenu.HandleInput();
            } else {
                if (System.Keys.WasReleased(Keys.Back) || System.Keys.WasReleased(Keys.Escape)) {
                    FocusOnCategoryMenu();
                }
                menu.HandleInput();
            }

            var scrolled = menu.PercentageScrolled;
            Scrollbar.SetScrollCaretScale(menu.PercentageShown);
            Scrollbar.SetNormalValue(scrolled);
            return false;
        }

        private void FocusOnCategoryMenu() {
            InCategoryMenu = true;
            var menu = SelectedMenu;
            menu.HideCursor();
            CategoryMenu.ShowCursor();
        }

        
    }
}