using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string ServerTime = "0服务器时间";
    [Category(ServerTime)]
    [DisplayName("使用本地时间")]
    public bool UseLoacalTime
    {
        get;
        set;
    }
    [Category(ServerTime)]
    [DisplayName("验证本地时间")]
    public bool ValidateLoacalTime
    {
        get;
        set;
    }
    
    [Category(ServerTime)]
    [DisplayName("UI路径")]
    public string openUIPath
    {
        get;
        set;
    }
    
    
    [Category(ServerTime)]
    [DisplayName("打开UI")]
    public void OpenUI()
    {
        UIManager.Instance.OpenWindow(openUIPath);
    }
}