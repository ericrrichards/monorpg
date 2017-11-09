using Microsoft.Xna.Framework;

using MonoRpg.Engine;

namespace MonoRpg {
    using System.Collections.Generic;

    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.Tiled;
    using MonoRpg.Engine.UI;

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
            _content = Engine.Content.Create(Content, GraphicsDevice);
            Renderer = Renderer.Create(GraphicsDevice, _content);


            var mapDef = _content.LoadMap("Content/small_room.json");

            mapDef.OnWake = new List<MapAction> {
            };
            mapDef.Actions = new Dictionary<string, MapAction> {
            };
            mapDef.TriggerTypes = new Dictionary<string, TriggerTypeDef> {
            };
            mapDef.Triggers = new List<TriggerDef> {
            };

            


            ItemDB.Initialize(
                new Item {
                    Name = "Mysterious Torque",
                    Type = ItemType.Accessory,
                    Description = "A golden torque that glitters"
                },
                new Item {
                    Name = "Heal Potion",
                    Type = ItemType.Useable,
                    Description = "Heals a little HP"
                },
                new Item {
                    Name = "Bronze Sword",
                    Type = ItemType.Weapon,
                    Description = "A short sword with a dull blade"
                },
                new Item {
                    Name = "Old bone",
                    Type = ItemType.Key,
                    Description = "A calcified human femur"
                }

            );

            
            World.Instance.AddItem(3);
            World.Instance.AddItem(1);
            World.Instance.AddItem(2, 4);
            World.Instance.AddKey(4);
            
            Icons.Instance = new Icons(_content.FindTexture("inventory_icons.png"));

            _stack = new StateStack();
            _stack.Push(new ExploreState(_stack, mapDef, new Vector3(11, 3, 0)));
            _stack.Push(new InGameMenu(_stack));


            


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
            System.Keys.Update(Microsoft.Xna.Framework.Input.Keyboard.GetState());

            World.Instance.Update(dt);


            




            
            _stack.Update(dt);
            //if (ks.IsKeyDown(Keys.F)) {
            //    _stack.Push(new FadeState(_stack, new FadeArgs() { AlphaStart = 1, AlphaFinish = 0 }));
            //}

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

    public class Block : IStateObject {
        private StateStack Stack { get; set; }

        public Block(StateStack stack) {
            Stack = stack;
        }

        public void Enter(EnterArgs arg) { }
        public void Exit() { }

        public bool Update(float dt) {
            return false;
        }
        public void Render(Renderer renderer) { }

        public void HandleInput(float dt) {
            Stack.Pop();
        }
    }
}
