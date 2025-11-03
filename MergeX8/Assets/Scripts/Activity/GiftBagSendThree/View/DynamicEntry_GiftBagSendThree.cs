using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagSendThree.Controller
{
    public class DynamicEntry_Home_GiftBagSendThree : DynamicEntryBase
    {
        protected override string _entryPath => GiftBagSendThreeModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_GiftBagSendThree);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return GiftBagSendThreeModel.Instance.ShowEntrance();
        }
    }
}