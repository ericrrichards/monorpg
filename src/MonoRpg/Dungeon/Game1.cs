using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace Dungeon {
    using MonoRpg.Engine.UI;

    using System = MonoRpg.Engine.System;
    using static Events;
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager _graphics;
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
            System.Renderer = Renderer;
            Renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            Renderer.ClearColor = Color.CornflowerBlue;

            _content.SetDefaultFont("junction");
            _content.LoadFont("contra_italic");
            


            _stack = new StateStack();
            var storyboard = new Storyboard(_stack, 
                BlackScreen(), 
                Play("rain.wav"),
                NoBlock(
                    FadeSound("rain.wav", 0, 1, 3)
                ),
                Caption("place", "title", "Village of Sontos"),
                Caption("time", "subtitle", "MIDNIGHT"),
                Wait(2),
                NoBlock(
                    FadeOutCaption("place", 3)
                ),
                FadeOutCaption("time", 3),
                FadeSound("rain.wav", 1, 0, 3),
                KillState("place"),
                KillState("time"),
                FadeOutScreen(),
                Stop("rain.wav")
            );
            _stack.Push(storyboard);
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

            // TODO: Add your update logic here
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
