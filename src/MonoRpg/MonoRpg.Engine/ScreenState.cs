namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;

    using MonoRpg.Engine.GameStates;

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
}