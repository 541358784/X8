using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagDouble.View
{
    public class DynamicEntry_Home_GiftBagDouble: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/GiftBagDouble/Aux_GiftBagDouble";
        protected override Type _dynamicType => typeof(Aux_GiftBagDouble);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return GiftBagDoubleModel.Instance.ShowAuxItem();
        }
    }
}