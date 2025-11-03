using System;
using Dynamic;
using UnityEngine;

namespace Activity.FlowerField.UI
{
    public class DynamicEntry_Home_FlowerField : DynamicEntryBase
    {
        protected override string _entryPath => FlowerFieldModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_FlowerField);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return FlowerFieldModel.Instance.ShowAuxItem();
        }
    }
    
    public class DynamicEntry_Game_FlowerField : DynamicEntryBase
    {
        protected override string _entryPath => FlowerFieldModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeFlowerField);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return FlowerFieldModel.Instance.ShowTaskEntrance();
        }
    }
}