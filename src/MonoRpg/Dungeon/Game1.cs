using Microsoft.Xna.Framework;

using MonoRpg.Engine;

namespace Dungeon {
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Input;

    using MonoRpg.Engine.Tiled;
    using MonoRpg.Engine.UI;

    using System = MonoRpg.Engine.System;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager _graphics;
        private Renderer Renderer { get; set; }
        private Content _content;
        private StateStack _stack;
        private bool _start;

        public Game1() {
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
            _content = new Content(Content, GraphicsDevice);
            System.Content = _content;
            Renderer = new Renderer(GraphicsDevice, _content);
            System.Renderer = Renderer;
            Renderer.AlignText(TextAlignment.Center, TextAlignment.Center);
            Renderer.ClearColor = Color.Black;
            _content.SetDefaultFont("junction");
            _content.LoadFont("contra_italic");
            Icons.Instance = new Icons(_content.FindTexture("inventory_icons.png"));

            _stack = new StateStack();
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

            EntityDefs.Load("Content/entityDefs.json");
            MapDB.AddMap("sontos_house.json");



            var bustedWallTrigger = new TriggerDef("cracked_stone", 60,11 );
            var skeleton1 = new TriggerDef("skeleton",73, 11 );
            var skeleton2 = new TriggerDef("skeleton", 74, 11 );
            var gregorTrigger = new TriggerDef("gregor_trigger", 59, 11);
            var gregorTalkTrigger = new TriggerDef("gregor_talk_trigger", 50, 13);
            var grateTrigger1 = new TriggerDef("grate_close", 57, 6);
            var grateTrigger2 = new TriggerDef("grate_close", 58, 6);
            MapDB.AddMap("jail.json", new Dictionary<string, MapAction> {
                ["break_wall_script"] = MapAction.RunScript(CrumbleScript, bustedWallTrigger),
                ["bone_script"] = MapAction.RunScript(BoneScript, skeleton1),
                ["move_gregor"] = MapAction.RunScript(MoveGregor, gregorTrigger),
                ["talk_gregor"] = MapAction.RunScript(TalkGregor, gregorTalkTrigger),
                ["use_grate"] = MapAction.RunScript(UseGrate, grateTrigger1),
                ["enter_grate"] = MapAction.RunScript(EnterGrate, grateTrigger1)
                
            }, new Dictionary<string, TriggerTypeDef> {
                ["cracked_stone"] = new TriggerTypeDef { OnUse = "break_wall_script" },
                ["skeleton"]=new TriggerTypeDef { OnUse = "bone_script"},
                ["gregor_trigger"] = new TriggerTypeDef { OnExit = "move_gregor"},
                ["gregor_talk_trigger"] = new TriggerTypeDef { OnUse = "talk_gregor"},
                ["grate_close"] = new TriggerTypeDef { OnUse = "use_grate"},
                ["grate_open"] = new TriggerTypeDef { OnEnter = "enter_grate"}
            }, new List<TriggerDef> {
                bustedWallTrigger,
                skeleton1,
                skeleton2,
                gregorTrigger,
                gregorTalkTrigger,
                grateTrigger1,
                grateTrigger2
            }, new List<MapAction> {
                MapAction.AddNpc("gregor", "prisoner", 44, 12)
            });




            var storyboard = new Storyboard(_stack,
                Events.Scene(new SceneArgs {
                    Map = "sontos_house.json",
                    FocusX = 14,
                    FocusY = 19,
                    HideHero = true
                }),
                /*
                Events.BlackScreen(),
                Events.RunAction("AddNPC",
                    "sontos_house.json", new AddNPCParams { Character = "sleeper", Id = "sleeper", X = 14, Y = 19 },
                    Events.GetMapRef),
                Events.Play("rain.wav"),
                Events.NoBlock(Events.FadeSound("rain.wav", 0, 1, 3)),
                Events.Caption("place", "title", "Village of Sontos"),
                Events.Caption("time", "subtitle", "MIDNIGHT"),
                Events.Wait(2),
                Events.NoBlock(Events.FadeOutCaption("place", 3)),
                Events.FadeOutCaption("time", 3),
                Events.KillState("place"),
                Events.KillState("time"),
                Events.FadeOutScreen(),
                Events.Wait(2),
                Events.FadeInScreen(),
                Events.RunAction("AddNPC",
                    "sontos_house.json", new AddNPCParams { Character = "guard", Id = "guard1", X = 19, Y = 22 },
                    Events.GetMapRef),
                Events.Wait(1),
                Events.Play("door_break.wav"),
                Events.NoBlock(Events.FadeOutScreen()),
                Events.MoveNpc("guard1", "sontos_house.json",
                    Facing.Up, Facing.Up, Facing.Up, Facing.Left, Facing.Left, Facing.Left
                ),
                Events.Wait(1f),
                Events.Say("sontos_house.json", "guard1", "Found you!", 2.5f),
                //Events.Wait(1),
                Events.Say("sontos_house.json", "guard1", "You're coming with me!", 3),
                Events.FadeInScreen(),

                // Kidnap
                Events.NoBlock(Events.Play("bell.wav")),
                Events.Wait(2.5f),
                Events.NoBlock(Events.Play("bell.wav", "bell2")),
                Events.FadeSound("bell.wav", 1, 0, 0.2f),
                Events.FadeSound("rain.wav", 1, 0, 1),
                Events.Play("wagon.wav"),
                Events.NoBlock(Events.FadeSound("wagon.wav", 0, 1, 2)),
                Events.Play("wind.wav"),
                Events.NoBlock(Events.FadeSound("wind.wav", 0, 1, 2)),
                Events.Wait(3),
                Events.Caption("time_passes", "title", "Two days later..."),
                Events.Wait(1),
                Events.FadeOutCaption("time_passes", 3),
                Events.KillState("time_passes"),
                Events.NoBlock(Events.FadeSound("wind.wav", 1, 0, 1)),
                Events.NoBlock(Events.FadeSound("wagon.wav", 1, 0, 1)),
                Events.Wait(2),
                Events.Caption("place", "title", "Unknown Dungeon"),
                Events.Wait(2),
                Events.FadeOutCaption("place", 3),
                Events.KillState("place"),
                */
                Events.ReplaceScene("sontos_house.json", new SceneArgs {
                    Map = "jail.json",
                    FocusX = 56,
                    FocusY = 11,
                    HideHero = false

                }),
                //Events.FadeOutScreen(),
                //Events.Wait(0.5f),
                //Events.Say("jail.json", "hero", "Where am I?", 3),
                //Events.Wait(3),
                
                Events.HandOff("jail.json", _stack)
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
            //if (System.Keys.WasPressed(Keys.S)) {
            //    _start = true;
            //}
            //if (_start) {
            //    _stack.Update(dt);
            //}
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

        private void CrumbleScript(Map map, TriggerDef def, Entity entity) {
            void PushWall(Map map1) {
                _stack.PushFit(Renderer, 0, 0, "The wall crumbles", 255);
                System.Content.GetSound("crumble.wav").Play();

                map1.RemoveTrigger(def.X, def.Y, def.Layer);
                map1.WriteTile(
                               new WriteTileArgs {
                                   X = def.X,
                                   Y = def.Y,
                                   Layer = def.Layer,
                                   Tile = 134,
                                   Collision = false
                               });
            }

            var dialogParams = new FixedTextboxParameters {
                Choices = new SelectionArgs<string>(
                                                    new List<string> {
                                                        "Push the wall",
                                                        "Leave it alone"
                                                    }) {
                    OnSelection = (i, s) => {
                        if (i == 0) {
                            PushWall(map);
                        }
                    }
                }
            };
            _stack.PushFit(Renderer, 0, 0, "The wall here is crumbling. Push it?", 255, dialogParams);
        }
        const int BoneItemId = 4;
        private void BoneScript(Map map, TriggerDef def, Entity entity) {
            
            void GiveBone() {
                _stack.PushFit(Renderer, 0, 0, "Found key item: \"Calcified bone\"", 255, new FixedTextboxParameters { TextScale = 1 });
                World.Instance.AddKey(BoneItemId);
                System.Content.GetSound("key_item.wav").Play();

            }
            _stack.PushFit(Renderer, 0, 0, "The skeleton collapsed into dust.", 255, new FixedTextboxParameters { TextScale = 1, OnFinish = GiveBone });
            System.Content.GetSound("skeleton_destroy.wav").Play();

            map.RemoveTrigger(73, 11, def.Layer);
            map.RemoveTrigger(74, 11, def.Layer);
            map.WriteTile(new WriteTileArgs { X = 74, Y = 11, Layer = def.Layer, Tile = 136, Collision = true });
        }

        private void MoveGregor(Map map, TriggerDef def, Entity entity) {
            if (World.Instance.HasKey(BoneItemId)) {
                var gregor = map.NpcById["gregor"];
                gregor.FollowPath(
                    Facing.Up,
                    Facing.Up,
                    Facing.Up,
                    Facing.Right,
                    Facing.Right, 
                    Facing.Right,
                    Facing.Right,
                    Facing.Right,
                    Facing.Right,
                    Facing.Down,
                    Facing.Down,
                    Facing.Down
                );
                map.RemoveTrigger(def.X, def.Y, def.Layer);
            }
        }

        private void TalkGregor(Map map, TriggerDef def, Entity entity) {
            var gregor = map.NpcById["gregor"];
            if (gregor.Entity.TileX == def.X && gregor.Entity.TileY == def.Y - 1) {
                var speech = new[] {
                    "You're another black blood aren't you?", 
                    "Come the morning, they'll kill you, just like the others.",
                    "If I was you, I'd try to escape.",
                    "Pry the drain open with that big bone you're holding."
                };

                var text = speech[gregor.TalkIndex];

                var height = 102;
                var width = 500;
                var x = 0;
                var y = -System.ScreenHeight / 2 + height / 2;
                _stack.PushFix(
                               Renderer,
                               x,
                               y,
                               width,
                               height,
                               text,
                               new FixedTextboxParameters {
                                   TextScale = 1,
                                   Title = "Prisoner"
                               });
                gregor.TalkIndex = (gregor.TalkIndex + 1) % speech.Length;

            }
        }

        private void UseGrate(Map map, TriggerDef def, Entity entity) {

            void OnOpen() {
                System.Content.GetSound("grate.wav").Play();
                map.WriteTile(new WriteTileArgs {
                   X = 57, Y = 6, Layer = def.Layer, Tile = 151, Collision = false
                });
                map.WriteTile(new WriteTileArgs {
                    X = 58, Y = 6, Layer = def.Layer, Tile = 151, Collision = false
                });
                map.AddTrigger(new TriggerDef("grate_open", 57, 6));
                map.AddTrigger(new TriggerDef("grate_open", 58, 6));
            }

            if (World.Instance.HasKey(BoneItemId)) {
                var dialogParams = new FixedTextboxParameters {
                    TextScale = 1,
                    Choices = new SelectionArgs<string>(
                                                        new List<string> {
                                                            "Prize open the grate",
                                                            "Leave it alone"
                                                        }) {
                        OnSelection = (i, s) => {
                            if (i == 0) {
                                OnOpen();
                            }
                        }
                    }
                };
                _stack.PushFit(Renderer, 0, 0, "Threre's a tunnel behind the grate. Prize it open with the bone?", 300, dialogParams);
            } else {
                _stack.PushFit(Renderer, 0, 0, "There's a tunnel behind the grate. But you can't move the grate with your bare hands", 
                    300, new FixedTextboxParameters { TextScale = 1});
            }
            
        }
        private void EnterGrate(Map map, TriggerDef def, Entity entity) {

        }
    }
}
