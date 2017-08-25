namespace MonoRpg.Engine.UI {
    using global::System;

    using Microsoft.Xna.Framework;

    public class Textbox {
        public Vector4 Bounds { get; set; }
        public Rectangle Size { get; set; }
        public int TextScale { get; set; }
        public Panel Panel { get; private set; }
        public string Text { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Tween AppearTween { get; set; }
        public bool IsDead => AppearTween.Finished && AppearTween.Value == 0;

        public Textbox(TextboxParams parameters) {
            parameters = parameters ?? new TextboxParams();


            Text = parameters.Text;
            TextScale = parameters.TextScale;
            Panel = new Panel(parameters.PanelArgs){PixelArt = true};
            Size = parameters.Size;
            Bounds = parameters.TextBounds;

            X = (Size.Right + Size.Left) / 2;
            Y = (Size.Top + Size.Bottom) / 2;
            Width = Math.Abs(Size.Right - Size.Left);
            Height = Math.Abs(Size.Top - Size.Bottom);
            AppearTween = new Tween(0, 1, 0.4f, Tween.EaseOutCirc);
        }

        public void Update(float dt) {
            AppearTween.Update(dt);
        }

        public void OnClick() {
            if (!(AppearTween.Finished && AppearTween.Value == 1) ){
                return;
            }
            AppearTween = new Tween(1,0,1.2f, Tween.EaseInCirc);
        }

        public void Render(Renderer renderer) {
            var scale = AppearTween.Value;
            renderer.SetTextAlignment(TextAlignment.Left, TextAlignment.Top);

            Panel.CenterPosition(X, Y, (int)(Width*scale), (int)(Height* scale));
            Panel.Render(renderer);

            var left = X - (Width / 2f * scale);
            var textLeft = left + (Bounds.X * scale);
            var top = Y + Height / 2f * scale;
            var textTop = top + Bounds.Y * scale;

            renderer.DrawText2D((int)textLeft, (int)textTop, Text, Color.White, TextScale * scale);
        }

        
    }

    public class TextboxParams {
        public string Text { get; set; }
        public int TextScale { get; set; }
        public PanelParams PanelArgs { get; set; }
        public Rectangle Size { get; set; }
        public Vector4 TextBounds { get; set; }
    }
}