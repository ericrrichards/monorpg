namespace MonoRpg.Engine.GameStates {
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.UI;

    public class StatusMenuState : BaseStateObject {
        public InGameMenu Parent { get; set; }
        public StateMachine StateMachine { get; private set; }
        public Layout Layout { get; private set; }
        public List<Panel> Panels { get; set; }
        public Actor Actor { get; private set; }
        public ActorSummary ActorSummary { get; private set; }
        public Selection<string> Actions { get; set; }
        public Selection<int> EquipmentMenu { get; set; }


        public StatusMenuState(InGameMenu parent) : base(parent.Stack) {
            Parent = parent;
            StateMachine = parent.StateMachine;
            Layout = new Layout()
                .Contract("screen", 118, 40)
                .SplitHorizontal("screen", "title", "bottom", 0.12f, 2);
            Panels = new List<Panel> { Layout.CreatePanel("title"), Layout.CreatePanel("bottom") };
        }

        public override void Enter(EnterArgs enterParams = null) {
            var actorArgs = enterParams as StatusArgs;
            Debug.Assert(actorArgs != null);

            Actor = actorArgs.Actor;
            ActorSummary = new ActorSummary(Actor, new ActorSummaryArgs { ShowXP = true });

            EquipmentMenu = new Selection<int>(System.Renderer, new SelectionArgs<int>(Actor.ActiveEquipSlots) {
                Columns = 1,
                Rows = Actor.ActiveEquipSlots.Length,
                SpacingY = 26,
                RenderItem = Actor.RenderEquipment
            });
            EquipmentMenu.HideCursor();

            Actions = new Selection<string>(System.Renderer,
                new SelectionArgs<string>(Actor.Actions.ToList()) {
                    Columns = 1,
                    Rows = Actor.Actions.Count,
                    SpacingY = 18,
                    RenderItem = (renderer, x, y, item) => {
                        var label = Actor.ActionLabels[item];
                        Selection<string>.RenderItemFunc(Actions, renderer, x, y, label);
                    }
                });
            Actions.HideCursor();
        }

        public override void Exit() {
        }

        public override void HandleInput(float dt) {
            if (System.Keys.WasPressed(Keys.Back) || System.Keys.WasPressed(Keys.Escape)) {
                StateMachine.Change("frontmenu");
            }
        }

        public void DrawStat(Renderer renderer, int x, int y, string label, float value) {
            renderer.AlignText(TextAlignment.Right, TextAlignment.Center);
            renderer.DrawText2D(x - 5, y, label);
            renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
            renderer.DrawText2D(x + 5, y, value.ToString());
        }

        public override void Render(Renderer renderer) {
            foreach (var panel in Panels) {
                panel.Render(renderer);
            }

            var titleX = Layout.CenterX("title");
            var titleY = Layout.CenterY("title");
            renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            renderer.DrawText2D(titleX, titleY, "Status", Color.White, 1.5f);

            var left = Layout.Left("bottom") + 10;
            var top = Layout.Top("bottom") - 10;
            ActorSummary.SetPosition(left, top);
            ActorSummary.Render(renderer);

            renderer.AlignText(TextAlignment.Left, TextAlignment.Top);
            var xpStr = $"XP: {Actor.XP}/{Actor.NextLevelXP}";
            renderer.DrawText2D(left+240, top-58, xpStr);

            EquipmentMenu.Position = new Vector2(-10, -64);
            EquipmentMenu.Render(renderer);

            var stats = Actor.Stats;

            var x = left + 106;
            var y = 0;
            for (var index = 0; index < Actor.ActorStats.Count; index++) {
                var actorStat = Actor.ActorStats[index];
                var label = Actor.ActorStatLabels[index];
                DrawStat(renderer, x, y, label, stats.GetStat(actorStat).Value);
                y -= 16;
            }
            y -= 16;
            for (var index = 0; index < Actor.ItemStats.Count; index++) {
                var itemStat = Actor.ItemStats[index];
                var label = Actor.ItemStatLabels[index];
                DrawStat(renderer, x, y, label, stats.GetStat(itemStat).Value);
                y -= 16;
            }
            renderer.AlignText(TextAlignment.Left, TextAlignment.Top);
            x = 75;
            y = 25;
            var w = 100;
            var h = 56;
            var box = new Textbox(new TextboxParams {
                Text = new List<string> { ""},
                TextScale = 1.5f,
                Size = new Rectangle(x,y-h, w, h),
                TextBounds = new Vector4(10, -10, -10, 10),
                PanelArgs = new PanelParams() {
                    Texture = System.Content.FindTexture("simple_panel.png"),
                    Size = 3
                }
            });
            box.AppearTween = new Tween(1, 1, 0);
            box.Render(renderer);
            Actions.Position = new Vector2(x-14, y-10);
            Actions.Render(renderer);
        }
    }
}