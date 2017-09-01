using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace MonoRpg {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Microsoft.Xna.Framework.Graphics;

    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.Tiled;
    using MonoRpg.Engine.UI;

    using Newtonsoft.Json;

    using System = MonoRpg.Engine.System;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        private readonly GraphicsDeviceManager _graphics;
        private Renderer Renderer { get; set; }
        private Content _content;
        private StateStack _stack;

        public Game1() {
            _graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 640,
                PreferredBackBufferHeight = 512
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
            Renderer = new Renderer(GraphicsDevice, _content);
            Renderer.SetTextAlignment(TextAlignment.Center, TextAlignment.Center);
            //_renderer.ClearColor = Color.White;


            var mapDef = _content.LoadMap("Content/small_room.json");

            mapDef.OnWake = new List<MapAction> {
            };
            mapDef.Actions = new Dictionary<string, MapAction> {
            };
            mapDef.TriggerTypes = new Dictionary<string, TriggerTypeDef> {
            };
            mapDef.Triggers = new List<TriggerDef> {
            };

            _stack = new StateStack();
            _stack.Push(new ExploreState(null, mapDef, new Vector3(11, 3, 0)));
            _stack.PushFit(Renderer, 0, 0, "You're trapped in a small room.");

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

            //_renderer.Translate(-_map.CamX, -_map.CamY);
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _stack.Update(dt);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            _stack.Render(Renderer);

            Renderer.Render();

            base.Draw(gameTime);
        }

        

    }
}
