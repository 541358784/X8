using System.ComponentModel;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Storage;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;


public partial class SROptions
{
    private const string Dynamic = "关卡动态控制";

    [Category(Dynamic)]
    [DisplayName("国家-PlatformGroup")]
    public int PlatformGroup
    {
        get { return AdConfigHandle.Instance.GetPlatformGroup(); }
    }

    [Category(Dynamic)]
    [DisplayName("国家-是否美国")]
    public bool Country
    {
        get { return StorageManager.Instance.GetStorage<StorageCommon>().Country.ToUpper() == "US"; }
        set { StorageManager.Instance.GetStorage<StorageCommon>().Country = value ? "US" : "NoUS"; }
    }

    [Category(Dynamic)]
    [DisplayName("国家-是IOS")]
    public bool IsIos
    {
        get { return StorageManager.Instance.GetStorage<StorageCommon>().Platform == 0; }
        set { StorageManager.Instance.GetStorage<StorageCommon>().Platform = value ? 0 : 1; }
    }


    [Category(Dynamic)]
    [DisplayName("大分组-UserGroup")]
    public int UserGroup
    {
        get
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageHome>().AdData;
            return storageCommon.UserGroup;
        }
        // set
        // {
        //     var storageCommon = StorageManager.Instance.GetStorage<StorageHome>().AdData;
        //     storageCommon.UserGroup = value;
        // }
    }

    [Category(Dynamic)]
    [DisplayName("小分组-SubUserGroup")]
    public int SubUserGroup
    {
        get
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageHome>().AdData;
            return storageCommon.SubUserGroup;
        }
        // set
        // {
        //     var storageCommon = StorageManager.Instance.GetStorage<StorageHome>().AdData;
        //     storageCommon.SubUserGroup = value;
        // }
    }

    [Category(Dynamic)]
    [DisplayName("当前最大充值金额")]
    public float PayMaxAmount
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().PayMaxAmount; }
        set { StorageManager.Instance.GetStorage<StorageHome>().PayMaxAmount = value; }
    }
}