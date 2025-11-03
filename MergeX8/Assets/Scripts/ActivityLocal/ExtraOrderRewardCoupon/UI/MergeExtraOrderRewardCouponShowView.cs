using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class MergeExtraOrderRewardCouponShowView: MonoBehaviour
{
    private Image Icon;
    private LocalizeTextMeshProUGUI TimeText;
    private Button Btn;
    private List<StorageExtraOrderRewardCouponItem> CurCouponList =>
        ExtraOrderRewardCouponModel.Instance.Storage.CurCouponList;
    private void Awake()
    {
        Icon = transform.Find("Icon").GetComponent<Image>();
        TimeText = transform.Find("TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0f,1f);
        Btn = transform.GetComponent<Button>();
        Btn.onClick.AddListener(OnClickBtn);
        EventDispatcher.Instance.AddEvent<EventExtraOrderRewardCouponStart>(OnStartCoupon);
        EventDispatcher.Instance.AddEvent<EventExtraOrderRewardCouponEnd>(OnEndCoupon);
        UpdateViewState();
    }

    public void OnStartCoupon(EventExtraOrderRewardCouponStart evt)
    {
        UpdateViewState();
    }
    public void OnEndCoupon(EventExtraOrderRewardCouponEnd evt)
    {
        UpdateViewState();
    }

    public void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventExtraOrderRewardCouponStart>(OnStartCoupon);
        EventDispatcher.Instance.RemoveEvent<EventExtraOrderRewardCouponEnd>(OnEndCoupon);
    }

    public void UpdateViewState()
    {
        gameObject.SetActive(CurCouponList.Count > 0);
        if (gameObject.activeSelf)
        {
            var coupon = CurCouponList[0];
            var config = ExtraOrderRewardCouponModel.Instance.Config[coupon.CouponId];
            var couponType = (ExtraOrderRewardCouponType)config.multiType[0];
            if (couponType == ExtraOrderRewardCouponType.ClimbTree)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(ClimbTreeModel._climbTreeBananaId);
                Icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else if (couponType == ExtraOrderRewardCouponType.Coin)
            {
                Icon.sprite = UserData.GetResourceIcon(UserData.ResourceId.Coin,UserData.ResourceSubType.Big);
            }
            else if (couponType == ExtraOrderRewardCouponType.DogHope)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(DogHopeModel._dogCookiesId);
                Icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else if (couponType == ExtraOrderRewardCouponType.SnakeLadder)
            {
                Icon.sprite = UserData.GetResourceIcon(UserData.ResourceId.SnakeLadderTurntable,UserData.ResourceSubType.Big);
            }
            else if (couponType == ExtraOrderRewardCouponType.ThemeDecoration)
            {
                Icon.sprite = UserData.GetResourceIcon(UserData.ResourceId.ThemeDecorationScore,UserData.ResourceSubType.Big);
            }
            else if (couponType == ExtraOrderRewardCouponType.Parrot)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(ParrotModel.ParrotMergeItemId);
                Icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else if (couponType == ExtraOrderRewardCouponType.FlowerField)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(FlowerFieldModel.FlowerFieldMergeItemId);
                Icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
        }
    }

    public void UpdateTime()
    {
        if (CurCouponList.Count == 0)
        {
            return;
        }
        var coupon = CurCouponList[0];
        var leftTime = coupon.GetLeftTime();
        TimeText.SetText(XUtility.FormatLongToTimeStr((long)leftTime,XUtility.ShowTimeStrLevel.Minute));
    }

    public void OnClickBtn()
    {
        if (CurCouponList.Count == 0)
        {
            return;
        }
        UIExtraOrderRewardGetController.Open(CurCouponList[0]);
    }
}