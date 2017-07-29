namespace MonoRpg.Engine {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mime;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Content {
        private const string DEFAULT_FONT = "default";

        private readonly ContentManager _manager;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();
        private readonly Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
       

        public SpriteFont DefaultFont => Fonts[DEFAULT_FONT];

        public Content(ContentManager manager, GraphicsDevice device) {
            _manager = manager;
            _graphicsDevice = device;
            Fonts[DEFAULT_FONT] = _manager.Load<SpriteFont>("default");
        }


        public Texture2D FindTexture(string texture) {
            if (Textures.ContainsKey(texture)) {
                return Textures[texture];
            }
            try {
                var tex = _manager.Load<Texture2D>(texture);
                Textures[texture] = tex;
                return tex;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            using (var stream = new FileStream(texture, FileMode.Open)) {
                var tex = Texture2D.FromStream(_graphicsDevice, stream);
                Textures[texture] = tex;
                return tex;
            }
        }
        
    }
}