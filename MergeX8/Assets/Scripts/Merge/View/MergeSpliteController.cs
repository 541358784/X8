/****************************************************
    文件：MergeSpliteController.cs
    作者：Cys
    邮箱: yongsheng.chen@dragonplus.com
    日期： 2021-11-23-10:19:27
    功能：....
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class MergeSpliteController : UIWindow
{
    private MergePackageUnit unit1, unit2, unit3;
    public Action<bool> onSpliteCallBack;
    private Button diamondsBtn;
    private float lastClickTime;
    private int itemId;
    private bool isInit;
    private LocalizeTextMeshProUGUI diamonds;
    private GameObject rvBtn;

    public override void PrivateAwake()
    {
    }
    // protected override void OnInitWindowProperty(ref UIWindowProperty property)
    // {
    //     base.OnInitWindowProperty(ref property);
    //     property.bgMaskMode = UIWindowBgMaskMode.Black;
    // }

    private void Init()
    {
        if (isInit)
            return;
        BindClick("Root/BGGroup/CloseButton", (go) => { CloseWindowWithinUIMgr(true); });
        Transform contentGroup = this.transform.Find("Root/ContentGroup");
        unit1 = contentGroup.GetComponentDefault<MergePackageUnit>("MergePackageUnit");
        unit1.SetBoardId(MergeBoardEnum.Main);
        unit2 = contentGroup.GetComponentDefault<MergePackageUnit>("MergePackageUnit1");
        unit2.SetBoardId(MergeBoardEnum.Main);
        unit3 = contentGroup.GetComponentDefault<MergePackageUnit>("MergePackageUnit2");
        unit3.SetBoardId(MergeBoardEnum.Main);
        diamondsBtn = contentGroup.GetComponentDefault<Button>("DiamondButton");
        rvBtn = contentGroup.Find("RvButton").gameObject;
        diamondsBtn.onClick.AddListener(OnDiamondsClick);
        diamonds = diamondsBtn.GetComponentDefault<LocalizeTextMeshProUGUI>("ContentGroup/Text");
        isInit = true;
        // UIAdRewardButton.Create(eAdReward.E_SplitItem, UIAdRewardButton.ButtonStyle.Disable,rvBtn,
        //     (s, r) =>
        //     {
        //         if (s)
        //         {
        //             int gridIndex = MergeManager.Instance.FindEmptyGrid(1);
        //             if (gridIndex == -1)
        //             {
        //                 TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text22"), rvBtn.transform);
        //             }
        //             else
        //             {
        //                 CloseWindowWithinUIMgrNew(true,true, () =>
        //                 { 
        //                     onSpliteCallBack?.Invoke(true);
        //                 });
        //             }
        //          
        //         }
        //     },onBtnClick: () =>
        //     {
        //     });
    }

    public void SetItemId(int id)
    {
        Init();
        this.itemId = id;
        var lastLevelItem = MergeConfigManager.Instance.GetLastLevelItemConfig(itemId);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(itemId);
        unit1.SetItemInfomation(itemConfig, -1, MergePackageUnitType.splite);
        unit2.SetItemInfomation(lastLevelItem, -1, MergePackageUnitType.splite);
        unit3.SetItemInfomation(lastLevelItem, -1, MergePackageUnitType.splite);
        diamonds.SetText(itemConfig.Gem_split.ToString());
    }

    private void OnDiamondsClick()
    {
        var itemConfig = GameConfigManager.Instance.GetItemConfig(itemId);
        if (itemConfig == null)
            return;
        // if (HotelDiamondModel.Instance.DiamondNumber() < itemConfig.Gem_split)
        // {
        //     if(UnlockManager.IsOpen(UnlockManager.MergeUnlockType.shop))
        //         UIManager.Instance.OpenWindowNew<ShopController>(UIWindowType.PopupTip, true).comingIndex = 4;
        //     else
        //         TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text14"), diamondsBtn.transform);
        //     return;
        // }
        int gridIndex = MergeManager.Instance.FindEmptyGrid(1,MergeBoardEnum.Main);
        if (gridIndex == -1)
        {
            //TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text22"), diamondsBtn.transform);
            return;
        }

        // CloseWindowWithinUIMgrNew(true,true, () =>
        // { 
        //     onSpliteCallBack?.Invoke(false);
        // });
        CloseWindowWithinUIMgr(true, () => { onSpliteCallBack?.Invoke(false); });
    }
}