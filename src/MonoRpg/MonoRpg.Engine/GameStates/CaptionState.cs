namespace MonoRpg.Engine.GameStates {
    using MonoRpg.Engine.RenderEngine;

    public class CaptionState : BaseStateObject {
        public CaptionStyle Style { get; set; }
        public string Text { get; set; }

        public CaptionState(CaptionStyle style, string text): base(null) {
            Style = style;
            Text = text;
        }
        

        public override bool Update(float dt) {
            return true;
        }

        public override void Render(Renderer renderer) {
            Style.Render(Style, renderer, Text);
        }
    }
}