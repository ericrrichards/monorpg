namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public static class System {
        private static GraphicsDeviceManager _graphics;

        public static void Init(GraphicsDeviceManager graphics) {
            _graphics = graphics;
        }
        public static int ScreenWidth => _graphics.PreferredBackBufferWidth;
        public static int ScreenHeight => _graphics.PreferredBackBufferHeight;
        public static Content Content { get; set; }

        public static GraphicsDevice Device => _graphics.GraphicsDevice;
        public static Renderer Renderer { get; set; }
    }
}