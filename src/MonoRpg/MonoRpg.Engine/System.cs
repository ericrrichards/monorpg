namespace MonoRpg.Engine {
    using global::System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public static class System {
        private static GraphicsDeviceManager _graphics;

        public static void Init(GraphicsDeviceManager graphics) {
            _graphics = graphics;
        }



        public static Content Content { get; set; }

        public static GraphicsDevice Device => _graphics.GraphicsDevice;
        public static RenderEngine.Renderer Renderer { get; set; }
        public static Action Exit { get; set; }
        public static readonly Keyboard Keys= new Keyboard();

        public static class Screen {
            public static int Width => _graphics.PreferredBackBufferWidth;
            public static int HalfWidth => Width / 2;
            public static int Height => _graphics.PreferredBackBufferHeight;
            public static int HalfHeight => Height / 2;
            public static Rectangle Bounds => RectangleEx.Create(-HalfWidth, HalfHeight, HalfWidth, -HalfHeight);
        }
    }

    

    public class Keyboard {
        private KeyboardState CurrentState { get; set; }
        private KeyboardState LastState { get; set; }

        internal Keyboard() {
            CurrentState = new KeyboardState();
            LastState = new KeyboardState();
        }

        public void Update(KeyboardState state) {
            LastState = CurrentState;
            CurrentState = state;
        }

        public bool WasPressed(Keys key) {
            return LastState.IsKeyUp(key) && CurrentState.IsKeyDown(key);
        }

        public bool WasReleased(Keys key) {
            return LastState.IsKeyDown(key) && CurrentState.IsKeyUp(key);
        }

        public bool IsDown(Keys key) {
            return CurrentState.IsKeyDown(key);
        }

        public bool IsUp(Keys key) {
            return CurrentState.IsKeyUp(key);
        }
    }

    public static class RectangleEx {
        public static Rectangle Create(int left, int top, int right, int bottom) {
            return new Rectangle(left, top, right-left, bottom - top);
        }
    }
}