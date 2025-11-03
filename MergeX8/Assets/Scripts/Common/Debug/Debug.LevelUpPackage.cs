using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string LevelUpPackage = "升级礼包";

    [Category(LevelUpPackage)]
    [DisplayName("重制升级礼包")]
    public void CleanLevelUpPackage()
    {
        LevelUpPackageModel.Instance.Storage.Clear();
    }
    [Category(LevelUpPackage)]
    [DisplayName("开启的礼包id")]
    public string ActiveLevelUpPackageIdList
    {
        get
        {
            var text = "";
            for (var i = 0; i < LevelUpPackageModel.Instance.Storage.PackageList.Count; i++)
            {
                text += LevelUpPackageModel.Instance.Storage.PackageList[i].PackageId + ",";
            }
            return text;
        }
    }
}