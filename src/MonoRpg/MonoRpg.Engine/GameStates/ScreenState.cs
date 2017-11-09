namespace MonoRpg.Engine.GameStates {
    using Microsoft.Xna.Framework;

    using MonoRpg.Engine.RenderEngine;

    public class ScreenState : BaseStateObject {
        public Color Color { get; set; }
        public ScreenState(Color? color=null): base(null) {
            Color = color ?? Color.Black;
        }

        public override bool Update(float dt) {
            return true;
        }

        public override void Render(Renderer renderer) {
            renderer.DrawRect2D(System.Screen.Bounds, Color);
        }
        
    }
}