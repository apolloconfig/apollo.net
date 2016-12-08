using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using Com.Ctrip.Framework.Apollo.Core.Ioc.LightInject;
using Com.Ctrip.Framework.Apollo.Core.Ioc.Utility;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc.Extensions.Annotation
{
    internal class NamedTypeExtractor : ITypeExtractor
    {
        public Type[] Execute(System.Reflection.Assembly assembly)
        {
            var targetNamespaces = NamespaceList.Create();
            var resourceNames = assembly.GetManifestResourceNames().Where(n => n.EndsWith("VenusIoc.config"));
            foreach (var resourceName in resourceNames)
            {
                var xmlDoc = new XmlDocument();
                using (var sr = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                {
                    xmlDoc.Load(sr);
                    foreach (var node in xmlDoc.DocumentElement.SelectNodes("components/assemblyScan/namespace"))
                    {
                        var name = ((XmlElement)node).GetAttribute("name");
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            targetNamespaces.Add(name.Split('.'));
                        }
                    }
                }
            }

            if (targetNamespaces.Count == 0)
                return new Type[0];

            var types = new List<Type>();
            var checkList = new HashSet<string>();
            var ignoreList = new HashSet<string>();
            foreach (var type in assembly.GetTypes())
            {
                bool toCheck = false;
                if (!ignoreList.Contains(type.Namespace) && type.Namespace != null)
                {
                    if (checkList.Contains(type.Namespace))
                    {
                        toCheck = true;
                    }
                    else
                    {
                        if (targetNamespaces.Include(type.Namespace.Split('.')))
                        {
                            checkList.Add(type.Namespace);
                            toCheck = true;
                        }
                        else
                        {
                            ignoreList.Add(type.Namespace);
                        }
                    }
                }

                if (toCheck)
                {
                    if (!type.IsAbstract && type.IsDefined(typeof(NamedAttribute), false))
                    {
                        types.Add(type);
                    }
                }
            }

            return types.ToArray();
        }
    }
}
