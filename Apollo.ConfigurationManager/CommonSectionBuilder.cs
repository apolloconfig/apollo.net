using System.Configuration;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using static Com.Ctrip.Framework.Apollo.ConfigExtensions;

namespace Com.Ctrip.Framework.Apollo;

public class CommonSectionBuilder : ApolloConfigurationBuilder
{
    private static readonly Action<ConfigurationElementCollection, ConfigurationElement> Add;
    private static readonly Func<ConfigurationElementCollection, ConfigurationElement> CreateNewElement;
    private static readonly Action<ConfigurationElement, ConfigurationProperty, string> SetValue;
    private string? _keyPrefix;

    static CommonSectionBuilder()
    {
        var set = typeof(ConfigurationElement).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(p =>
            {
                var ip = p.GetIndexParameters();

                return ip.Length == 1 && ip[0].ParameterType == typeof(ConfigurationProperty);
            })
            .SetMethod;

        var convertFromString = typeof(ConfigurationProperty).GetMethod("ConvertFromString", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var createNewElement = typeof(ConfigurationElementCollection).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(m => m.Name == nameof(CreateNewElement) && m.GetParameters().Length == 0);

        var param0 = Expression.Parameter(typeof(ConfigurationElementCollection));
        var param1 = Expression.Parameter(typeof(ConfigurationElement));
        var param2 = Expression.Parameter(typeof(ConfigurationProperty));
        var param3 = Expression.Parameter(typeof(string));

        Add = CreateAddMethod();

        CreateNewElement = Expression.Lambda<Func<ConfigurationElementCollection, ConfigurationElement>>(
            Expression.Call(param0, createNewElement), param0).Compile();

        SetValue = Expression.Lambda<Action<ConfigurationElement, ConfigurationProperty, string>>(
            Expression.Call(param1, set, param2, Expression.Call(param2, convertFromString, param3)),
            param1, param2, param3).Compile();
    }

    public override void Initialize(string name, NameValueCollection config)
    {
        base.Initialize(name, config);

        _keyPrefix = config["keyPrefix"]?.TrimEnd(':');
    }

    public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
    {
        Bind(configSection, GetConfig(), string.IsNullOrWhiteSpace(_keyPrefix ??= configSection.SectionInformation.SectionName)
            ? new("", "")
            : new ConfigKey(_keyPrefix!.Substring(_keyPrefix!.LastIndexOf(':') + 1), _keyPrefix));

        return configSection;
    }

    private static void Bind(ConfigurationElement configElement, IConfig config, ConfigKey configKey)
    {
        if (string.IsNullOrWhiteSpace(configKey.FullName) &&
            configElement is not ConfigurationSection) return;

        foreach (var property in configElement.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var cpa = property.GetCustomAttribute<ConfigurationPropertyAttribute>();
            if (cpa == null) continue;

            var key = string.IsNullOrWhiteSpace(configKey.FullName) ? cpa.Name : $"{configKey.FullName}:{cpa.Name}";

            if (typeof(ConfigurationElement).IsAssignableFrom(property.PropertyType))
            {
                var element = property.GetValue(configElement);
                if (element == null) property.SetValue(configElement, element = Activator.CreateInstance(property.PropertyType));

                if (element is ConfigurationElementCollection cec)
                {
                    foreach (var child in config.GetChildren(key))
                    {
                        var ele = CreateNewElement(cec);

                        Bind(ele, config, child);

                        Add(cec, ele);
                    }
                }
                else if (element is ConfigurationElement ce) Bind(ce, config, new(cpa.Name, key));
            }
            else
            {
                var cp = new ConfigurationProperty(cpa.Name, property.PropertyType, cpa.DefaultValue, cpa.Options);

                ExceptionDispatchInfo? ex = null;

                if (cpa.IsKey && !string.IsNullOrWhiteSpace(configKey.Name))
                    try
                    {
                        SetValue(configElement, cp, configKey.Name);
                    }
                    catch (ConfigurationErrorsException e)
                    {
                        ex = ExceptionDispatchInfo.Capture(e);
                    }
                else if (string.Equals("value", cpa.Name, StringComparison.OrdinalIgnoreCase) &&
                         !string.IsNullOrWhiteSpace(configKey.FullName) &&
                         config.TryGetProperty(configKey.FullName, out var value))
                    try
                    {
                        SetValue(configElement, cp, value);
                    }
                    catch (ConfigurationErrorsException e)
                    {
                        ex = ExceptionDispatchInfo.Capture(e);
                    }

                if (ex == null)
                {
                    if (config.TryGetProperty(key, out var value))
                        SetValue(configElement, cp, value);
                }
                else if (config.TryGetProperty(key, out var value))
                    SetValue(configElement, cp, value);
                else ex.Throw();
            }
        }
    }

    private static Action<ConfigurationElementCollection, ConfigurationElement> CreateAddMethod()
    {
        var methods = typeof(ConfigurationElementCollection)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        MethodInfo GetMethod(string method, params Type[] parameterTypes)
        {
            var ms = methods.Where(m => m.Name == method).ToArray();
            if (parameterTypes.Length < 1) return ms.FirstOrDefault(m => m.GetParameters().Length == 0) ?? ms.First();

            return ms.First(m =>
            {
                var p = m.GetParameters();

                if (p.Length != parameterTypes.Length) return false;

                return !p.Where((t, index) => t.ParameterType != parameterTypes[index]).Any();
            });
        }

        var method = new DynamicMethod("Add", typeof(void),
            new[] { typeof(ConfigurationElementCollection), typeof(ConfigurationElement) },
            typeof(CommonSectionBuilder).Module, true);

        var il = method.GetILGenerator();

        //BaseRemove(GetElementKey(element));
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Callvirt, GetMethod("GetElementKey"));
        il.Emit(OpCodes.Call, GetMethod("BaseRemove", typeof(object)));

        //BaseAdd(element);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Callvirt, GetMethod("BaseAdd", typeof(ConfigurationElement)));

        il.Emit(OpCodes.Ret);

        return (Action<ConfigurationElementCollection, ConfigurationElement>)method.CreateDelegate(typeof(Action<ConfigurationElementCollection, ConfigurationElement>));
    }
}
