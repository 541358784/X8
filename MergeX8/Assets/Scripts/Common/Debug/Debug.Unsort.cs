using System.ComponentModel;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Storage;
using Gameplay;

/// <summary>
/// 临时debug相关， 单元测试相关
/// </summary>
public partial class SROptions
{
    [Category(categoryAd)]
    [DisplayName("playerid")]
    public string playerid
    {
        get
        {
            return DragonU3DSDK.Utils.PlayerIdToString(StorageManager.Instance.GetStorage<StorageCommon>().PlayerId);
        }
    }

    [Category(categoryAd)]
    [DisplayName("DeviceInfo")]
    public string ANDROID_DEVICEINFO
    {
        get
        {
            return string.Format("ANDROID_ID:{0},DEVICEID:{1}", DragonU3DSDK.DragonNativeBridge.getAndroidID(),
                DragonU3DSDK.DragonNativeBridge.getDeivceId());
        }
    }

    [Category(categoryUI)]
    [DisplayName("广告-重置")]
    public void ResetAd()
    {
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        storageHome.AdData.Clear();
    }


    [Category(categoryUI)]
    [DisplayName("设置广告预测用户组")]
    public int ADUserGroup
    {
        get
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            return storageCommon.AdsPredictUserGroup;
        }
        set
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            storageCommon.AdsPredictUserGroup = value;
        }
    }


    [Category(categoryUI)]
    [DisplayName("Common 表id")]
    public string AdCommonID
    {
        get
        {
            Common common = AdConfigHandle.Instance.GetCommon();
            if (common == null)
                return "no get";

            return common.Id.ToString();
        }
    }
}