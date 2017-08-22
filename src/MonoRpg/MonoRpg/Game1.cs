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
        private TeleportAction _upDoorTeleport;
        private TeleportAction _downDoorTeleport;
        private Trigger _triggerTop;
        private Trigger _triggerBot;
        private Trigger _potTrigger;

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
                TileX = 11,
                TileY = 3,
                Layer = 0
            };
            var frames = new List<List<int>> {
                new List<int>{0, 1,2,3},
                new List<int>{4,5,6,7},
                new List<int>{8,9,10,11},
                new List<int>{12,13,14,15}
            };

            _hero = new Character(new Entity(heroDef), _map, frames);
            _hero.Controller.Change("wait");



            


            _upDoorTeleport = new TeleportAction(_map, 11, 3);
            _upDoorTeleport.Execute(null, _hero.Entity);

            _downDoorTeleport = new TeleportAction(_map, 10, 11);


            _triggerTop = new Trigger(_downDoorTeleport);
            _triggerBot = new Trigger(_upDoorTeleport);
            _potTrigger = new Trigger(onUse:_downDoorTeleport);

            _map.Triggers[0].Add(_map.CoordToIndex(10, 12), _triggerBot);
            _map.Triggers[0].Add(_map.CoordToIndex(11, 2), _triggerTop);

            _map.Triggers[0].Add(_map.CoordToIndex(10, 3), _potTrigger);
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
                var (x, y) = _hero.GetFacedTileCoords();
                var trigger = _map.GetTrigger(_hero.Entity.Layer, x, y);
                trigger?.OnUse.Execute(trigger, _hero.Entity);
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
            var layerCount = _map.LayerCount;
            for (int i = 0; i < layerCount; i++) {
                _map.RenderLayer(_renderer, i);
                if (i == _hero.Entity.Layer) {
                    _renderer.DrawSprite(_hero.Entity.Sprite);
                }
            }
            
            _renderer.Render();
            
            base.Draw(gameTime);
        }
        
        private void Teleport(Entity entity, Map map) {
            var pos = map.GetTileFoot(entity.TileX, entity.TileY);
            entity.Sprite.Position = new Vector2(pos.X, pos.Y + entity.Height/2);
        }
    }
}
