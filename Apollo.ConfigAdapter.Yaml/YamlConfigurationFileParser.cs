using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Com.Ctrip.Framework.Apollo.ConfigAdapter
{
    internal class YamlConfigurationFileParser
    {
        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath = "";

        public IDictionary<string, string> Parse(TextReader reader)
        {
            _data.Clear();

            var yamlStream = new YamlStream();
            yamlStream.Load(reader);

            if (yamlStream.Documents.Count > 0 && yamlStream.Documents[0].RootNode is YamlMappingNode mappingNode)
                foreach (var node in mappingNode.Children)
                    if (node.Key is YamlScalarNode ysn && ysn.Value != null)
                        VisitYamlNode(ysn.Value, node.Value);
                    else
                        throw UnsupportedKeyType(node.Key, _currentPath);

            return _data;
        }

        private void VisitYamlNode(string context, YamlNode node)
        {
            switch (node)
            {
                case YamlScalarNode scalarNode:
                    VisitYamlScalarNode(context, scalarNode);
                    break;
                case YamlMappingNode mappingNode:
                    VisitYamlMappingNode(context, mappingNode);
                    break;
                case YamlSequenceNode sequenceNode:
                    VisitYamlSequenceNode(context, sequenceNode);
                    break;
                default:
                    throw UnsupportedNodeType(node, _currentPath);
            }
        }

        private static Exception UnsupportedKeyType(YamlNode node, string path) =>
            new FormatException($"Unsupported YAML key type '{node.GetType().Name} was found. Path '{path}', line {node.Start.Line} position {node.Start.Column}.");

        private static Exception UnsupportedNodeType(YamlNode node, string path) =>
            new FormatException($"Unsupported YAML node type '{node.GetType().Name} was found. Path '{path}', line {node.Start.Line} position {node.Start.Column}.");

        private static Exception UnsupportedMergeKeys(YamlNode node, YamlNode parent, string path) =>
            new FormatException($"Unsupported YAML merge keys '{node.GetType().Name} was found. Path '{path}', line {parent.Start.Line} position {parent.Start.Column}.");

        private void VisitYamlScalarNode(string context, YamlScalarNode scalarNode)
        {
            EnterContext(context);

            if (_data.ContainsKey(_currentPath)) throw new FormatException($"A duplicate key '{_currentPath}' was found.");

            _data[_currentPath] = IsNullValue(scalarNode) ? "" : scalarNode.Value!;

            ExitContext();
        }

        private void VisitYamlMappingNode(string context, YamlMappingNode mappingNode)
        {
            EnterContext(context);

            var dic = new Dictionary<string, YamlNode>();

            void VisitMergeKeys(YamlMappingNode refer)
            {
                foreach (var node in refer.Children)
                {
                    if (!(node.Key is YamlScalarNode ysn)) throw UnsupportedKeyType(node.Key, _currentPath);

                    if (ysn.Value == "<<")
                        switch (node.Value)
                        {
                            case YamlMappingNode ymn:
                                VisitMergeKeys(ymn);
                                break;
                            case YamlSequenceNode sequenceNode:
                                foreach (var item in sequenceNode.Children)
                                    if (item is YamlMappingNode ymn)
                                        VisitMergeKeys(ymn);
                                    else
                                        throw UnsupportedMergeKeys(item, sequenceNode, _currentPath);
                                break;
                            default:
                                throw UnsupportedMergeKeys(node.Value, node.Value, _currentPath);
                        }
                    else if (ysn.Value != null)
                        dic[ysn.Value] = node.Value;
                }
            }

            VisitMergeKeys(mappingNode);

            foreach (var node in dic) VisitYamlNode(node.Key, node.Value);

            ExitContext();
        }

        private void VisitYamlSequenceNode(string context, YamlSequenceNode sequenceNode)
        {
            EnterContext(context);

            for (var index = 0; index < sequenceNode.Children.Count; index++)
                VisitYamlNode(index.ToString(), sequenceNode.Children[index]);

            ExitContext();
        }

        private void EnterContext(string context)
        {
            _context.Push(context);

            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();

            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private static bool IsNullValue(YamlScalarNode yamlValue) =>
            yamlValue.Style == ScalarStyle.Plain
                && (yamlValue.Value == "~"
                    || yamlValue.Value == null
                    || yamlValue.Value == "null"
                    || yamlValue.Value == "Null"
                    || yamlValue.Value == "NULL");
    }
}
