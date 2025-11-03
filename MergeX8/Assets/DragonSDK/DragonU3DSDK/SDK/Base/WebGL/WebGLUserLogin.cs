using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Dlugin.PluginStructs;
#if UNITY_WEBGL
namespace Dlugin
{
	public class WebGLUserLogin : WebGLServiceProvider, IUserLogin
	{
		[DllImport("__Internal")]
		private static extern string Plugin_getUserInfo (string id);
		[DllImport("__Internal")]
		private static extern void Plugin_login (string permissions, string id, string context);
		[DllImport("__Internal")]
		private static extern void Plugin_refreshUserInfo (string id, string context);
		[DllImport("__Internal")]
		private static extern void Plugin_changePermission (string permissions, string id, string context);
		[DllImport("__Internal")]
		private static extern void Plugin_logout (string id, string context);
		[DllImport("__Internal")]
		private static extern int Plugin_getUserLoginStatus (string id);

		public WebGLUserLogin(PluginDefine[] allDefine) : base(allDefine) 
		{
		}

		public UserInfo GetUserInfo()
		{
			var pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			while (pluginIter.MoveNext ()) {
				PluginDefine def = pluginIter.Current as PluginDefine;
				if (def != null) {
					string ret = Plugin_getUserInfo (def.pluginId);
                    return JsonUtility.FromJson<UserInfo>(ret);
				}
			}
			SDK.FormatWarning ("WebGLUserLogin.GetUserInfo ---> get user info but no pluging supply this");
			return null;
		}
		public int GetUserLoginStatus()			//see login status in constant
		{
			var pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			while (pluginIter.MoveNext ()) {
				PluginDefine def = pluginIter.Current as PluginDefine;
				if (def != null) {
					return Plugin_getUserLoginStatus (def.pluginId);
				}
			}
			SDK.FormatWarning ("WebGLUserLogin.GetUserLoginStatus ---> get user status but no pluging supply this");
			return Constants.kLoginStatusUnknown;
		}
		public void Login(string context, int[] allPermission)
		{
			var pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			string[] allPermissionStr = new string[allPermission.Length];
			for (var i = 0; i < allPermission.Length; i++) {
				allPermissionStr[i] = allPermission [i].ToString();
			}
			while (pluginIter.MoveNext ()) {
				PluginDefine def = pluginIter.Current as PluginDefine;
				if (def != null) {
					Plugin_login (string.Join(",", allPermissionStr) ,def.pluginId, context);
					return;
				}
			}
			SDK.FormatWarning ("WebGLUserLogin.Login ---> login but no pluging supply this");
		}
		public void RefreshUserInfo(string context)
		{
			var pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			while (pluginIter.MoveNext ()) {
				PluginDefine def = pluginIter.Current as PluginDefine;
				if (def != null) {
					Plugin_refreshUserInfo (def.pluginId, context);
					return;
				}
			}
			SDK.FormatWarning ("WebGLUserLogin.RefreshUserInfo ---> refresh user info but no pluging supply this");
		}
		public void ChangePermission(string context, int[] allPermission)
		{
			var pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			string[] allPermissionStr = new string[allPermission.Length];
			for (var i = 0; i < allPermission.Length; i++) {
				allPermissionStr[i] = allPermission [i].ToString();
			}
			while (pluginIter.MoveNext ()) {
				PluginDefine def = pluginIter.Current as PluginDefine;
				if (def != null) {
					Plugin_changePermission (string.Join(",", allPermissionStr), def.pluginId, context);
					return;
				}
			}
			SDK.FormatWarning ("WebGLUserLogin.ChangePermission ---> change user permission but no pluging supply this");
		}
		public void Logout(string context)
		{
			var pluginIter = GetAllPluginFiltedIterator (PluginFilter);
			while (pluginIter.MoveNext ()) {
				PluginDefine def = pluginIter.Current as PluginDefine;
				if (def != null) {
					Plugin_logout (def.pluginId, context);
					return;
				}
			}
			SDK.FormatWarning ("WebGLUserLogin.Logout ---> logout but no pluging supply this");
		}
	}
}
#endif