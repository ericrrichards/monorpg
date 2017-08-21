namespace MonoRpg.Engine {
    using global::System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public static class Texture2DExtensions {
        public static List<Rectangle> GenerateUVs(this Texture2D texture, int tileSize) { return GenerateUVs(texture, tileSize, tileSize); }
        public static List<Rectangle> GenerateUVs(this Texture2D texture, int tileWidth, int tileHeight) {
            var uvs = new List<Rectangle>();

            float texWidth = texture.Width;
            float texHeight = texture.Height;
            var cols = texWidth / tileWidth;
            var rows = texHeight / tileHeight;

            var u0 = 0;
            var v0 = 0;
            for (var j = 0; j < rows; j++) {
                for (var i = 0; i < cols; i++) {
                    uvs.Add(new Rectangle(u0, v0, tileWidth, tileHeight));
                    u0 += tileWidth;
                }
                u0 = 0;
                v0 += tileHeight;
            }


            return uvs;
        }
    }
}