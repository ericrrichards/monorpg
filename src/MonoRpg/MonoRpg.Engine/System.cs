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
        public static int ScreenWidth => _graphics.PreferredBackBufferWidth;
        public static int ScreenHeight => _graphics.PreferredBackBufferHeight;
        public static Content Content { get; set; }

        public static GraphicsDevice Device => _graphics.GraphicsDevice;
        public static RenderEngine.Renderer Renderer { get; set; }
        public static Action Exit { get; set; }
        public static readonly Keyboard Keys= new Keyboard();

        
    }
    public class Keyboard {
        private KeyboardState CurrentState { get; set; }
        private KeyboardState LastState { get; set; }

        public Keyboard() {
            CurrentState = new KeyboardState();
            LastState = new KeyboardState();
        }

        public void Update() {
            LastState = CurrentState;
            CurrentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
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
}