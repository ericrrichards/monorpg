using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Arena {
    using MonoRpg.Engine;
    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.UI;

    using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Arena : Game {
        GraphicsDeviceManager _graphics;
        private Renderer Renderer { get; set; }
        private Content _content;
        private StateStack _stack;

        public Arena() {
            _graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 640,
                PreferredBackBufferHeight = 360
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
            _content = MonoRpg.Engine.Content.Create(Content, GraphicsDevice);
            Renderer = Renderer.Create(GraphicsDevice, _content);
            System.Exit = Exit;

            Icons.Instance = new Icons(_content.FindTexture("inventory_icons.png"));

            _stack = new StateStack();
            ItemDB.Initialize("Content/items.json");

            EntityDefs.Load("Content/entityDefs.json");

            MapDB.AddMap("arena.json");

            _stack.Push(new ExploreState(_stack, MapDB.Maps["arena.json"](), new Vector3(30,18,0)));
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
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            System.Keys.Update();

            _stack.Update(dt);
            World.Instance.Update(dt);


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
