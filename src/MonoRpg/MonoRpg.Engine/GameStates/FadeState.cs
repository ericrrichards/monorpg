namespace MonoRpg.Engine.GameStates {
    using Microsoft.Xna.Framework;

    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.UI;

    public class FadeState : BaseStateObject {
        public float Duration { get; set; }
        public float AlphaFinish { get; set; }
        public float AlphaStart { get; set; }
        public Color Color { get; set; }
        public Tween Tween { get; private set; }

        public FadeState(StateStack stack, FadeArgs args=null): base(stack) {
            args = args ?? new FadeArgs();
            AlphaStart = args.AlphaStart;
            AlphaFinish = args.AlphaFinish;
            Duration = args.Duration;
            Color = args.Color;
            Tween = new Tween(AlphaStart, AlphaFinish, Duration);
        }

        
        public override bool Update(float dt) {
            Tween.Update(dt);
            var alpha = Tween.Value;
            Color = new Color(Color, alpha);

            if (Tween.Finished) {
                Stack.Pop();
            }
            return true;
        }

        public override void Render(Renderer renderer) {
            renderer.DrawRect2D(
                -System.ScreenWidth / 2, 
                System.ScreenHeight / 2, 
                System.ScreenWidth / 2, 
                -System.ScreenHeight / 2, 
                Color
            );
        }
    }

    public class FadeArgs {
        public float AlphaStart { get; set; }
        public float AlphaFinish { get; set; }
        public float Duration { get; set; }
        public Color Color { get; set; }    

        public FadeArgs() {
            AlphaStart = 1f;
            AlphaFinish = 0f;
            Duration = 1f;
            Color = new Color(0,0,0, AlphaStart);
        }
    }
}