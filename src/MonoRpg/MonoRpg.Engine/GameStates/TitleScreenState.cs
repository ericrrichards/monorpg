namespace MonoRpg.Engine.GameStates {
    using Microsoft.Xna.Framework;

    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.UI;

    public class TitleScreenState : BaseStateObject {
        public Selection<string> Menu { get; set; }

        public Sprite TitleBanner { get; set; }
        public Storyboard Storyboard { get; private set; }
        public TitleScreenState(StateStack stack, Storyboard storyboard): base(stack) {
            TitleBanner = new Sprite { Texture = System.Content.FindTexture("title_screen.png"), Position = new Vector2(0, 100) };
            
            Storyboard = storyboard;
            Menu = new Selection<string>(System.Renderer, 
                                         new SelectionArgs<string>("Play", "Exit") {
                                             SpacingY = 32,
                                             OnSelection = OnSelection
                                         }) {
                CursorWidth = 25
            };
            Menu.X = Menu.GetWidth() / 2 - Menu.CursorWidth*2;
            Menu.Y = -50;

        }
        
        public override void HandleInput(float dt) {
            Menu.HandleInput();
        }

        private void OnSelection(int index, string item) {
            if (index == 0) {
                Stack.Pop();
                Stack.Push(Storyboard);
            } else {
                System.Exit();
            }
        }

        public override void Render(Renderer renderer) {
            renderer.DrawRect2D(System.Screen.Bounds, Color.Blue);

            renderer.DrawSprite(TitleBanner);
            renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            renderer.DrawText2D(0, 60, "A mini-rpg adventure", Color.White, 0.8f);
            renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
            Menu.Render(renderer);
        }
    }
}