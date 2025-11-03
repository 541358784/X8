using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagSend6.Controller
{
    public class DynamicEntry_Home_GiftBagSend6 : DynamicEntryBase
    {
        protected override string _entryPath => GiftBagSend6Model.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_GiftBagSend6);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return GiftBagSend6Model.Instance.ShowEntrance();
        }
    }
}