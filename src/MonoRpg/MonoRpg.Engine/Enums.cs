namespace MonoRpg.Engine {
    public enum ItemType {
        None=-1,
        Accessory=1,
        Useable=0,
        Weapon=2,
        Armor=3,
        Key=4,
    }

    public enum Animations {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public enum Facing {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public enum TextAlignment {
        Top,
        Bottom,
        Left,
        Right,
        Center
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