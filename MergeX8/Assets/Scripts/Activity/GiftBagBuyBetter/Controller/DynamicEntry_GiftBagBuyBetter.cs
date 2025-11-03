using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagBuyBetter.Controller
{
    public class DynamicEntry_Home_GiftBagBuyBetter : DynamicEntryBase
    {
        protected override string _entryPath => GiftBagBuyBetterModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_GiftBagBuyBetter);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return GiftBagBuyBetterModel.Instance.ShowEntrance();
        }
    }
}