using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagSendTwo.Controller
{
    public class DynamicEntry_Home_GiftBagSendTwo : DynamicEntryBase
    {
        protected override string _entryPath => GiftBagSendTwoModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_GiftBagSendTwo);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return GiftBagSendTwoModel.Instance.ShowEntrance();
        }
    }
}