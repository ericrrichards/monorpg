using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace MonoRpg {
    using System;
    using System.Collections.Generic;

    using System = MonoRpg.Engine.System;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        private readonly GraphicsDeviceManager _graphics;
        private Renderer _renderer;
        private Content _content;
        

        private Map _map;
        private Character _hero;

        public Game1() {
            _graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600
            };
            System.Init(_graphics);
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
            System.Content = _content;
            _renderer = new Renderer(GraphicsDevice, _content);
            _renderer.SetTextAlignment(TextAlignment.Center, TextAlignment.Center);
            
            _map = new Map(_content.LoadMap("Content/small_room.json"));
            _map.GotoTile(5,5);
            // TODO: use this.Content to load your game content here

            var heroDef = new EntityDef {
                Texture = "walk_cycle.png", 
                Width = 16,
                Height = 24,
                StartFrame = 8,
                TileX = 10,
                TileY = 2
            };
            var frames = new List<List<int>> {
                new List<int>{0, 1,2,3},
                new List<int>{4,5,6,7},
                new List<int>{8,9,10,11},
                new List<int>{12,13,14,15}
            };

            _hero = new Character(new Entity(heroDef), _map, frames);
            _hero.Controller.Change("wait");



            Teleport(_hero.Entity, _map);
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
            var ks = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || ks.IsKeyDown(Keys.Escape))
                Exit();

            _renderer.Translate(-_map.CamX, -_map.CamY);
            _hero.Controller.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (ks.IsKeyDown(Keys.Space)) {
                _map.GotoTile(0, 0);
            }
            var playerPos = _hero.Entity.Sprite.Position;
            _map.CamX = (int)Math.Floor(playerPos.X);
            _map.CamY = (int)Math.Floor(playerPos.Y);
                

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {

            _map.Render(_renderer);
            _renderer.DrawSprite(_hero.Entity.Sprite);
            _renderer.Render();
            
            base.Draw(gameTime);
        }
        
        private void Teleport(Entity entity, Map map) {
            var pos = map.GetTileFoot(entity.TileX, entity.TileY);
            entity.Sprite.Position = new Vector2(pos.X, pos.Y + entity.Height/2);
        }
    }
}
