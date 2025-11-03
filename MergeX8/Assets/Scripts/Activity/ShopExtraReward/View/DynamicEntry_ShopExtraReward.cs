using System;
using Dynamic;
using UnityEngine;

namespace Activity.ShopExtraReward.View
{
    public class DynamicEntry_Home_ShopExtraReward: DynamicEntryBase
    {
        protected override string _entryPath => ShopExtraRewardModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_ShopExtraReward);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return ShopExtraRewardModel.Instance.ShowEntrance();
        }
    }
}