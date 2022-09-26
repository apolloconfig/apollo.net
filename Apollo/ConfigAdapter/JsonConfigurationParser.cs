#if NETFRAMEWORK
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
#endif

namespace Com.Ctrip.Framework.Apollo.ConfigAdapter;

internal class JsonConfigurationParser
{
    private JsonConfigurationParser() { }

    private readonly IDictionary<string, string> _data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private readonly Stack<string> _context = new();
#if NETFRAMEWORK
    private string _currentPath = string.Empty;

    private JsonTextReader? _reader;

    public static IDictionary<string, string> Parse(string input) => new JsonConfigurationParser().ParseString(input);

    private IDictionary<string, string> ParseString(string input)
    {
        _reader = new(new StringReader(input))
        {
            DateParseHandling = DateParseHandling.None
        };

        var jsonConfig = JObject.Load(_reader);

        VisitJObject(jsonConfig);

        return _data;
    }

    private void VisitJObject(JObject jObject)
    {
        foreach (var property in jObject.Properties())
        {
            EnterContext(property.Name);
            VisitProperty(property);
            ExitContext();
        }
    }

    private void VisitProperty(JProperty property)
    {
        VisitToken(property.Value);
    }

    private void VisitToken(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                VisitJObject(token.Value<JObject>());
                break;

            case JTokenType.Array:
                VisitArray(token.Value<JArray>());
                break;

            case JTokenType.Integer:
            case JTokenType.Float:
            case JTokenType.String:
            case JTokenType.Boolean:
            case JTokenType.Bytes:
            case JTokenType.Raw:
            case JTokenType.Null:
                VisitPrimitive(token.Value<JValue>());
                break;

            default:
                throw new FormatException($"Unsupported JSON token '{_reader!.TokenType}' was found. Path '{_reader.Path}', line {_reader.LineNumber} position {_reader.LinePosition}.");
        }
    }

    private void VisitArray(JArray array)
    {
        for (var index = 0; index < array.Count; index++)
        {
            EnterContext(index.ToString());
            VisitToken(array[index]);
            ExitContext();
        }
    }

    private void VisitPrimitive(JValue data)
    {
        var key = _currentPath;

        if (_data.ContainsKey(key))
        {
            throw new FormatException($"A duplicate key '{key}' was found.");
        }
        _data[key] = data.ToString(CultureInfo.InvariantCulture);
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
#else
    public static IDictionary<string, string> Parse(string input)
        => new JsonConfigurationParser().ParseStream(input);

    private IDictionary<string, string> ParseStream(string input)
    {
        var jsonDocumentOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        using (var doc = JsonDocument.Parse(input, jsonDocumentOptions))
        {
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                throw new FormatException($"Top-level JSON element must be an object. Instead, '{doc.RootElement.ValueKind}' was found.");

            VisitObjectElement(doc.RootElement);
        }

        return _data;
    }

    private void VisitObjectElement(JsonElement element)
    {
        var isEmpty = true;

        foreach (var property in element.EnumerateObject())
        {
            isEmpty = false;
            EnterContext(property.Name);
            VisitValue(property.Value);
            ExitContext();
        }

        SetNullIfElementIsEmpty(isEmpty);
    }

    private void VisitArrayElement(JsonElement element)
    {
        var index = 0;

        foreach (var arrayElement in element.EnumerateArray())
        {
            EnterContext(index.ToString());
            VisitValue(arrayElement);
            ExitContext();
            index++;
        }

        SetNullIfElementIsEmpty(isEmpty: index == 0);
    }

    private void SetNullIfElementIsEmpty(bool isEmpty)
    {
        if (isEmpty && _context.Count > 0)
        {
            _data[_context.Peek()] = "";
        }
    }

    private void VisitValue(JsonElement value)
    {
        Debug.Assert(_context.Count > 0);

        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                VisitObjectElement(value);
                break;

            case JsonValueKind.Array:
                VisitArrayElement(value);
                break;

            case JsonValueKind.Number:
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                var key = _context.Peek();

                if (_data.ContainsKey(key))
                    throw new FormatException($"A duplicate key '{key}' was found.");

                _data[key] = value.ToString();
                break;

            default:
                throw new FormatException($"Unsupported JSON token '{value.ValueKind}' was found.");
        }
    }

    private void EnterContext(string context) =>
        _context.Push(_context.Count > 0 ? _context.Peek() + ":" + context : context);

    private void ExitContext() => _context.Pop();
#endif
}
