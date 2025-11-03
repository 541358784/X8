using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dlugin
{
    public interface IPluginManager
    {
        string Initialize();
        IEnumerable<IServiceProvider> GetServiceProviders(int service);
        void Dispose(string pluginId);
        void DisposeAll();
    }
}
