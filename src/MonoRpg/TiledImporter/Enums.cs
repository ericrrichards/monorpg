namespace TiledImporter {
    public enum RenderOrder {
        RightDown,
        RightUp,
        LeftDown,
        LeftUp
    }

    public enum Orientation {
        Orthagonal,
        Isometric,
        Staggered,
        Hexagonal
    }

    public enum Encoding {
        XML,
        CSV,
        Base64
    }

    public enum Compression {
        None,
        GZip,
        ZLib
    }
}