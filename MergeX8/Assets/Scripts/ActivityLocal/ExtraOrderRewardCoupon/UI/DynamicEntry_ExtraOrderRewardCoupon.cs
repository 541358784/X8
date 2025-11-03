using System;
using Dynamic;
using UnityEngine;

namespace ActivityLocal.ExtraOrderRewardCoupon.UI
{
    public class DynamicEntry_Game_ExtraOrderRewardCoupon : DynamicEntryBase
    {
        protected override string _entryPath => ExtraOrderRewardCouponModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeExtraOrderRewardCoupon);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return ExtraOrderRewardCouponModel.Instance.ShowEntrance();
        }
    }
}