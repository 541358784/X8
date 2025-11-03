using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagLink.Controller
{
    public class DynamicEntry_Home_GiftBagLink : DynamicEntryBase
    {
        protected override string _entryPath => GiftBagLinkModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_GiftBagLink);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return GiftBagLinkModel.Instance.ShowEntrance();
        }
    }
}