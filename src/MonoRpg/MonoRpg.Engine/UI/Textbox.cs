namespace MonoRpg.Engine.UI {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class Textbox {
        private float _keyboardBuffer;
        public Vector4 Bounds { get; set; }
        public Rectangle Size { get; set; }
        public float TextScale { get; set; }
        public Panel Panel { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Tween AppearTween { get; set; }
        public bool IsDead => AppearTween.Finished && AppearTween.Value == 0;
        public int Wrap { get; set; }
        public List<TextboxChild> Children { get; set; }
        public List<string> Chunks { get; set; }
        public int ChunkIndex { get; set; }
        public Sprite ContinueMark { get; set; }
        public float Time { get; set; }
        public Selection SelectionMenu { get; set; }

        public Textbox(TextboxParams parameters) {
            parameters = parameters ?? new TextboxParams();


            Chunks = parameters.Text;
            ChunkIndex = 0;
            ContinueMark = new Sprite {
                Texture = System.Content.FindTexture("continue_caret.png")
            };
            Time = 0f;
            TextScale = parameters.TextScale;
            Panel = new Panel(parameters.PanelArgs){PixelArt = true};
            Size = parameters.Size;
            Bounds = parameters.TextBounds;

            X = (Size.Right + Size.Left) / 2;
            Y = (Size.Top + Size.Bottom) / 2;
            Width = Math.Abs(Size.Right - Size.Left);
            Height = Math.Abs(Size.Top - Size.Bottom);
            AppearTween = new Tween(0, 1, 0.4f, Tween.EaseOutCirc);
            Wrap = parameters.Wrap;
            Children = parameters.Children;
            SelectionMenu = parameters.SelectionMenu;
        }

        

        public void Update(float dt) {
            Time += dt;
            AppearTween.Update(dt);
        }

        public void OnClick() {
            if (ChunkIndex >= Chunks.Count-1) {
                if (!(AppearTween.Finished && AppearTween.Value == 1)) {
                    return;
                }
                AppearTween = new Tween(1, 0, 1.2f, Tween.EaseInCirc);
            } else {
                ChunkIndex++;
            }
        }

        public void HandleInput(float dt) {
            if (IsDead) {
                return;
            }
            var ks = Keyboard.GetState();
            if (ks.GetPressedKeys().Any() && _keyboardBuffer <= 0.0f) {
                SelectionMenu?.HandleInput();
                
                if (ks.IsKeyDown(Keys.Space)) {
                    OnClick();
                }
                _keyboardBuffer = 0.1f;
            }
            _keyboardBuffer -= dt;
        }

        public void Render(Renderer renderer) {
            var scale = AppearTween.Value;
            renderer.SetTextAlignment(TextAlignment.Left, TextAlignment.Top);

            Panel.CenterPosition(X, Y, (int)(Width*scale), (int)(Height* scale));
            Panel.Render(renderer);

            var left = X - (Width / 2f * scale);
            var textLeft = left + (Bounds.X * scale);
            var top = Y + Height / 2f * scale;
            var textTop = top + Bounds.Z * scale;
            var bottom = Y - Height / 2f * scale;
            Console.WriteLine("{0}\t{1}", textLeft, Chunks[ChunkIndex]);
            var bounds = new TextboxBounds {
                Left = left,
                Top = top,
                TextLeft = textLeft,
                TextTop = textTop,
                Scale = scale
            };

            renderer.DrawText2D((int)textLeft, (int)textTop, Chunks[ChunkIndex], Color.White, TextScale * scale, (int)(Wrap*scale));

            if (SelectionMenu != null) {
                renderer.SetTextAlignment(TextAlignment.Left, TextAlignment.Center);
                var menuX = (int)textLeft;
                var menuY = (int)(bottom + SelectionMenu.GetHeight());
                menuY -= (int)Bounds.W;
                SelectionMenu.Position = new Vector2(menuX, menuY);
                SelectionMenu.Scale = scale;
                SelectionMenu.TextScale = TextScale;
                SelectionMenu.Render(renderer);
            }

            if (ChunkIndex < Chunks.Count-1) {
                var offset = 12 + (float)Math.Floor(Math.Sin(Time * 10)) * scale;
                ContinueMark.Scale = new Vector2(scale, scale);
                ContinueMark.Position = new Vector2(X, bottom + offset);
                renderer.DrawSprite(ContinueMark);
            }
            foreach (var child in Children) {
                child.Render(renderer, bounds, this);
            }
        }

        
    }

    public class TextboxBounds {
        public float Left { get; set; }
        public float Top { get; set; }
        public float TextLeft { get; set; }
        public float TextTop { get; set; }
        public float Scale { get; set; }
    }

    public class TextboxParams {
        public List<string> Text { get; set; }
        public float TextScale { get; set; }
        public PanelParams PanelArgs { get; set; }
        public Rectangle Size { get; set; }
        public Vector4 TextBounds { get; set; }
        public int Wrap { get; set; }
        public List<TextboxChild> Children { get; set; }
        public Selection SelectionMenu { get; set; }

        public TextboxParams() {
            Wrap = -1;
            Children = new List<TextboxChild>();
        }
    }

    public abstract class TextboxChild {
        public int X { get; set; }
        public int Y { get; set; }

        public abstract void Render(Renderer renderer, TextboxBounds bounds, Textbox textbox);
    }

    public class TextChild :TextboxChild{
        public string Text { get; set; }

        public override void Render(Renderer renderer, TextboxBounds bounds, Textbox textbox) {
            renderer.DrawText2D(
                (int)(bounds.TextLeft + X * bounds.Scale), 
                (int)(bounds.TextTop + Y * bounds.Scale), 
                Text, Color.White, bounds.Scale*textbox.TextScale
            );
        }
    }

    public class SpriteChild : TextboxChild {
        public Sprite Sprite { get; set; }

        public override void Render(Renderer renderer, TextboxBounds bounds, Textbox textbox) {
            Sprite.Position = new Vector2(bounds.Left + X , bounds.Top + Y );
            Sprite.Scale = new Vector2(bounds.Scale, bounds.Scale);
            renderer.DrawSprite(Sprite);
        }
    }
}