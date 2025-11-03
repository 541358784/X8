using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagSend4.Controller
{
    public class DynamicEntry_Home_GiftBagSend4 : DynamicEntryBase
    {
        protected override string _entryPath => GiftBagSend4Model.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_GiftBagSend4);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return GiftBagSend4Model.Instance.ShowEntrance();
        }
    }
}