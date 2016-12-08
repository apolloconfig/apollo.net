using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    public interface IProvider
    {
        Type Type { get; }
        string Property(string name, string defaultValue);
        void Initialize();
    }
}
