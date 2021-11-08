using System.Linq;
using System.Xml;

namespace Com.Ctrip.Framework.Apollo
{
    internal static class XmlExtensions
    {
        public static void Bind(this XmlNode? xmlNode, IConfig config, string keyPrefix)
        {
            if (xmlNode == null) return;

            foreach (var key in config.GetChildren(keyPrefix))
            {
                if (config.GetChildren(key.FullName).Any())
                    CreateXmlNode(xmlNode, key.Name).Bind(config, key.FullName);
                else
                    AddOrUpdateAttribute(xmlNode, key.Name, config.GetProperty(key.FullName, ""));
            }
        }

        private static void AddOrUpdateAttribute(XmlNode xmlNode, string name, string value)
        {
            if (xmlNode.Attributes == null) return;

            foreach (XmlAttribute attr in xmlNode.Attributes)
            {
                if (attr.Name != name) continue;

                attr.Value = value;

                return;
            }

            if (xmlNode.OwnerDocument == null) return;

            var createAttribute = xmlNode.OwnerDocument.CreateAttribute(name, xmlNode.NamespaceURI);

            createAttribute.Value = value;

            xmlNode.Attributes.Append(createAttribute);
        }

        private static XmlNode? CreateXmlNode(XmlNode? xmlNode, string nodeName)
        {
            if (xmlNode?.OwnerDocument == null || string.IsNullOrEmpty(nodeName)) return null;

            if (xmlNode[nodeName] == null)
                xmlNode.AppendChild(xmlNode.OwnerDocument.CreateElement(nodeName));

            return xmlNode[nodeName];
        }
    }
}
