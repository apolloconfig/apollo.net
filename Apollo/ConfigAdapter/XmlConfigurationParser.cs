using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Com.Ctrip.Framework.Apollo.ConfigAdapter
{
    internal static class XmlConfigurationParser
    {
        private const string NameAttributeKey = "Name";

        /// <summary>
        /// Read a stream of INI values into a key/value dictionary.
        /// </summary>
        /// <param name="stream">The stream of INI data.</param>
        /// <returns></returns>
        public static IDictionary<string, string> Read(TextReader stream)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var readerSettings = new XmlReaderSettings
            {
                CloseInput = false, // caller will close the stream
                DtdProcessing = DtdProcessing.Prohibit,
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using var reader = XmlReader.Create(stream, readerSettings);
            var prefixStack = new Stack<string>();

            SkipUntilRootElement(reader);

            // We process the root element individually since it doesn't contribute to prefix
            ProcessAttributes(reader, prefixStack, data, AddNamePrefix);
            ProcessAttributes(reader, prefixStack, data, AddAttributePair);

            var preNodeType = reader.NodeType;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        prefixStack.Push(reader.LocalName);
                        ProcessAttributes(reader, prefixStack, data, AddNamePrefix);
                        ProcessAttributes(reader, prefixStack, data, AddAttributePair);

                        // If current element is self-closing
                        if (reader.IsEmptyElement)
                        {
                            prefixStack.Pop();
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (prefixStack.Any())
                        {
                            // If this EndElement node comes right after an Element node,
                            // it means there is no text/CDATA node in current element
                            if (preNodeType == XmlNodeType.Element)
                            {
                                var key = ConfigurationPath.Combine(prefixStack.Reverse());
                                data[key] = string.Empty;
                            }

                            prefixStack.Pop();
                        }
                        break;

                    case XmlNodeType.CDATA:
                    case XmlNodeType.Text:
                    {
                        var key = ConfigurationPath.Combine(prefixStack.Reverse());
                        if (data.ContainsKey(key))
                        {
                            throw new FormatException($"A duplicate key '{key}' was found.{GetLineInfo(reader)}");
                        }

                        data[key] = reader.Value;
                        break;
                    }
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.Comment:
                    case XmlNodeType.Whitespace:
                        // Ignore certain types of nodes
                        break;

                    default:
                        throw new FormatException($"Unsupported node type '{reader.NodeType}' was found.{GetLineInfo(reader)}");
                }
                preNodeType = reader.NodeType;
                // If this element is a self-closing element,
                // we pretend that we just processed an EndElement node
                // because a self-closing element contains an end within itself
                if (preNodeType == XmlNodeType.Element &&
                    reader.IsEmptyElement)
                {
                    preNodeType = XmlNodeType.EndElement;
                }
            }

            return data;
        }

        private static void SkipUntilRootElement(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.XmlDeclaration &&
                    reader.NodeType != XmlNodeType.ProcessingInstruction)
                {
                    break;
                }
            }
        }

        private static string GetLineInfo(XmlReader reader) =>
            reader is IXmlLineInfo lineInfo ? $"Line {lineInfo.LineNumber}, position {lineInfo.LinePosition}." : string.Empty;

        private static void ProcessAttributes(XmlReader reader, Stack<string> prefixStack, IDictionary<string, string> data,
            Action<XmlReader, Stack<string>, IDictionary<string, string>> act)
        {
            for (var i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                // If there is a namespace attached to current attribute
                if (!string.IsNullOrEmpty(reader.NamespaceURI))
                {
                    throw new FormatException($"XML namespaces are not supported.{GetLineInfo(reader)}");
                }

                act(reader, prefixStack, data);
            }

            // Go back to the element containing the attributes we just processed
            reader.MoveToElement();
        }

        // The special attribute "Name" only contributes to prefix
        // This method adds a prefix if current node in reader represents a "Name" attribute
        private static void AddNamePrefix(XmlReader reader, Stack<string> prefixStack,
            IDictionary<string, string> data)
        {
            if (!string.Equals(reader.LocalName, NameAttributeKey, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // If current element is not root element
            if (prefixStack.Any())
            {
                var lastPrefix = prefixStack.Pop();
                prefixStack.Push(ConfigurationPath.Combine(lastPrefix, reader.Value));
            }
            else
            {
                prefixStack.Push(reader.Value);
            }
        }

        // Common attributes contribute to key-value pairs
        // This method adds a key-value pair if current node in reader represents a common attribute
        private static void AddAttributePair(XmlReader reader, Stack<string> prefixStack,
            IDictionary<string, string> data)
        {
            prefixStack.Push(reader.LocalName);
            var key = ConfigurationPath.Combine(prefixStack.Reverse());
            if (data.ContainsKey(key))
            {
                throw new FormatException($"A duplicate key '{key}' was found.{GetLineInfo(reader)}");
            }

            data[key] = reader.Value;
            prefixStack.Pop();
        }
    }
}
