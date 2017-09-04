using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace MonoRpg {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
        private World _world;
        private Selection<ItemCount> _itemList;
        private Selection<int?> _keyItemList;

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
            System.Renderer = Renderer;
            Renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            Renderer.ClearColor = Color.CornflowerBlue;


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
                    Description = "A golden torque that glitters",
                    Stats = new ItemStats {
                        Strength = 10,
                        Speed = 10
                    }
                },
                new Item {
                    Name = "Heal Potion",
                    Type = ItemType.Useable,
                    Description = "Heals a little HP"
                },
                new Item {
                    Name = "Bronze Sword",
                    Type = ItemType.Weapon,
                    Description = "A short sword with a dull blade",
                    Stats = new ItemStats {
                        Attack = 10
                    }
                },
                new Item {
                    Name = "Old bone",
                    Type = ItemType.Key,
                    Description = "A calcified human femur"
                }

            );

            _world = new World();
            _world.AddItem(3);

            _itemList = new Selection<ItemCount>(Renderer, new SelectionArgs<ItemCount>(_world.Items)) {
                SpacingY = 32,
                DisplayRows = 5,
                RenderItem = (renderer, x, y, item) => {
                    var i = item as ItemCount;
                    if (i != null) {
                        var itemDef = ItemDB.Items[i.ItemId];
                        var label = $"{itemDef.Name} ({i.Count})";
                        renderer.DrawText2D(x, y, label);
                    } else {
                        renderer.DrawText2D(x,y, "--");
                    }
                }
            };
            _keyItemList = new Selection<int?>(Renderer, new SelectionArgs<int?>(_world.KeyItems)) {
                SpacingY = 32,
                DisplayRows = 5,
                RenderItem = (renderer, x, y, item) => {
                    var itemId = item as int?;
                    if (itemId != null) {
                        var itemDef = ItemDB.Items[itemId.Value];
                        renderer.DrawText2D(x, y, itemDef.Name);
                    } else {
                        renderer.DrawText2D(x,y, "--");
                    }
                }
            };
            _keyItemList.HideCursor();

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
            System.Keys.Update();

            _world.Update(dt);


            _itemList.HandleInput();

            if (System.Keys.WasPressed(Keys.A)) {
                _world.AddItem(1);
            } else if (System.Keys.WasPressed(Keys.R)) {
                var item = _itemList.SelectedItem;
                if (item != null) {
                    _world.RemoveItem(item.ItemId);
                }
            } else if (System.Keys.WasPressed(Keys.K)) {
                if (!_world.HasKey(4)) {
                    _world.AddKey(4);
                }
            } else if (System.Keys.WasPressed(Keys.U)) {
                if (_world.HasKey(4)) {
                    _world.RemoveKey(4);
                }
            } else if (System.Keys.WasPressed(Keys.G)) {
                _world.Gold += DateTime.Now.Millisecond % 100;
            }




            
            //_stack.Update(dt);
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

            //_stack.Render(Renderer);

            var x = -200;
            var y = 50;
            Renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            Renderer.DrawText2D(x + _itemList.GetWidth()/2, y + 32, "ITEMS");
            Renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
            _itemList.Position = new Vector2(x,y);
            _itemList.Render(Renderer);

            x = 100;

            Renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            Renderer.DrawText2D(x + _itemList.GetWidth()/2, y + 32, "KEY ITEMS");
            _keyItemList.Position = new Vector2(x,y);
            _keyItemList.Render(Renderer);

            var timeText = $"TIME: {_world.TimeString}";
            var goldText = $"GOLD: {_world.Gold}";
            Renderer.DrawText2D(0, 150, timeText);
            Renderer.DrawText2D(0, 120, goldText);

            var tip = "A - Add Item, R - Remove Item, K - Add Key, U - Use Key, G - Add Gold";
            Renderer.DrawText2D(0, -150, tip, Color.White, 1.0f, 300);


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
