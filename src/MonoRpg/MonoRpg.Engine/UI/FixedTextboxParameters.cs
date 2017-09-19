namespace MonoRpg.Engine.UI {
    using Microsoft.Xna.Framework.Graphics;

    public class FixedTextboxParameters {
        public Texture2D Avatar { get; set; }
        public string Title { get; set; }

        public SelectionArgs<string> Choices { get; set; }
        public float TextScale { get; set; }

        public FixedTextboxParameters() {
            TextScale = 1.0f;
        }
    }
}