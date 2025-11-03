using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dlugin
{
    public interface IConfigurablePluginManager : IPluginManager
    {
        void Configure(string configure);
    }
}
