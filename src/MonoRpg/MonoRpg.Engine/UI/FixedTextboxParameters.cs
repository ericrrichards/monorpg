namespace MonoRpg.Engine.UI {
    using Microsoft.Xna.Framework.Graphics;

    public class FixedTextboxParameters {
        public Texture2D Avatar { get; set; }
        public string Title { get; set; }

        public SelectionArgs Choices { get; set; }
    }
}