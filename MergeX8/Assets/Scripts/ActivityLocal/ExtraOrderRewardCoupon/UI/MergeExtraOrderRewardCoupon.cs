using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class MergeExtraOrderRewardCoupon : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    private Image Icon;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;

    private StorageExtraOrderRewardCouponItem ShowCoupon =>
        ExtraOrderRewardCouponModel.Instance.Storage.PayCouponList.Count > 0
            ? ExtraOrderRewardCouponModel.Instance.Storage.PayCouponList.Last()
            : null;
    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        Icon = transform.Find("Root/Icon").GetComponent<Image>();
        _redPoint = transform.Find("Root/RedPoint");
        _redPointText = transform.Find("Root/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("RefreshCountDown", 0, 1f);
        EventDispatcher.Instance.AddEvent<EventExtraOrderRewardCouponGetNewCoupon>(OnGetNewCoupon);
        EventDispatcher.Instance.AddEvent<EventExtraOrderRewardCouponUsePayCoupon>(OnUsePayCoupon);
    }

    public void OnGetNewCoupon(EventExtraOrderRewardCouponGetNewCoupon evt)
    {
        RefreshView();
    }
    public void OnUsePayCoupon(EventExtraOrderRewardCouponUsePayCoupon evt)
    {
        RefreshView();
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventExtraOrderRewardCouponGetNewCoupon>(OnGetNewCoupon);
        EventDispatcher.Instance.RemoveEvent<EventExtraOrderRewardCouponUsePayCoupon>(OnUsePayCoupon);
    }

    // public void Update()
    // {
    //     RefreshView();
    // }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ExtraOrderRewardCouponInfo);
        ExtraOrderRewardCouponModel.Instance.UsePayCoupon();
    }

    private void OnEnable()
    {
        RefreshView();
        StopAllCoroutines();
    }

    private void RefreshView()
    {
        gameObject.SetActive(ExtraOrderRewardCouponModel.Instance.ShowEntrance());
        if (gameObject.activeInHierarchy)
        {
            Icon.sprite = UserData.GetResourceIcon(ShowCoupon.CouponId, UserData.ResourceSubType.Big);
            _redPoint.gameObject.SetActive(true);
            _redPointText.gameObject.SetActive(true);
            _redPointText.SetText(ExtraOrderRewardCouponModel.Instance.Storage.PayCouponList.Count.ToString());
        }
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (ShowCoupon != null)
        {
            _countDownTime.SetText(CommonUtils.FormatLongToTimeStr((long)ExtraOrderRewardCouponModel.Instance.Config[ShowCoupon.CouponId].GetCurDayLeftTime()));
        }
    }
}