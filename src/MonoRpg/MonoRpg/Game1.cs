using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace MonoRpg {
    using System;
    using System.Collections.Generic;
    using System.IO;

    using MonoRpg.Engine.Tiled;

    using Newtonsoft.Json;

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
            EntityDefs.Load("Content/entityDefs.json");
            
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

            

            var mapDef = _content.LoadMap("Content/small_room.json");

            mapDef.OnWake = new List<MapAction> {
                new MapAction {
                    ID = "AddNPC",
                    Params = new AddNPCParams{
                        Character = "strolling_npc",
                        X = 11,
                        Y = 5
                    }
                },
                new MapAction {
                    ID="AddNPC",
                    Params = new AddNPCParams{
                        Character = "strolling_npc",
                        X = 4,
                        Y = 5
                    }
                }
            };
            mapDef.Actions = new Dictionary<string, MapAction> {
                {"tele_south", new MapAction{ ID = "Teleport", Params = new TeleportParams{X =11, Y=3}} },
                {"tele_north", new MapAction{ID = "Teleport", Params = new TeleportParams{X=10, Y = 11}} }
            };
            mapDef.TriggerTypes = new Dictionary<string, TriggerTypeDef> {
                {"north_door_trigger", new TriggerTypeDef{OnEnter = "tele_north"} },
                {"south_door_trigger", new TriggerTypeDef{OnEnter = "tele_south"} },

            };
            mapDef.Triggers = new List<TriggerDef> {
                new TriggerDef { Trigger = "north_door_trigger", X = 11, Y = 2 },
                new TriggerDef { Trigger = "south_door_trigger", X = 10, Y = 12 }
            };


            _map = new Map(mapDef);
            _map.GotoTile(5, 5);

            _hero = new Character(EntityDefs.Instance.Characters["hero"], _map);




            
            _hero.Entity.SetTilePosition(11, 3, 0, _map);
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
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _hero.Controller.Update(dt);
            foreach (var npc in _map.NPCs) {
                npc.Controller.Update(dt);
            }

            if (ks.IsKeyDown(Keys.Space)) {
                var (x, y) = _hero.GetFacedTileCoords();
                var trigger = _map.GetTrigger(_hero.Entity.Layer, x, y);
                trigger?.OnUse(_hero.Entity);
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
                Entity heroEntity = null;
                if (i == _hero.Entity.Layer) {
                    heroEntity = _hero.Entity;
                }
                _map.RenderLayer(_renderer, i, heroEntity);
                
            }

            _renderer.Render();

            base.Draw(gameTime);
        }

        private void Teleport(Entity entity, Map map) {
            var pos = map.GetTileFoot(entity.TileX, entity.TileY);
            entity.Sprite.Position = new Vector2(pos.X, pos.Y + entity.Height / 2);
        }
    }
}
