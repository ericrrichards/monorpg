namespace MonoRpg.Engine.GameStates {
    using global::System;
    using global::System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.UI;

    public class FrontMenu : BaseStateObject {
        public InGameMenu Parent { get; set; }
        public StateMachine StateMachine { get; private set; }
        public Layout Layout { get; private set; }
        public Selection<string> Selections { get; set; }
        public List<Panel> Panels { get; set; }
        public string TopBarText { get; set; }


        public FrontMenu(InGameMenu parent) : base(parent.Stack) {
            var layout = new Layout()
                .Contract("screen", 118, 40)
                .SplitHorizontal("screen", "top", "bottom", 0.12f, 2)
                .SplitVertical("bottom", "left", "party", 0.726f, 2)
                .SplitHorizontal("left", "menu", "gold", 0.7f, 2);

            Parent = parent;
            StateMachine = parent.StateMachine;
            Layout = layout;
            Selections = new Selection<string>(System.Renderer,
                                               new SelectionArgs<string>("Items") {
                                                   SpacingY = 32,
                                                   OnSelection = (i, s) => OnMenuClick(i)

                                               }
                                              );
            Panels = new List<Panel> {
                layout.CreatePanel("gold"),
                layout.CreatePanel("top"),
                layout.CreatePanel("party"),
                layout.CreatePanel("menu")
            };
            TopBarText = "Current Map Name";
        }

        private void OnMenuClick(int index) {
            var items = 0;
            if (index == items) {
                StateMachine.Change("items");
            }
        }

        public override bool Update(float dt) {

            return false;
        }


        public override void Render(Renderer renderer) {
            foreach (var panel in Panels) {
                panel.Render(renderer);
            }

            renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
            var menuX = Layout.Left("menu") + 16;
            var menuY = Layout.Top("menu") - 24;
            Selections.TextScale = Parent.TitleSize;
            Selections.Position = new Vector2(menuX, menuY);
            Selections.Render(renderer);

            var nameX = Layout.CenterX("top");
            var nameY = Layout.CenterY("top");
            renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            renderer.DrawText2D(nameX, nameY, TopBarText);

            var goldX = Layout.CenterX("gold") - 22;
            var goldY = Layout.CenterY("gold") + 22;

            renderer.AlignText(TextAlignment.Right, TextAlignment.Top);
            var scale = Parent.LabelSize;
            renderer.DrawText2D(goldX, goldY, "GP:", Color.White, scale);
            renderer.DrawText2D(goldX, goldY - 25, "TIME:", Color.White, scale);
            renderer.AlignText(TextAlignment.Left, TextAlignment.Top);
            renderer.DrawText2D(goldX + 10, goldY, World.Instance.Gold.ToString(), Color.White, scale);
            renderer.DrawText2D(goldX + 10, goldY - 25, World.Instance.TimeString, Color.White, scale);

        }

        public override void HandleInput(float dt) {
            Selections.HandleInput();

            if (System.Keys.WasPressed(Keys.Back) || System.Keys.WasPressed(Keys.Escape)) {
                Stack.Pop();
            }
        }
    }
}