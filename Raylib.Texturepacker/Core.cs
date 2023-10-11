using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Raylib.Texturepacker
{
    public class AtlasDocument
    {
        public string Directory { get; set; }

        protected XDocument ReadXml(string path)
        {
            var fullPath = Path.GetFullPath(path);
            Directory = Path.GetDirectoryName(fullPath);

            XDocument xDoc = XDocument.Load(path);

            return xDoc;

        }
    }
}