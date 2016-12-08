using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    public interface IProviderManager
    {
        string GetProperty(string name, string defaultValue);

        IProvider Provider(Type clazz);
    }
}
