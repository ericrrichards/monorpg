namespace MonoRpg.Engine.UI {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Text;

    using JetBrains.Annotations;

    using Microsoft.Xna.Framework;

    using MonoRpg.Engine.GameStates;

    public class StateStack {
        public List<IStateObject> States { get; set; }
        public StateStack() {
            States = new List<IStateObject>();
        }

        public void Update(float dt) {
            for (var index = States.Count - 1; index >= 0; index--) {
                if (index >= States.Count) {
                    continue;
                }
                var state = States[index];
                var keepGoing = state.Update(dt);
                if (!keepGoing) {
                    break;
                }
            }
            var top = States.LastOrDefault();
            top?.HandleInput(dt);

        }

        public void Push(IStateObject state) {
            States.Add(state);
        }
        [CanBeNull]
        public IStateObject Pop() {
            var top = States.LastOrDefault();
            if (top == null) {
                return null;
            }
            States.Remove(top);
            top.Exit();
            return top;
        }
        [CanBeNull]
        public IStateObject Top => States.LastOrDefault();


        public void Render(Renderer renderer) {
            foreach (var state in States) {
                state.Render(renderer);
            }
        }

        public Textbox PushFix(Renderer renderer, int x, int y, int width, int height, string text, FixedTextboxParameters parameters = null) {
            if (parameters == null) {
                parameters = new FixedTextboxParameters();
            }
            var padding = 10;
            var textScale = parameters.TextScale;
            var panelTileSize = 3;
            var wrap = width - padding;
            var boundsTop = padding;
            var boundsLeft = padding;
            var boundsBottom = padding;

            var children = new List<TextboxChild>();
            var avatar = parameters.Avatar;
            var title = parameters.Title;
            var choices = parameters.Choices;

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

            Selection<string> selectionMenu = null;
            if (choices != null) {
                selectionMenu = new Selection<string>(renderer, choices);
                boundsBottom -= padding / 2;
            }

            if (!string.IsNullOrEmpty(title)) {
                var size = renderer.MeasureText(title, wrap, textScale);
                boundsTop = (int)(size.Y + padding * 2 + parameters.TitlePadY);
                children.Add(new TextChild { Text = title, X = 0, Y = (int)size.Y });
            }

            var faceHeight = renderer.TextHeight(text.Substring(0, 1), wrap, textScale);

            var lines = renderer.ChunkText(text, wrap, textScale);
            var chunks = new List<string>();
            var currentHeight = 0f;
            var boundsHeight = height - (boundsTop + boundsBottom);
            var currentChunk = new StringBuilder();
            foreach (var line in lines) {
                if (currentHeight + faceHeight > boundsHeight) {
                    currentHeight = faceHeight;
                    var s = currentChunk.ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(s)) {
                        chunks.Add(s);
                    }
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
                Size = new Rectangle(x-width/2, y-height/2, width, height),
                TextBounds = new Vector4(boundsLeft, -padding, -boundsTop, padding),
                Wrap = wrap,
                SelectionMenu = selectionMenu,
                OnFinish = parameters.OnFinish,
                Stack = this,
                PanelArgs = new PanelParams {
                    Texture = System.Content.FindTexture("simple_panel.png"),
                    Size = panelTileSize
                },
                Children = children

            });

            States.Add(textbox);
            return textbox;
        }

        public Textbox PushFit(Renderer renderer, int x, int y, string text, int wrap = -1, FixedTextboxParameters args = null) {
            if (args == null) {
                args = new FixedTextboxParameters();
            }
            var choices = args.Choices;
            var title = args.Title;
            var avatar = args.Avatar;

            var padding = 10;
            var textScale = args.TextScale;

            var size = renderer.MeasureText(text, wrap, textScale);
            var width = (int)(size.X + padding * 3);
            var height = (int)(size.Y + padding * 2);

            if (choices != null) {
                var selectionMenu = new Selection<string>(renderer, choices);
                height += selectionMenu.GetHeight() + padding ;
                width = Math.Max(width, selectionMenu.GetWidth() + padding * 2);
            }
            if (!string.IsNullOrEmpty(title)) {
                var titleSize = renderer.MeasureText(title, wrap, textScale);
                height += (int)(titleSize.Y + padding + args.TitlePadY);
                width = Math.Max(width, (int)(size.X + padding * 2));
            }
            if (avatar != null) {
                var avatarWidth = avatar.Width;
                var avatarHeight = avatar.Height;
                width += avatarWidth + padding;
                height = Math.Max(height, avatarHeight + padding);
            }
            
            return PushFix(renderer, x, y, width, height, text, args);
        }


    }
}