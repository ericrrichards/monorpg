using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace Dungeon {
    using System.Collections.Generic;

    using MonoRpg.Engine.Tiled;
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
            EntityDefs.Load("Content/entityDefs.json");
            MapDB.AddMap("sontos_house.json");
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
                Scene(new SceneArgs {
                    Map = "sontos_house.json",
                    FocusX = 14,
                    FocusY = 20,
                    HideHero = true
                }),
                BlackScreen(),
                RunAction("AddNPC",
                    "sontos_house.json", new AddNPCParams { Character = "sleeper", Id = "sleeper", X = 14, Y = 19 },
                    GetMapRef),
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
                KillState("place"),
                KillState("time"),
                FadeOutScreen(),
                Wait(3),
                FadeInScreen(),
                RunAction("AddNPC",
                    "sontos_house.json", new AddNPCParams { Character = "guard", Id = "guard1", X = 19, Y = 22 },
                    GetMapRef),
                NoBlock(FadeOutScreen()),
                MoveNpc("guard1", "sontos_house.json",
                    Facing.Up, Facing.Up, Facing.Up, Facing.Left, Facing.Left, Facing.Left
                ),
                Wait(0.3f),
                Say("sontos_house.json", "guard1", "Take him!"),
                FadeInScreen(),
                FadeSound("rain.wav", 1, 0, 1),
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
