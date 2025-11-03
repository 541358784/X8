using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;
#if UNITY_WEBGL
namespace Dlugin
{
    public class WebGLServiceProvider : IServiceProvider
	{ 
		public WebGLServiceProvider(PluginDefine[] allDefine)
		{
			m_AllPlugins = new PluginDefine[allDefine.Length];
			for (int i = 0; i < allDefine.Length; i++) {
				m_AllPlugins [i] = allDefine [i];
			}
		}
            
		public IEnumerable<PluginDefine> GetAllPlugin()				//get all plugin that contains in the service provider
		{
			return m_AllPlugins;
		}
		public IEnumerable<PluginDefine> GetAllPluginFilted(PluginFilter filter)		//get all plugin with this filter
		{
			if (filter == null)
				return m_AllPlugins;
			List<PluginDefine> ret = new List<PluginDefine> ();
			for (int i = 0; i < m_AllPlugins.Length; i++) {
				if (filter.PassFilter (m_AllPlugins [i].pluginId))
					ret.Add (m_AllPlugins [i]);
			}
			return ret;
        }
        public bool CanPassFilter(PluginFilter filter)
        {
            if (filter == null)
                return true;
            for (int i = 0; i < m_AllPlugins.Length; i++)
            {
                if (filter.PassFilter(m_AllPlugins[i].pluginId))
                {
                    return true;
                }
            }
            return false;
        }
        public void DisposePlugin(string pluginId)
        {
            List<PluginDefine> newPlugin = null;
            for (var i = 0; i < m_AllPlugins.Length; i++)
            {
                if (m_AllPlugins[i].pluginId == pluginId)
                {
                    if (newPlugin == null)
                    {
                        newPlugin = new List<PluginDefine>();
                        for (var j = 0; j < i; j++)
                        {
                            newPlugin.Add(m_AllPlugins[j]);
                        }
                    }
                }
                else if(newPlugin != null)
                {
                    newPlugin.Add(m_AllPlugins[i]);
                }
            }
            if (newPlugin != null)
                m_AllPlugins = newPlugin.ToArray();
        }
		public PluginFilter PluginFilter{ get; set;}

		protected IEnumerator GetAllPluginFiltedIterator(PluginFilter filter)
		{
			for (int i = 0; i < m_AllPlugins.Length; i++) {
				if (filter == null || filter.PassFilter (m_AllPlugins [i].pluginId))
					yield return m_AllPlugins [i];
			}
		}

		protected PluginDefine[] m_AllPlugins;
	}
}
#endif