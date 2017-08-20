using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace MonoRpg {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        private readonly GraphicsDeviceManager _graphics;
        private Renderer _renderer;
        private Content _content;

        private Texture2D[] _textures;
        private Sprite _sprite;

        private int _left, _top;
        private int _tilesPerRow, _tilesPerColumn;
        private int _tileWidth, _tileHeight;

        private int[] _map = new[] {
            0,0,0,0,4,5,6,0,
            0,0,0,0,4,5,6,0,
            0,0,0,0,4,5,6,0,
            2,2,2,2,10,5,6,0,
            8,8,8,8,8,8,9,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,1,2,
        };
        private int _mapWidth = 8;
        private int _mapHeight = 7;
        private Texture2D _textureAtlas;
        private List<Rectangle> _uvs;

        private int GetTile(int[] map, int rowSize, int x, int y) {
            return map[x + y * rowSize];
        }

        public Game1() {
            _graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 256,
                PreferredBackBufferHeight = 224
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            _content = new Content(Content, GraphicsDevice);
            _renderer = new Renderer(GraphicsDevice, _content);
            _renderer.SetTextAlignment(TextAlignment.Center, TextAlignment.Center);

            _textures = new[] {
                _content.FindTexture("Content/tiles_00.png"),
                _content.FindTexture("Content/tiles_01.png"),
                _content.FindTexture("Content/tiles_02.png"),
                _content.FindTexture("Content/tiles_03.png"),
                _content.FindTexture("Content/tiles_04.png"),
                _content.FindTexture("Content/tiles_05.png"),
                _content.FindTexture("Content/tiles_06.png"),
                _content.FindTexture("Content/tiles_07.png"),
                _content.FindTexture("Content/tiles_08.png"),
                _content.FindTexture("Content/tiles_09.png"),
                _content.FindTexture("Content/tiles_10.png")
            };
            _tileWidth = _textures[0].Width;
            _tileHeight = _textures[0].Height;

            _textureAtlas = _content.FindTexture("Content/atlas.png");
            _uvs = GenerateUVs(_textureAtlas, _tileWidth);

            _sprite = new Sprite();
            _sprite.Texture = _textureAtlas;

            _left = -_graphics.PreferredBackBufferWidth / 2 + _tileWidth / 2;
            _top = _graphics.PreferredBackBufferHeight / 2 - _tileHeight / 2;

            _tilesPerRow = (int)Math.Ceiling(_graphics.PreferredBackBufferWidth / (double)_tileWidth);
            _tilesPerColumn = (int)Math.Ceiling((_graphics.PreferredBackBufferHeight / (double)_tileHeight));

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {

            for (int j = 0; j < _mapHeight; j++) {


                for (int i = 0; i < _mapWidth; i++) {
                    var tile = GetTile(_map, _mapWidth, i, j);
                    _sprite.SetUVs(_uvs[tile]);


                    _sprite.Position = new Vector2(_left + i * _tileWidth, _top - j * _tileHeight);
                    _renderer.DrawSprite(_sprite);
                }
            }


            _renderer.Render();
            base.Draw(gameTime);
        }

        public List<Rectangle> GenerateUVs(Texture2D texture, int tileSize) { return GenerateUVs(texture, tileSize, tileSize); }
        public List<Rectangle> GenerateUVs(Texture2D texture, int tileWidth, int tileHeight) {
            var uvs = new List<Rectangle>();

            float texWidth = texture.Width;
            float texHeight = texture.Height;
            var cols = texWidth / tileWidth;
            var rows = texHeight / tileHeight;

            var u0 = 0;
            var v0 = 0;
            for (var j = 0; j < rows; j++) {
                for (var i = 0; i < cols; i++) {
                    uvs.Add(new Rectangle(u0, v0, tileWidth, tileHeight));
                    u0 += tileWidth;
                }
                u0 = 0;
                v0 += tileHeight;
            }


            return uvs;
        }
    }
}
