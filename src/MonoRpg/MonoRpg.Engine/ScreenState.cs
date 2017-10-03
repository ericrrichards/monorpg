namespace MonoRpg.Engine {
    using global::System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.UI;

    public class ScreenState : IStateObject {
        public Color Color { get; set; }
        public ScreenState(Color? color=null) {
            Color = color ?? Color.Black;
        }

        public void Enter(EnterArgs enterParams = null) {
        }
        public void Exit() {
        }
        public void HandleInput(float dt) {
        }

        public bool Update(float dt) {
            return true;
        }

        public void Render(Renderer renderer) {
            renderer.DrawRect2D(-System.ScreenWidth/2, System.ScreenHeight-2, System.ScreenWidth/2, -System.ScreenHeight/2, Color);
        }
        
    }

    public class CaptionState : IStateObject {
        public CaptionStyle Style { get; set; }
        public string Text { get; set; }

        public CaptionState(CaptionStyle style, string text) {
            Style = style;
            Text = text;
        }

        public void Enter(EnterArgs enterParams = null) {}
        public void Exit() {}
        public void HandleInput(float dt) {}

        public bool Update(float dt) {
            return true;
        }

        public void Render(Renderer renderer) {
            Style.Render(Style, renderer, Text);
        }
    }

    public class GameOverState : IStateObject {
        public StateStack Stack { get; set; }
        public CaptionStyle Subtitle { get; set; }
        public CaptionStyle Title { get; set; }

        public GameOverState(StateStack stack) {
            Stack = stack;
            Title = new CaptionStyle(CaptionStyle.Styles["title"]);
            Title.Color = new Color(Title.Color, 1f);
            Subtitle = new CaptionStyle(CaptionStyle.Styles["subtitle"]);
            Subtitle.Color = new Color(Subtitle.Color, 1f);
        }

        public void Enter(EnterArgs enterParams = null) {
            
        }

        public void Exit() {
            if (System.Keys.WasPressed(Keys.Space)) {
                System.Exit();
            }
        }

        public void HandleInput(float dt) {
        }

        public bool Update(float dt) {
            return false;
        }

        public void Render(Renderer renderer) {
            Title.Render(Title, renderer, "Game Over");
            Subtitle.Render(Subtitle, renderer, "Want to find out what happens next? Write it!");

        }
    }

    public class TitleScreenState : IStateObject {
        public Selection<string> Menu { get; set; }

        public Sprite TitleBanner { get; set; }
        public StateStack Stack { get; set; }
        public Storyboard Storyboard { get; private set; }
        public TitleScreenState(StateStack stack, Storyboard storyboard) {
            TitleBanner = new Sprite { Texture = System.Content.FindTexture("title_screen.png"), Position = new Vector2(0, 100) };

            Stack = stack;
            Storyboard = storyboard;
            Menu = new Selection<string>(System.Renderer, 
                new SelectionArgs<string>(new List<string>{"Play", "Exit"}) {
                    SpacingY = 32,
                    OnSelection = OnSelection
                }) {
                CursorWidth = 25
            };
            Menu.X = Menu.GetWidth() / 2 - Menu.CursorWidth*2;
            Menu.Y = -50;

        }

        public void Enter(EnterArgs enterParams = null) {
        }

        public void Exit() {
        }

        public void HandleInput(float dt) {
            Menu.HandleInput();
        }

        public bool Update(float dt) {
            return false;
        }

        private void OnSelection(int index, string item) {
            if (index == 0) {
                Stack.Pop();
                Stack.Push(Storyboard);
            } else {
                System.Exit();
            }
        }

        public void Render(Renderer renderer) {
            renderer.DrawRect2D(-System.ScreenWidth/2, -System.ScreenHeight/2, System.ScreenWidth/2, System.ScreenHeight/2, Color.Black);

            renderer.DrawSprite(TitleBanner);
            renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            renderer.DrawText2D(0, 60, "A mini-rpg adventure", Color.White, 0.8f);
            renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
            Menu.Render(renderer);
        }
    }
}