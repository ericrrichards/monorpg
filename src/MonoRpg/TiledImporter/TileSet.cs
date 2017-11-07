namespace TiledImporter {
    using System.IO;
    using System.Xml.Linq;

    using Ardalis.GuardClauses;

    using TiledImporter;

    public class TileSet {
        public int FirstGID { get; }
        public string Name { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public int TileCount { get; }
        public int Columns { get; }
        private Image Image { get; }

        public TileSet(XElement tilesetElement) {
            Guard.Against.Null(tilesetElement, nameof(tilesetElement));
            Guard.Against.WrongXmlElement(tilesetElement, nameof(tilesetElement), "tileset");
            FirstGID = tilesetElement.ParseAttribute("firstgid");
            Name = tilesetElement.Attribute("name")?.Value;
            TileWidth = tilesetElement.ParseAttribute("tilewidth");
            TileHeight = tilesetElement.ParseAttribute("tileheight");
            TileCount = tilesetElement.ParseAttribute("tilecount");
            Columns = tilesetElement.ParseAttribute("columns");
            Image = new Image(tilesetElement.Element("image"));
        }

        public string GetImageFilename() {
            return Image.Source;
        }

        public Stream GetImageStream() {
            return File.Open(Image.Source, FileMode.Open);
        }

        public SpriteBounds GetTileSpriteBounds(int gid) {
            Guard.Against.OutOfRange(gid, nameof(gid), FirstGID, FirstGID + TileCount-1);
            gid = gid - FirstGID;
            var x = gid % Columns;
            var y = gid / Columns;
            return new SpriteBounds(x*TileWidth, y * TileHeight, TileWidth, TileHeight);
        }

        public bool ContainsGID(int gid) {
            return gid >= FirstGID && gid < FirstGID + TileCount;
        }
        
    }

    internal class Image {
        public string Source { get; }
        public int Width { get; }
        public int Height { get; }

        public Image(XElement imageElement) {
            Guard.Against.Null(imageElement, nameof(imageElement));
            Guard.Against.WrongXmlElement(imageElement, nameof(imageElement), "image");
            Source = imageElement.Attribute("source")?.Value;
            Width = imageElement.ParseAttribute("width");
            Height = imageElement.ParseAttribute("height");
        }
    }

    public class SpriteBounds {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;
        public int CenterX => X + Width / 2;
        public int CenterY => Y + Height / 2;
        public (int x, int y) Center => (CenterX, CenterY);

        public SpriteBounds(int x, int y, int width, int height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}