using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
    
    public abstract class IServiceProvider 
    {
        public PluginDefine m_PluginDefine = new PluginDefine();
        public abstract void DisposePlugin(string pluginName);
	}
}
