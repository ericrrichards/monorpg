namespace MonoRpg.Engine.UI {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

    using JetBrains.Annotations;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Icons {
        public static Icons Instance;

        public Dictionary<IconDefs, Sprite> Sprites { get; set; }
        public List<Rectangle> UVs { get; set; }
        public Texture2D Texture { get; private set; }

        public Icons(Texture2D texture) {
            Texture = texture;
            UVs = texture.GenerateUVs(18, 18);
            Sprites = new Dictionary<IconDefs, Sprite>();

            foreach (var icon in Enum.GetValues(typeof(IconDefs)).Cast<IconDefs>()) {
                var sprite = new Sprite();
                sprite.Texture = Texture;
                sprite.SetUVs(UVs[(int)icon]);
                Sprites[icon] = sprite;
            }
        }
        [CanBeNull]
        public Sprite Get(ItemType id) {
            if (Sprites.ContainsKey((IconDefs)id)) {
                return Sprites[(IconDefs)id];
            }
            return null;
        }

        
    }

    public enum IconDefs {
        Useable = 0,
        Accessory = 1,
        Weapon = 2,
        Armor = 3,
        UpArrow = 4,
        DownArrow = 5
    }
}