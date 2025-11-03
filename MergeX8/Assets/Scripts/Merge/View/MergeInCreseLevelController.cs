/****************************************************
    文件：MergeInCreseLevelController.cs
    作者：Cys
    邮箱: yongsheng.chen@dragonplus.com
    日期： 2022-01-04-15:04:37
    功能：....
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Network.API;
public class UIPopupMergeIncreaseLevelController :  UIWindow
{
    private int itemId;
    private int itemId1;
    private bool isInit;
    private Button okBtn;
    private Button closeBtn;
    private float clickTime;
    public Action OnConfirm;
    public Action OnDismiss;
    public override void PrivateAwake()
    {
     
        okBtn = this.transform.GetComponentDefault<Button>("Root/ContentGroup/ButtonGroup/ButtonNew");
        okBtn.onClick.AddListener(OnConfirmClick);
        
        closeBtn = this.transform.GetComponentDefault<Button>("Root/ContentGroup/ButtonGroup/ButtonReturn");
        closeBtn.onClick.AddListener(OnCloseClick);
        
        closeBtn = this.transform.GetComponentDefault<Button>("Root/BgPopupBoand/ButtonClose");
        closeBtn.onClick.AddListener(OnCloseClick);
    }

    public void SetItemId(int id,int id1)// 物品id  万能物品id
    {
        if(isInit)
            return;
        
        var config1 = GameConfigManager.Instance.GetItemConfig(id);
        var script1 = this.transform.Find("Root/ContentGroup/MergePackageUnit1").gameObject.GetComponentDefault<MergePackageUnit>();
        script1.SetBoardId(MergeBoardEnum.Main);
        script1.SetItemInfomation(config1,-1,MergePackageUnitType.increasItem);
        
        var config2 = GameConfigManager.Instance.GetItemConfig(config1.next_level);
        var script2 = this.transform.Find("Root/ContentGroup/MergePackageUnit2").gameObject.GetComponentDefault<MergePackageUnit>();
        script2.SetBoardId(MergeBoardEnum.Main);
        script2.SetItemInfomation(config2,-1,MergePackageUnitType.increasItem);
        
        Image icon = okBtn?.transform.GetComponentDefault<Image>("ContentGroup/Props");
        if (icon != null)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(id1);
            icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig?.image);
        }
        isInit = true;
    }

    private void OnConfirmClick()
    {
        if(Time.time - clickTime<1)
            return;
        AnimCloseWindow(() =>
        {
            OnConfirm?.Invoke();
        });

        clickTime = Time.time;
    }

    public override bool OnBack()
    {
        ClickUIMask();
        return true;
    }
    public override void  ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        
        AnimCloseWindow(() =>
        {
            OnDismiss?.Invoke();
        });
    }
    private void OnCloseClick()
    {
        ClickUIMask();
    }
}
