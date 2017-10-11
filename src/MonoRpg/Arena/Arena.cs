using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Arena {
    using System.Collections.Generic;

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
        private Dictionary<string, ActorDef> _partyMemberDefs;

        public Arena() {
            _graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 640,
                PreferredBackBufferHeight = 360
            };
            System.Init(_graphics);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _partyMemberDefs = new Dictionary<string, ActorDef> {
                ["hero"] = new ActorDef {
                    Id = "hero",
                    Stats = new[] {
                        new Stat(Stats.HitPoints, 300),
                        new Stat(Stats.MaxHitPoints, 300),
                        new Stat(Stats.MagicPoints, 300),
                        new Stat(Stats.MaxMagicPoints, 300),
                        new Stat(Stats.Strength, 10),
                        new Stat(Stats.Speed, 10),
                        new Stat(Stats.Intelligence, 10), 
                    },
                    StatGrowth = new Dictionary<string, Dice> {
                        [Stats.MaxHitPoints] = new Dice("4d50+100"),
                        [Stats.MaxMagicPoints] = new Dice("2d50+100"),
                        [Stats.Strength] = Growth.Fast,
                        [Stats.Speed] = Growth.Fast,
                        [Stats.Intelligence] = Growth.Med
                    },
                    Portrait = "hero_portrait.png",
                    Name = "Seven",
                    Actions = new List<string> { "attack", "item"}
                },
                ["thief"] = new ActorDef {
                    Id = "thief",
                    Stats = new[] {
                        new Stat(Stats.HitPoints, 280),
                        new Stat(Stats.MaxHitPoints, 280),
                        new Stat(Stats.MagicPoints, 150),
                        new Stat(Stats.MaxMagicPoints, 150),
                        new Stat(Stats.Strength, 10),
                        new Stat(Stats.Speed, 15),
                        new Stat(Stats.Intelligence, 10),
                    },
                    StatGrowth = new Dictionary<string, Dice> {
                        [Stats.MaxHitPoints] = new Dice("3d40+100"),
                        [Stats.MaxMagicPoints] = new Dice("4d50+100"),
                        [Stats.Strength] = Growth.Med,
                        [Stats.Speed] = Growth.Fast,
                        [Stats.Intelligence] = Growth.Med
                    },
                    Portrait = "thief_portrait.png",
                    Name = "Jude",
                    Actions = new List<string> { "attack", "item" }
                },
                ["mage"] = new ActorDef {
                    Id = "mage",
                    Stats = new[] {
                        new Stat(Stats.HitPoints, 100),
                        new Stat(Stats.MaxHitPoints, 200),
                        new Stat(Stats.MagicPoints, 250),
                        new Stat(Stats.MaxMagicPoints, 250),
                        new Stat(Stats.Strength, 8),
                        new Stat(Stats.Speed, 10),
                        new Stat(Stats.Intelligence, 20),
                    },
                    StatGrowth = new Dictionary<string, Dice> {
                        [Stats.MaxHitPoints] = new Dice("3d40+100"),
                        [Stats.MaxMagicPoints] = new Dice("4d50+100"),
                        [Stats.Strength] = Growth.Med,
                        [Stats.Speed] = Growth.Med,
                        [Stats.Intelligence] = Growth.Fast
                    },
                    Portrait = "mage_portrait.png",
                    Name = "Ermis",
                    Actions = new List<string> { "attack", "item" }
                },
            };
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

            //_content.SetDefaultFont("junction");
            //_content.LoadFont("contra_italic");

            Icons.Instance = new Icons(_content.FindTexture("inventory_icons.png"));

            World.Instance.Party.Add(new Actor(_partyMemberDefs["hero"]));
            World.Instance.Party.Add(new Actor(_partyMemberDefs["thief"]));
            World.Instance.Party.Add(new Actor(_partyMemberDefs["mage"]));

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
