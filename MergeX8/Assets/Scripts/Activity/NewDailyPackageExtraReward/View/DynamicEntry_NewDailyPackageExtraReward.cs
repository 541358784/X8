using System;
using Dynamic;
using UnityEngine;

namespace Activity.NewDailyPackageExtraReward.View
{
    public class DynamicEntry_Home_NewDailyPackageExtraReward: DynamicEntryBase
    {
        protected override string _entryPath => NewDailyPackageExtraRewardModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_NewDailyPackageExtraReward);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return NewDailyPackageExtraRewardModel.Instance.ShowAuxItem();
        }
    }
}