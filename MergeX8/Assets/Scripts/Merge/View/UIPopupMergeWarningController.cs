/****************************************************
    文件：UIPopupMergeWarningController.cs
    作者：Cys
    邮箱: yongsheng.chen@dragonplus.com
    日期： 2021-11-17-15:19:05
    功能：....
*****************************************************/

using System;
using DragonPlus;
using UnityEngine;

public class UIPopupMergeWarningController : UIWindow
{
    private MergePackageUnit packageUnit;
    private int itemId;
    public Action OnSellItem;
    private LocalizeTextMeshProUGUI soldText;

    public override void PrivateAwake()
    {
        BindClick("Root/ContentGroup/Button", (go) => { CloseWindowWithinUIMgr(true); });
        packageUnit = this.transform.GetComponentDefault<MergePackageUnit>("Root/ContentGroup/MergePackageUnit");
        packageUnit.SetBoardId(MergeBoardEnum.Main);
        BindClick("Root/ContentGroup/SellButton", OnClickSell);
        // soldText = packageUnit.transform.GetComponentDefault<LocalizeTextMeshProUGUI>("State1/Conversion/LevelText");
    }

    public void InitPackageUnit(int id)
    {
        itemId = id;
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return;
        packageUnit.SetItemInfomation(config, -1, MergePackageUnitType.warning);
        // soldText.SetText("x" + config.sold_gold.ToString());
    }

    private void OnClickSell(GameObject go)
    {
        var config = GameConfigManager.Instance.GetItemConfig(itemId);
        if (config == null)
            return;
        OnSellItem?.Invoke();
        CloseWindowWithinUIMgr(true);
    }
}