﻿namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    internal class DrawSpriteCommand : IDrawCommand {
        private readonly Texture2D _texture;
        private readonly Vector2 _position;
        private readonly Rectangle? _sourceRect;

        public DrawSpriteCommand(Sprite sprite, Vector2 position) {
            _texture = sprite.Texture;
            var width = sprite.SourceRectangle != null ? sprite.SourceRectangle.Value.Width : sprite.Width;
            var height = sprite.SourceRectangle!= null ? sprite.SourceRectangle.Value.Height : sprite.Height;
            _position = position + new Vector2(-width/2f, -height/2f);
            _sourceRect = sprite.SourceRectangle;
        }

        public void Draw(SpriteBatch spriteBatch) {

            spriteBatch.Draw(_texture, _position, _sourceRect, Color.White);
        }
    }
}