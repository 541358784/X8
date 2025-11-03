using System;
using Dynamic;
using UnityEngine;

namespace Activity.DiamondReward.View
{
    public class DynamicEntry_Home_DiamondReward: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/DiamondReward/Aux_DiamondReward";
        protected override Type _dynamicType => typeof(Aux_DiamondReward);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return DiamondRewardModel.Model.DiamondRewardModel.Instance.IsOpened();
        }
    }
}