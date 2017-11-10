using System;
using System.Collections.Generic;
using System.IO;
namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
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
        private readonly Dictionary<string, SoundEffect> SoundEffects = new Dictionary<string, SoundEffect>();
        private SpriteFont _defaultFont;

        public SpriteFont DefaultFont => Fonts[DEFAULT_FONT];
        public Texture2D DummyTexture { get; }

        private Content(ContentManager manager, GraphicsDevice device) {
            _manager = manager;
            _graphicsDevice = device;
            Fonts[DEFAULT_FONT] = _manager.Load<SpriteFont>("default");
            _defaultFont = Fonts[DEFAULT_FONT];
            
            DummyTexture = new Texture2D(System.Device, 1, 1);
            DummyTexture.SetData(new[] { Color.White });
        }

        public static Content Create(ContentManager manager, GraphicsDevice device) {
            System.Content = new Content(manager, device);
            return System.Content;
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

        public SoundEffect GetSound(string soundFile) {
            if (SoundEffects.ContainsKey(soundFile)) {
                return SoundEffects[soundFile];
            }
            if (!File.Exists(soundFile) && !soundFile.Contains(_manager.RootDirectory)) {
                return GetSound(Path.Combine(_manager.RootDirectory, "Sounds", soundFile));
            }
            try {
                var sound = _manager.Load<SoundEffect>(soundFile);
                SoundEffects[soundFile] = sound;
                return sound;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            using (var stream = new FileStream(soundFile, FileMode.Open)) {
                var sound = SoundEffect.FromStream(stream);
                SoundEffects[soundFile] = sound;
                return sound;
            }
        }

        public static TiledMap LoadMap(string mapFile) { return new TiledMap(TiledImporter.Map.LoadFromFile(mapFile)); }
        
    }
}