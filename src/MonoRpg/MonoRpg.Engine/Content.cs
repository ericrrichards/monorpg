using System;
using System.Collections.Generic;
using System.IO;
namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    using MonoRpg.Engine.Tiled;

    using Newtonsoft.Json;

    public class Content {
        private const string DEFAULT_FONT = "default";

        private readonly ContentManager _manager;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();
        private readonly Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private SpriteFont _defaultFont;

        public SpriteFont DefaultFont => Fonts[DEFAULT_FONT];
        public Texture2D DummyTexture { get; }

        public Content(ContentManager manager, GraphicsDevice device) {
            _manager = manager;
            _graphicsDevice = device;
            Fonts[DEFAULT_FONT] = _manager.Load<SpriteFont>("default");
            _defaultFont = Fonts[DEFAULT_FONT];
            
            DummyTexture = new Texture2D(System.Device, 1, 1);
            DummyTexture.SetData(new[] { Color.White });
        }

        public SpriteFont LoadFont(string fontName) {
            if (!Fonts.ContainsKey(fontName)) {
                Fonts[fontName] = _manager.Load<SpriteFont>(fontName);
            }
            return Fonts[fontName];
        }

        public void SetDefaultFont(string fontName) {
            if (!Fonts.ContainsKey(fontName)) {
                Fonts[DEFAULT_FONT] = LoadFont(fontName);
            }
        }

        public void ResetDefaultFont() {
            Fonts[DEFAULT_FONT] = _defaultFont;
        }


        public Texture2D FindTexture(string texture) {
            if (Textures.ContainsKey(texture)) {
                return Textures[texture];
            }
            if (!File.Exists(texture) && !texture.Contains(_manager.RootDirectory)) {
                return FindTexture(Path.Combine(_manager.RootDirectory, texture));
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

        public TiledMap LoadMap(string mapFile) { return JsonConvert.DeserializeObject<TiledMap>(File.ReadAllText(mapFile)); }
        
    }
}