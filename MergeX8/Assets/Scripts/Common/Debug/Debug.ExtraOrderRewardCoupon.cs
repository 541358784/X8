using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string ExtraOrderRewardCoupon = "任务翻倍券";
    [Category(ExtraOrderRewardCoupon)]
    [DisplayName("获取券")]
    public void ResetExtraOrderRewardCoupon()
    {
        if (ExtraOrderRewardCouponModel.IsCouponId(_couponId))
        {
            ExtraOrderRewardCouponModel.Instance.AddCoupon((UserData.ResourceId)_couponId,1,"Debug");
        }
    }

    private int _couponId;
    [Category(ExtraOrderRewardCoupon)]
    [DisplayName("券Id")]
    public int CouponId
    {
        get
        {
            return _couponId;
        }
        set
        {
            _couponId = value;
        }
    }

    [Category(ExtraOrderRewardCoupon)]
    [DisplayName("当前券剩余时间(秒)")]
    public int CurCouponLeftTime
    {
        get
        {
            if (ExtraOrderRewardCouponModel.Instance.Storage.CurCouponList.Count == 0)
                return 0;
            var leftTime = APIManager.Instance.GetServerTime() -
                           ExtraOrderRewardCouponModel.Instance.Storage.CurCouponList[0].EndTime;
            return (int)(leftTime / XUtility.Second);
        }
        set
        {
            if (ExtraOrderRewardCouponModel.Instance.Storage.CurCouponList.Count == 0)
                return;
            var leftTime = (ulong) value * XUtility.Second;
            ExtraOrderRewardCouponModel.Instance.Storage.CurCouponList[0].EndTime = APIManager.Instance.GetServerTime()+leftTime;
        }
    }

    [Category(ExtraOrderRewardCoupon)]
    [DisplayName("清空所有券")]
    public void CleanStorage()
    {
        ExtraOrderRewardCouponModel.Instance.Storage.Clear();
    }
}