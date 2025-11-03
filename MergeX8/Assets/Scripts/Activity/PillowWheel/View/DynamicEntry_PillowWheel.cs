using System;
using Dynamic;
using UnityEngine;

namespace Activity.PillowWheel.UI
{
    public class DynamicEntry_Home_PillowWheel : DynamicEntryBase
    {
        protected override string _entryPath => PillowWheelModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_PillowWheel);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return PillowWheelModel.Instance.ShowAuxItem();
        }
    }
    
    public class DynamicEntry_Game_PillowWheel : DynamicEntryBase
    {
        protected override string _entryPath => PillowWheelModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergePillowWheel);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return PillowWheelModel.Instance.ShowTaskEntrance();
        }
    }
}