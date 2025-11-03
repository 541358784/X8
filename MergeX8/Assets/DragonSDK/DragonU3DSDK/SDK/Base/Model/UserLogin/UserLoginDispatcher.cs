using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin.PluginStructs;

namespace Dlugin
{
	public class UserLoginDispatcher 
    {
		private static UserLoginDispatcher s_Instance = new UserLoginDispatcher ();
		public static UserLoginDispatcher GetInstance()
		{
			return s_Instance;
		}

		public delegate void UserLoginCallback(string pluginId,UserInfo userInfo, SDKError error);

        public event UserLoginCallback onLoginOver;
		public event UserLoginCallback onLogoutOver;
		public event UserLoginCallback onRefreshUserInfoOver;
		public event UserLoginCallback onChangePermissionOver;
		public event UserLoginCallback onReauthAccessTokenOver;

		public void ProcessLoginOver(string pluginId, UserInfo userInfo, SDKError error)
        {
            if (onLoginOver != null)
            {
                onLoginOver(pluginId, userInfo, error);
            }
		}

		public void ProcessLogoutOver(string pluginId, UserInfo userInfo, SDKError error)
        {
            if(onLogoutOver != null)
			    onLogoutOver (pluginId, userInfo, error);
		}

		public void ProcessChangePermissionOver(string pluginId, UserInfo userInfo, SDKError error)
        {
            if(onChangePermissionOver != null)
			    onChangePermissionOver (pluginId, userInfo, error);
		}

		public void ProcessRefreshUserInfoOver(string pluginId, UserInfo userInfo, SDKError error)
        {
            if(onRefreshUserInfoOver != null)
			    onRefreshUserInfoOver (pluginId, userInfo, error);
		}

		public void ProcessReauthAccessTokenOver(string pluginId, UserInfo userInfo, SDKError error)
        {
			if (onReauthAccessTokenOver != null)
				onReauthAccessTokenOver(pluginId, userInfo, error);
		}
	}
}
