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

        public Textbox(TextboxParams parameters) {
            parameters = parameters ?? new TextboxParams();


            Text = parameters.Text;
            TextScale = parameters.TextScale;
            Panel = new Panel(parameters.PanelArgs){PixelArt = false};
            Size = parameters.Size;
            Bounds = parameters.TextBounds;

            X = (Size.Right + Size.Left) / 2;
            Y = (Size.Top + Size.Bottom) / 2;
            Width = Math.Abs(Size.Right - Size.Left);
            Height = Math.Abs(Size.Top - Size.Bottom);
        }

        public void Render(Renderer renderer) {
            var scale = 1f;
            renderer.ScaleText(TextScale * scale);
            renderer.SetTextAlignment(TextAlignment.Left, TextAlignment.Top);

            Panel.CenterPosition(X, Y, (int)(Width*scale), (int)(Height* scale));
            Panel.Render(renderer);

            var left = X - (Width / 2f * scale);
            var textLeft = left + (Bounds.X * scale);
            var top = Y + Height / 2f * scale;
            var textTop = top + Bounds.Y * scale;

            renderer.DrawText2D((int)textLeft, (int)textTop, Text, Color.White);
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