namespace MonoRpg.Engine {
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Content {
        private const string DEFAULT_FONT = "default";

        private readonly ContentManager _manager;
        private readonly Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();
        public SpriteFont DefaultFont => Fonts[DEFAULT_FONT];

        public Content(ContentManager manager) {
            _manager = manager;
            Fonts[DEFAULT_FONT] = _manager.Load<SpriteFont>("default");
        }

        
    }
}