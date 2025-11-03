using System;
using Activity.JumpGrid;
using Dynamic;
using UnityEngine;

namespace ActivityLocal.NewIceBreakGiftBag.View
{
    public class DynamicEntry_Home_NewIceBreakGiftBag: DynamicEntryBase
    {
        protected override string _entryPath => NewIceBreakGiftBagModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_NewIceBreakGiftBag);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return NewIceBreakGiftBagModel.Instance.IsOpen;
        }
    }
}