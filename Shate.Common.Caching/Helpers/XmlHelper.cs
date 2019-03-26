using System;
using System.Xml;

namespace Shate.Common.Caching
{
    internal class XmlHelper
    {
        public static string GetString(XmlNode node, string attrName)
        {
            return SetByXElement<string>(node, attrName, Convert.ToString);
        }

        public static bool GetBool(XmlNode node, string attrName)
        {
            return SetByXElement<bool>(node, attrName, Convert.ToBoolean);
        }

        public static int GetInt(XmlNode node, string attrName)
        {
            return SetByXElement<int>(node, attrName, Convert.ToInt32);
        }

        public static T SetByXElement<T>(XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node?.Attributes == null) return default(T);

            var attr = node.Attributes[attrName];

            if (attr == null) return default(T);

            var attrVal = attr.Value;

            return converter(attrVal);
        }
    }
}
