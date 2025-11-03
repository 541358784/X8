using System.ComponentModel;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(Easter)]
    [DisplayName("清理复活节活动")]
    public void ClearEaster()
    {
        HideDebugPanel();
        EasterModel.Instance.StorageEaster.Clear();
    }

    [Category(Easter)]
    [DisplayName("清理复活节礼包")]
    public void ClearEasterPack()
    {
        HideDebugPanel();
        EasterPackModel.Instance.StorageEasterPack.Clear();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "easterPack");
    }
    
    
    [Category(Easter)]
    [DisplayName("修改积分")]
    public int AddEasterValue
    {
        get
        {
            return EasterModel.Instance.GetScore();
        }
        set
        {
            EasterModel.Instance.AddScore(value);
        }
    }
    [Category(Easter)]
    [DisplayName("结束页面")]
    public void UIEasterEnd()
    {
        UIManager.Instance.OpenUI(UINameConst.UIEasterEnd, EasterModel.Instance.StorageEaster);
    }
        
    [Category(Easter)]
    [DisplayName("移除复活节物品")]
    public void RemoveEaster()
    {
        MergeManager.Instance.ReplaceEaster(MergeBoardEnum.Main);
    }
    [Category(Easter)]
    [DisplayName("清理复活节礼包 ")]
    public void Clear()
    {
        HideDebugPanel();
        EasterGiftModel.Instance.ClearPack();
    }
}