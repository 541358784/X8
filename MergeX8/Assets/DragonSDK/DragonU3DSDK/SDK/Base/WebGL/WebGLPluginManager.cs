using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Dlugin.PluginStructs;
#if UNITY_WEBGL
namespace Dlugin
{
    public class WebGLPluginManager : IConfigurablePluginManager
	{
        public void Configure(string configure)
        {
            Plugin_configure(configure);
        }
        public string Initialize()
        {
            string err = Plugin_initialize();
            if (!string.IsNullOrEmpty(err))
                return err;
            int count = 0;
            //add login plugin
            string pluginData = Plugin_getLoginPlugins();
            DebugUtil.Log("WebGLPluginManager.Init ----> get login plugin data:{0}", pluginData); 
            string[] plugins = Util.DecodeStructString(pluginData);
            for (var i = 0; i < plugins.Length; i += count)
            {
                count = AddPlugin(EServiceType.UserLogin, plugins, i);
            }
            //add os utility plugin
            pluginData = Plugin_getOSUtilPlugins();
            DebugUtil.Log("WebGLPluginManager.Init ----> get os util plugin data:{0}", pluginData); 
            plugins = Util.DecodeStructString(pluginData);
            for (var i = 0; i < plugins.Length; i += count)
            {
                count = AddPlugin(EServiceType.OSUtility, plugins, i);
            }
            //add login login
            pluginData = Plugin_getIAPPlugins();
            DebugUtil.Log("WebGLPluginManager.Init ----> get iap plugin data:{0}", pluginData); 
            plugins = Util.DecodeStructString(pluginData);
            for (var i = 0; i < plugins.Length; i += count)
            {
                count = AddPlugin(EServiceType.IAP, plugins, i);
            }

            PluginDefine[] pds = m_AllPlugins.Values.Select(pd => pd.allService.CheckContains(Constants.kPayService)).ToArray();
            //DebugUtil.Log("WebGLPluginManager.Init ----> collected the iap plugin:" + pds.ToStringEx("iapplugin", e => e.pluginId));
            m_IAPService = new WebGLIAPService(pds);
            pds = m_AllPlugins.Values.Select(pd => pd.allService.CheckContains(Constants.kOSUtilService)).ToArray();
            //DebugUtil.Log("WebGLPluginManager.Init ----> collected the os utility plugin:" + pds.ToStringEx("iapplugin", e => e.pluginId));
            m_OSUtility = new WebGLOSUtility(pds);
            pds = m_AllPlugins.Values.Select(pd => pd.allService.CheckContains(Constants.kUserLoginService)).ToArray();
            //DebugUtil.Log("WebGLPluginManager.Init ----> collected the user login plugin:" + pds.ToStringEx("iapplugin", e => e.pluginId));
            m_UserLogin = new WebGLUserLogin(pds);
            return err;
        }
        public void Dispose(string pluginId)
        {
            Plugin_dispose(pluginId);
            m_AllPlugins.Remove(pluginId);
            if (m_IAPService != null)
            {
                m_IAPService.DisposePlugin(pluginId);
                if (!m_IAPService.GetAllPlugin().GetEnumerator().MoveNext())
                    m_IAPService = null;
            }
            if (m_OSUtility != null)
            {
                m_OSUtility.DisposePlugin(pluginId);
                if (!m_OSUtility.GetAllPlugin().GetEnumerator().MoveNext())
                    m_OSUtility = null;
            }
            if (m_UserLogin != null)
            {
                m_UserLogin.DisposePlugin(pluginId);
                if (!m_UserLogin.GetAllPlugin().GetEnumerator().MoveNext())
                    m_UserLogin = null;
            }
        }
        public void DisposeAll()
        {
            m_IsDisposed = true;
            string[] allPlugin = m_AllPlugins.Keys.ToArrayEx();
            if (allPlugin != null)
            {
                for (int i = 0; i < allPlugin.Length; i++)
                {
                    Dispose(allPlugin[i]);
                }
            }
            Plugin_dispose("");
            m_AllPlugins.Clear ();
            m_IAPService = null;
            m_OSUtility = null;
            m_UserLogin = null;
        }
        public IEnumerable<IServiceProvider> GetServiceProviders()
        {
            List<IServiceProvider> services = new List<IServiceProvider>();
            if(m_IAPService != null)
                services.Add(m_IAPService);
            if(m_OSUtility != null)
                services.Add(m_OSUtility);
            if(m_UserLogin != null)
                services.Add(m_UserLogin);
            return services;
        }

		private int AddPlugin(EServiceType type, string[] str, int index)
		{
			int count = 0;
            PluginDefine define = null;
			if (str.Length > count + index) {
				string pluginId = str [count + index];
				define = GetOrCreatePluginDefine (pluginId);
                define.allService = define.allService.Append ((int)type);
				count++;
			}
			if (str.Length > count + index) {
				string pluginType = str [count + index];
				define.pluginType = pluginType;
				count++;
			}
			if (str.Length > count + index) {
				string pluginVersion = str [count + index];
				define.pluginVersion = pluginVersion;
				count++;
			}
            DebugUtil.Log("WebGLPluginManager.AddPlugin ----> add {3} type PluginDefine pluginId:{0}, pluginType:{1}, pluginVersion:{2}", define.pluginId, define.pluginType, define.pluginVersion, type);
			return count;
		}

		private PluginDefine GetOrCreatePluginDefine(string pluginId) 
		{
			PluginDefine ret = null;
			if (!m_AllPlugins.TryGetValue (pluginId, out ret)) {
                ret = new PluginDefine (pluginId, "", EPluginLocation.JavascriptCode, "");
				m_AllPlugins.Add (pluginId, ret);
			}
            return ret;
		}

		private Dictionary<string, PluginDefine> m_AllPlugins = new Dictionary<string, PluginDefine>();
		private WebGLIAPService m_IAPService = null;
		private WebGLOSUtility m_OSUtility = null;
		private WebGLUserLogin m_UserLogin = null;
        [DllImport("__Internal")]
        private static extern void Plugin_addMessageReceiver(string name);
        [DllImport("__Internal")]
        private static extern void Plugin_configure(string configure);
        [DllImport("__Internal")]
        private static extern string Plugin_initialize();
        [DllImport("__Internal")]
        private static extern string Plugin_dispose(string pluginId);
        [DllImport("__Internal")]
        private static extern void Plugin_removeMessageReceiver(string name);
        [DllImport("__Internal")]
		private static extern string Plugin_getLoginPlugins ();
        [DllImport("__Internal")]
		private static extern string Plugin_getIAPPlugins ();
        [DllImport("__Internal")]
		private static extern string Plugin_getOSUtilPlugins ();

        private bool m_IsDisposed = false;
	}
}
#endif