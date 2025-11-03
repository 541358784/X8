using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Dlugin.PluginStructs;

#if UNITY_WEBGL
namespace Dlugin
{
	public class WebGLOSUtility : WebGLServiceProvider, IOSUtility
	{
		[DllImport("__Internal")]
		private static extern string Plugin_getOSVersion (string id);
		[DllImport("__Internal")]
		private static extern string Plugin_getDeviceModel (string id);
		[DllImport("__Internal")]
		private static extern string Plugin_getDeviceID (string id);

		public WebGLOSUtility (PluginDefine[] allDefine) : base (allDefine)
		{
		}

		#region IOSUtility
		public string GetOSVersion()
		{
			IEnumerator pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			if (pluginIter.MoveNext ())
				return Plugin_getOSVersion ((pluginIter.Current as PluginDefine).pluginId);
			else
				return "";
		}
		public string GetDeviceModel()
		{
			IEnumerator pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			if (pluginIter.MoveNext ())
				return Plugin_getDeviceModel ((pluginIter.Current as PluginDefine).pluginId);
			else
				return "";
		}
		public string GetDeviceID()
		{
			IEnumerator pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			if (pluginIter.MoveNext ())
				return Plugin_getDeviceID ((pluginIter.Current as PluginDefine).pluginId);
			else
				return "";
		}
		#endregion
	}
}
#endif