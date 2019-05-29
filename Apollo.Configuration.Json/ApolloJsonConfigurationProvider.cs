using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Internals;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloJsonConfigurationProvider : ApolloConfigurationProvider
    {
        public ApolloJsonConfigurationProvider(string sectionKey, IConfigRepository configRepository)
            : base(sectionKey, configRepository)
        {
        }

        protected override void SetData(Properties properties)
        {
            // json 类型的应该是只有一个 content 的值
            if (properties.Source == null || properties.Source.Count == 0)
            {
                Data = new Dictionary<string, string>();
                return;
            }
            
            var json = properties.GetProperty("content");
            Data = JsonConfigurationParser.Parse(json, _sectionKey);
        }
    }
}