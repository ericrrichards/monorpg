namespace TiledImporter {
    using System;
    using System.Xml.Linq;

    public static class XElementExtensions {
        public static T ParseAttribute<T>(this XElement element, string attribute) where T : struct {
            Enum.TryParse(element.Attribute(attribute)?.Value.Replace("-", string.Empty), true, out T value);
            return value;
        }

        public static int ParseAttribute(this XElement element, string attribute) {
            return Convert.ToInt32(element.Attribute(attribute)?.Value);
        }
    }
}