using System;
using Com.Ctrip.Framework.Apollo.Core.Utils;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public interface RepositoryChangeListener
    {
        /// <summary>
        /// Invoked when config repository changes. </summary>
        /// <param name="namespaceName"> the namespace of this repository change </param>
        /// <param name="newProperties"> the properties after change </param>
        void OnRepositoryChange(string namespaceName, Properties newProperties);
    }
}

