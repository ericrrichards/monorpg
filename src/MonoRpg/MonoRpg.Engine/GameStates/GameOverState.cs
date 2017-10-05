namespace MonoRpg.Engine.GameStates {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.UI;

    public class GameOverState : BaseStateObject {
        public CaptionStyle Subtitle { get; set; }
        public CaptionStyle Title { get; set; }

        public GameOverState(StateStack stack) : base(stack) {
            Title = new CaptionStyle(CaptionStyle.Styles["title"]);
            Title.Color = new Color(Title.Color, 1f);
            Subtitle = new CaptionStyle(CaptionStyle.Styles["subtitle"]);
            Subtitle.Color = new Color(Subtitle.Color, 1f);
        }
        

        public override void HandleInput(float dt) {
            if (System.Keys.WasPressed(Keys.Space)) {
                System.Exit();
            }
        }

        public override void Render(Renderer renderer) {
            Title.Render(Title, renderer, "Game Over");
            Subtitle.Render(Subtitle, renderer, "Want to find out what happens next? Write it!");

        }
    }
}