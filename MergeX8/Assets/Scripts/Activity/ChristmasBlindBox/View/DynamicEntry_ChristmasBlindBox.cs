using System;
using Dynamic;
using UnityEngine;

namespace Activity.ChristmasBlindBox.View
{
    public class DynamicEntry_Home_ChristmasBlindBox: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/ChristmasBlindBox/Aux_ChristmasBlindBox";
        protected override Type _dynamicType => typeof(Aux_ChristmasBlindBox);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return ChristmasBlindBoxModel.Instance.IsPrivateOpened();
        }
    }
}