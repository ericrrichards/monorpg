namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;

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
}