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
    private Transform _timeOrderGift = null;
    private LocalizeTextMeshProUGUI _timeOrderText;
    
    public void InitTimeOrder()
    {
        _timeOrderGift = transform.Find("GiftRoot/TimeOrderGift");
        _timeOrderText = transform.Find("GiftRoot/TimeOrderGift/FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _timeOrderGift.gameObject.SetActive(false);
        
        _timeOrderGift.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.OpenWindow(UINameConst.UIPopupTimeOrderGift);
        });
    }

    private void RefreshTimeOrder()
    {
        if(_timeOrderGift == null)
            return;
        
        if (TimeOrderModel.Instance.IsOpened())
        {
            if (TimeOrderModel.Instance.OrderId == 0 && TimeOrderModel.Instance.TimeOrder.JoinEndTime == 0)
            {
                _timeOrderGift.gameObject.SetActive(false);
            }
            else
            {
                _timeOrderGift.gameObject.SetActive(TimeOrderModel.Instance.TimeOrder.IsShowGift && !TimeOrderModel.Instance.TimeOrder.IsBuyGift);
            }

            if(!_timeOrderGift.gameObject.activeSelf)
                return;
        
            _timeOrderText.SetText(TimeOrderModel.Instance.GetJoinEndTimeString());
            return;
        }
        
        _timeOrderGift.gameObject.SetActive(false);
        
    }
}
