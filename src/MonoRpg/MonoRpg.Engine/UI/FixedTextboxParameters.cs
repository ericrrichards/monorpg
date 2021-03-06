namespace MonoRpg.Engine.UI {
    using global::System;

    using Microsoft.Xna.Framework.Graphics;

    public class FixedTextboxParameters {
        public Texture2D Avatar { get; set; }
        public string Title { get; set; }

        public SelectionArgs<string> Choices { get; set; }
        public float TextScale { get; set; }
        public Action OnFinish { get; set; }
        public float TitlePadY { get; set; }

        public FixedTextboxParameters() {
            TextScale = 1.0f;
            TitlePadY = 10;
        }
    }
}