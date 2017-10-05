namespace MonoRpg.Engine.UI {
    using global::System.CodeDom;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using MonoRpg.Engine.RenderEngine;

    public class ProgressBar {
        public int X { get; set; }
        public int Y { get; set; }
        public Sprite Background { get; set; }
        public Sprite Foreground { get; set; }
        public float Value { get; set; }
        public float Maximum { get; set; }
        public int HalfWidth { get; set; }

        public ProgressBar(ProgressBarArgs args) {
            X = args.X;
            Y = args.Y;
            Background = new Sprite() {
                Texture = args.Background
            };
            Foreground = new Sprite() {
                Texture = args.Foreground
            };
            Value = args.Value;
            Maximum = args.Maximum;

            HalfWidth = args.Foreground.Width / 2;
            SetValue(Value);

        }

        public void SetValue(float value, int? max = null) {
            Maximum = max ?? Maximum;
            SetNormalValue(value / Maximum);
        }

        private void SetNormalValue(float value) {
            Foreground.SetUVs(new Rectangle(0,0, (int)(Foreground.Width*value), Foreground.Height));
            var position = new Vector2(X - HalfWidth * (1f-value), Y);
            Foreground.Position = position;
        }
        

        public Vector2 Position {
            get {
                return new Vector2(X,Y);
            }
            set {
                X = (int)value.X;
                Y = (int)value.Y;
                var position = new Vector2(X, Y);
                Background.Position = position;
                Foreground.Position = position;
                SetValue(Value);
            }
        }

        public void Render(Renderer renderer) {
            renderer.DrawSprite(Background);
            renderer.DrawSprite(Foreground);
        }
    }

    public class ProgressBarArgs {
        public ProgressBarArgs() {
            Maximum = 1f;
            
        }
        public Texture2D Foreground { get; set; }
        public Texture2D Background { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public float Value { get; set; }
        public float Maximum { get; set; }
    }
}