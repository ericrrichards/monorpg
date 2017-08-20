using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace MonoRpg {
    using System;
    using System.Collections.Generic;

    using MonoRpg.Engine.Tiled;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        private readonly GraphicsDeviceManager _graphics;
        private Renderer _renderer;
        private Content _content;

        private Sprite _sprite;

        private int _left, _top;
        private int _tileWidth, _tileHeight;

        private List<int> _map = new List<int> {
            0,0,0,0,4,5,6,0,
            0,0,0,0,4,5,6,0,
            0,0,0,0,4,5,6,0,
            2,2,2,2,10,5,6,0,
            8,8,8,8,8,8,9,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,1,2,
        };
        private int _mapWidth;
        private int _mapHeight;
        private Texture2D _textureAtlas;
        private List<Rectangle> _uvs;
        private TiledMap _tiledMap;

        private int GetTile(IReadOnlyList<int> map, int rowSize, int x, int y) {
            return map[x + y * rowSize] - 1; // Tiled uses 1 as the first ID, instead of 0 like everything else in the world does.
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


            _tiledMap = _content.LoadMap("Content/example_map.json");

            _tileWidth = _tiledMap.TileSets[0].TileWidth;
            _tileHeight = _tiledMap.TileSets[0].TileHeight;
            _mapWidth = _tiledMap.Width;
            _mapHeight = _tiledMap.Height;
            _map = _tiledMap.Layers[0].Data;


            _textureAtlas = _content.FindTexture("Content/"  + _tiledMap.TileSets[0].Image);
            _uvs = GenerateUVs(_textureAtlas, _tileWidth);

            _sprite = new Sprite();
            _sprite.Texture = _textureAtlas;

            _left = -_graphics.PreferredBackBufferWidth / 2 + _tileWidth / 2;
            _top = _graphics.PreferredBackBufferHeight / 2 - _tileHeight / 2;


            


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
