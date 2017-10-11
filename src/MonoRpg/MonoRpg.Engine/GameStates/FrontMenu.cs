namespace MonoRpg.Engine.GameStates {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

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
        public Selection<ActorSummary> PartyMenu { get; set; }
        public string PrevTopBarText { get; set; }
        public bool InPartyMenu { get; set; }

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
                                               new SelectionArgs<string>("Items", "Status") {
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

            PartyMenu = new Selection<ActorSummary>(System.Renderer, new SelectionArgs<ActorSummary>(CreatePartySummaries()) {
                SpacingY =  90,
                Columns = 1,
                Rows = 3,
                OnSelection = OnPartyMemberChosen,
                RenderItem = (renderer, x, y, item) => {
                    item.SetPosition(x, y + 35);
                    item.Render(renderer);
                }
                
            });
            PartyMenu.HideCursor();
            TopBarText = "Empty Room";
            InPartyMenu = false;
        }

        private void OnMenuClick(int index) {
            var items = 0;
            if (index == items) {
                StateMachine.Change("items");
                return;
            }
            InPartyMenu = true;
            Selections.HideCursor();
            PartyMenu.ShowCursor();
            PrevTopBarText = TopBarText;
            TopBarText = "Chose a party member";
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

            var partyX = Layout.Left("party") - 16;
            var partyY = Layout.Top("party") - 45;
            PartyMenu.Position = new Vector2(partyX, partyY);
            PartyMenu.Render(renderer);

        }

        public override void HandleInput(float dt) {
            if (InPartyMenu) {
                PartyMenu.HandleInput();

                if (System.Keys.WasPressed(Keys.Back) || System.Keys.WasPressed(Keys.Escape)) {
                    InPartyMenu = false;
                    TopBarText = PrevTopBarText;
                    Selections.ShowCursor();
                    PartyMenu.HideCursor();
                }
            } else {
                Selections.HandleInput();

                if (System.Keys.WasPressed(Keys.Back) || System.Keys.WasPressed(Keys.Escape)) {
                    Stack.Pop();
                }
            }
            
        }

        public List<ActorSummary> CreatePartySummaries() {
            var members = World.Instance.Party.Members;
            return members.Select(kv => new ActorSummary(kv.Value, new ActorSummaryArgs { ShowXP = true })).ToList();
        }

        private void OnPartyMemberChosen(int actorIndex, ActorSummary actorSummary) {
            var indexToStateId = new Dictionary<int, string> {
                [1] = "status"
            };
            var actor = actorSummary.Actor;
            var index = Selections.GetIndex();
            var stateId = indexToStateId[index];
            StateMachine.Change(stateId, new StatusArgs{Actor = actor});
        }
    }

    public class ActorSummary  {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public Actor Actor { get; set; }
        public ProgressBar HPBar { get; set; }
        public ProgressBar MPBar { get; set; }
        public int AvatarTextPad { get; set; }
        public int LabelRightPad { get; set; }
        public int LabelValuePad { get; set; }
        public int VerticalPad { get; set; }
        public bool ShowXP { get; set; }
        public ProgressBar XPBar { get; set; }

        public ActorSummary(Actor actor, ActorSummaryArgs args) {
            X = 0;
            Y = 0;
            Width = 340;
            Actor = actor;
            HPBar = new ProgressBar(new ProgressBarArgs {
              Value  = actor.Stats.GetStat(Stats.HitPoints).Value,
              Maximum = actor.Stats.GetStat(Stats.MaxHitPoints).Value,
              Background = System.Content.FindTexture("hpbackground.png"),
              Foreground = System.Content.FindTexture("hpforeground.png")
            });
            HPBar.Foreground.PixelArt = true;
            HPBar.Background.PixelArt = true;
            MPBar = new ProgressBar(new ProgressBarArgs {
                Value = actor.Stats.GetStat(Stats.MagicPoints).Value,
                Maximum = actor.Stats.GetStat(Stats.MaxMagicPoints).Value,
                Background = System.Content.FindTexture("mpbackground.png"),
                Foreground = System.Content.FindTexture("mpforeground.png")
            });
            MPBar.Foreground.PixelArt = true;
            MPBar.Background.PixelArt = true;
            AvatarTextPad = 14;
            LabelRightPad = 15;
            LabelValuePad = 8;
            VerticalPad = 18;
            ShowXP = args.ShowXP;

            if (ShowXP) {
                XPBar = new ProgressBar(new ProgressBarArgs {
                    Value = actor.XP,
                    Maximum = actor.NextLevelXP,
                    Background = System.Content.FindTexture("xpbackground.png"),
                    Foreground = System.Content.FindTexture("xpforeground.png")
                });
            }
            SetPosition(X, Y);
        }

        public void SetPosition(int x, int y) {
            X = x;
            Y = y;

            int barX;
            if (ShowXP) {
                var boxRight = X + Width;
                barX = boxRight - XPBar.HalfWidth;
                var barY = Y - 44;
                XPBar.Position = new Vector2(barX, barY);
            }
            var avatarW = Actor.PortraitTexture.Width;
            barX = X + avatarW + AvatarTextPad;
            barX += LabelRightPad + LabelValuePad;
            barX += MPBar.HalfWidth;

            MPBar.Position = new Vector2(barX, Y - 72);
            HPBar.Position = new Vector2(barX, Y - 54);

        }

        public Vector2 GetCursorPosition() {
            return new Vector2(X, Y - 40);
        }

        public void Render(Renderer renderer) {
            var avatarW = Actor.PortraitTexture.Width;
            var avatarH = Actor.PortraitTexture.Height;
            var avatarX = X + avatarW / 2;
            var avatarY = Y - avatarH / 2;
            Actor.Portrait.Position = new Vector2(avatarX, avatarY);
            renderer.DrawSprite(Actor.Portrait);


            renderer.AlignText(TextAlignment.Left, TextAlignment.Top);

            var textPadY = 2;
            var textX = avatarX + avatarW / 2 + AvatarTextPad;
            var textY = Y - textPadY;
            renderer.DrawText2D(textX, textY, Actor.Name, Color.White, 1.2f );

            renderer.AlignText(TextAlignment.Right, TextAlignment.Top);
            textX += LabelRightPad;
            textY -= 20;
            var startStatsY = textY;
            var scale = 1.0f;
            renderer.DrawText2D(textX, textY, "LV", Color.White, scale);
            textY -= VerticalPad;
            renderer.DrawText2D(textX, textY, "HP", Color.White, scale);
            textY -= VerticalPad;
            renderer.DrawText2D(textX, textY, "MP", Color.White, scale);
            textY = startStatsY;
            textX += LabelValuePad;
            renderer.AlignText(TextAlignment.Left, TextAlignment.Top);
            var level = Actor.Level;
            var hp = Actor.Stats.GetStat(Stats.HitPoints).Value;
            var maxHp = Actor.Stats.GetStat(Stats.MaxHitPoints).Value;
            var mp = Actor.Stats.GetStat(Stats.MaxMagicPoints).Value;
            var maxMp = Actor.Stats.GetStat(Stats.MaxMagicPoints).Value;

            var counter = "{0}/{1}";
            renderer.DrawText2D(textX, textY, level.ToString(), Color.White, scale);
            textY -= VerticalPad;
            renderer.DrawText2D(textX, textY, string.Format(counter, hp, maxHp), Color.White, scale);
            textY -= VerticalPad;
            renderer.DrawText2D(textX, textY, string.Format(counter, mp, maxMp), Color.White, scale);

            if (ShowXP) {
                renderer.AlignText(TextAlignment.Left, TextAlignment.Top);
                var boxRight = X + Width;
                textY = startStatsY;
                var left = boxRight - XPBar.HalfWidth*2;
                renderer.DrawText2D(left, textY, "Next Level", Color.White, scale);
                XPBar.Render(renderer);
            }
            HPBar.Render(renderer);
            MPBar.Render(renderer);
        }

    }

    public class ActorSummaryArgs {
        public bool ShowXP { get; set; }
    }

    public class StatusArgs : EnterArgs {
        public Actor Actor { get; set; }
    }
}