using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoRpg.Engine;

namespace MonoRpg {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Microsoft.Xna.Framework.Graphics;

    using MonoRpg.Engine.Tiled;
    using MonoRpg.Engine.UI;

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
        private Textbox _textBox;
        private float _keyboardBuffer = 0;

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
            _renderer = new Renderer(GraphicsDevice, _content);
            _renderer.SetTextAlignment(TextAlignment.Center, TextAlignment.Center);
            //_renderer.ClearColor = Color.White;


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

            _textBox = CreateFixed(_renderer, -System.ScreenWidth/2+2, -System.ScreenHeight/2 , System.ScreenWidth-4, 102,
                "A nation can survive its fools, and even the ambitious. But it cannot survive treason from within. An enemy at the gates is less formidable, " + 
                "for he is known and carries his banner openly. But the traitor moves amongst those within the gate freely, his sly whispers rustling through " + 
                "all the alleys, heard in the very halls of government itself. For the traitor appears not a traitor; he speaks in accents familiar to his " + 
                "victims, and he wears their face and their arguments, he appeals to the baseness that lies deep in the hearts of all men. He rots the soul " + 
                "of a nation, he works secretly and unknown in the night to undermine the pillars of the city, he infects the body politic so that it can no " + 
                "longer resist. A murderer is less to fear.",
                new FixedTextboxParameters()
            );


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
            _hero.Controller.Update(dt);
            foreach (var npc in _map.NPCs) {
                npc.Controller.Update(dt);
            }
            


            var playerPos = _hero.Entity.Sprite.Position;
            _map.CamX = (int)Math.Floor(playerPos.X);
            _map.CamY = (int)Math.Floor(playerPos.Y);
            if (!_textBox.IsDead) {
                _textBox.Update(dt);
            }
            if (ks.IsKeyDown(Keys.Space) && _keyboardBuffer < 0.0f) {
                _textBox.OnClick();
                _keyboardBuffer = 1.0f;
            }


            _keyboardBuffer -= dt;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            var layerCount = _map.LayerCount;
            for (var i = 0; i < layerCount; i++) {
                Entity heroEntity = null;
                if (i == _hero.Entity.Layer) {
                    heroEntity = _hero.Entity;
                }
                _map.RenderLayer(_renderer, i, heroEntity);

            }
            

            _textBox.Render(_renderer);

            _renderer.Render();

            base.Draw(gameTime);
        }

        public Textbox CreateFixed(Renderer renderer, int x, int y, int width, int height, string text, FixedTextboxParameters parameters) {
            var padding = 10;
            var textScale = 1.25f;
            var panelTileSize = 3;
            var wrap = width - padding;
            var boundsTop = padding;
            var boundsLeft = padding;
            var boundsBottom = padding;

            var children = new List<TextboxChild>();
            var avatar = parameters.Avatar;
            var title = parameters.Title;

            if (avatar != null) {
                boundsLeft = avatar.Width + padding * 2;
                wrap = width - boundsLeft - padding;
                var sprite = new Sprite {
                    Texture = avatar
                };
                children.Add(new SpriteChild {
                    Sprite = sprite,
                    X = avatar.Width / 2 + padding,
                    Y = -avatar.Height / 2
                });
            }
            if (!string.IsNullOrEmpty(title)) {
                var size = renderer.MeasureText(title, wrap);
                boundsTop = (int)(size.Y + padding * 2);
                children.Add(new TextChild { Text = title, X = 0, Y = (int)(size.Y + padding) });
            }

            var faceHeight = renderer.TextHeight(text.Substring(0, 1), wrap);
                
            var lines = renderer.ChunkText(text, wrap, textScale);
            var chunks = new List<string>();
            var currentHeight = 0f;
            var boundsHeight = height - (boundsTop + boundsBottom);
            var currentChunk = new StringBuilder();
            foreach (var line in lines) {
                if (currentHeight + faceHeight > boundsHeight) {
                    currentHeight = faceHeight;
                    chunks.Add(currentChunk.ToString().Trim());
                    currentChunk = new StringBuilder(line);
                } else {
                    currentChunk.Append(line);
                    currentHeight += faceHeight;
                }
            }
            if (!string.IsNullOrEmpty(currentChunk.ToString())) {
                chunks.Add(currentChunk.ToString());
            }



            var textbox = new Textbox(new TextboxParams {
                Text = chunks,
                TextScale = textScale,
                Size = new Rectangle(x, y, width, height),
                TextBounds = new Vector4(boundsLeft, -padding, -boundsTop, padding),
                Wrap = wrap,
                PanelArgs = new PanelParams {
                    Texture = System.Content.FindTexture("simple_panel.png"),
                    Size = panelTileSize
                },
                Children = children
            });

            return textbox;
        }

        public class FixedTextboxParameters {
            public Texture2D Avatar { get; set; }
            public string Title { get; set; }
        }

    }
}
