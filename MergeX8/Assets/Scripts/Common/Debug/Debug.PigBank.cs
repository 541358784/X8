using System.ComponentModel;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(PiggyBank)]
    [DisplayName("重置小猪")]
    public void ResetPigBank()
    {
        HideDebugPanel();
        StoragePigBank _storageData = StorageManager.Instance.GetStorage<StorageHome>().PigBankData;
        _storageData.Indexs.Clear();
        _storageData.PigBankIds.Clear();
        _storageData.PigBankValue.Clear();
        _storageData.Clear();
    }


    [Category(PiggyBank)]
    [DisplayName("灌注增加")]
    public void AddCollectValue()
    {
        PigBankModel.Instance.AddCollectValue();
        EventDispatcher.Instance.DispatchEvent(EventEnum.PIGBANK_UI_REFRESH);
    }

    public bool isOpenPigBank = false;
    public bool isSetOpen = false;

    [Category(PiggyBank)]
    [DisplayName("开启 OR 关闭 小猪")]
    public bool DebugOpenPigBank
    {
        get
        {
            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
                return isOpenPigBank;
            return false;
        }
        set
        {
            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
            {
                isSetOpen = true;
                isOpenPigBank = value;
            }
        }
    }

    [Category(PiggyBank)]
    [DisplayName("显示小猪")]
    public void ShowPigBank()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupPigBox);
        HideDebugPanel();
    }
}