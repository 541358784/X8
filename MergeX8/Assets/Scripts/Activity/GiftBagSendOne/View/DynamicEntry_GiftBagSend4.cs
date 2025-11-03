using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagSendOne.Controller
{
    public class DynamicEntry_Home_GiftBagSendOne : DynamicEntryBase
    {
        protected override string _entryPath => GiftBagSendOneModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_GiftBagSendOne);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return GiftBagSendOneModel.Instance.ShowEntrance();
        }
    }
}