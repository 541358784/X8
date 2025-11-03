using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ABTest;
using Activity.CrazeOrder.Model;
using Activity.GardenTreasure.View;
using Activity.JungleAdventure.Controller;
using Activity.LimitTimeOrder;
using Activity.LuckyGoldenEgg;
using Activity.LuckyGoldenEgg.Controller;
using Activity.Matreshkas.View;
using Activity.SaveTheWhales;
using Activity.TimeOrder;
using Activity.TotalRecharge;
using Activity.TreasureHuntModel;
using Activity.Turntable.Model;
using ActivityLocal.DecoBuildReward;
using Deco.World;
using Decoration;
using DG.Tweening;
using Difference;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Farm.Model;
using Framework;
using Gameplay;
using Gameplay.UI.EnergyTorrent;
using Manager;
using Merge.Order;
using TotalRecharge_New;
using UnityEngine;
using UnityEngine.UI;
using Type = System.Type;

public partial class MergeTaskTipsController : MonoBehaviour
{
    private Transform _limitTimeOrderGift = null;
    private LocalizeTextMeshProUGUI _limitTimeOrderText;
    
    public void InitLimitTimeOrder()
    {
        _limitTimeOrderGift = transform.Find("GiftRoot/LimitOrderGift");
        _limitTimeOrderText = transform.Find("GiftRoot/LimitOrderGift/FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _limitTimeOrderGift.gameObject.SetActive(false);
        
        _limitTimeOrderGift.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.OpenWindow(UINameConst.UIPopupLimitOrderGift);
        });
    }

    private void RefreshLimitTimeOrder()
    {
        if(_limitTimeOrderGift == null)
            return;
        
        if (LimitTimeOrderModel.Instance.IsOpened())
        {
            if (!LimitTimeOrderModel.Instance.IsLastOrder() || LimitTimeOrderModel.Instance.OrderId == 0 && LimitTimeOrderModel.Instance.LimitOrderLine.JoinEndTime == 0)
            {
                _limitTimeOrderGift.gameObject.SetActive(false);
            }
            else
            {
                _limitTimeOrderGift.gameObject.SetActive(LimitTimeOrderModel.Instance.LimitOrderLine.IsShowGift && !LimitTimeOrderModel.Instance.LimitOrderLine.IsBuyGift);
            }

            if(!_limitTimeOrderGift.gameObject.activeSelf)
                return;
        
            _limitTimeOrderText.SetText(LimitTimeOrderModel.Instance.GetJoinEndTimeString());
            return;
        }
        
        _limitTimeOrderGift.gameObject.SetActive(false);
        
    }
}
