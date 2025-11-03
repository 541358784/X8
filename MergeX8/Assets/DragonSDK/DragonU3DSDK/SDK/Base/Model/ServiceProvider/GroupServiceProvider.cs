using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
    public class GroupServiceProvider  
    {
        public void AddService(IServiceProvider provider)
        {
            if (provider == null)
                return;
            if (!m_AllService.Contains(provider))
            {
                m_AllService.Add(provider);
            }
        }

        public virtual void DisposePlugin(string pluginId)
        {
            for (int i = m_AllService.Count - 1; i >= 0; i--)
            {
                m_AllService[i].DisposePlugin(pluginId);
                m_AllService.RemoveAt(i);
            }
        }

        public override string ToString() 
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{\n");

            foreach (IServiceProvider provider in m_AllService)
            {
                sb.AppendFormat("\tpluginId:{0}, pluginType:{1}, pluginPosition:{2}, version:{3}\n", provider.m_PluginDefine.m_PluginParam, provider.m_PluginDefine.m_PluginName, provider.m_PluginDefine.m_PluginVersion);
            }
            sb.Append("}\n");
            return sb.ToString();
        }

      

        public List<IServiceProvider> m_AllService = new List<IServiceProvider>();
    }
}
