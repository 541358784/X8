using System;
using System.ComponentModel.Composition.Primitives;
using Activity.CollectStone.Model;
using Dynamic;
using UnityEngine;

namespace Activity.CollectStone.View
{
    public class DynamicEntry_Home_CollectStone :  DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/CollectStone/Aux_CollectStone";
        protected override Type _dynamicType => typeof(Aux_CollectStone);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return CollectStoneModel.Instance.IsOpened();
        }
    }
}