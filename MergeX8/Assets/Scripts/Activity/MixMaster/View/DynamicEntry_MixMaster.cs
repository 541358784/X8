using System;
using Dynamic;
using UnityEngine;

namespace Activity.MixMaster.View
{
    public class DynamicEntry_Home_MixMaster : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/MixMaster/Aux_MixMaster";
        protected override Type _dynamicType => typeof(Aux_MixMaster);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return MixMasterModel.Instance.ShowAuxItem();
        }
    }
    
    public class DynamicEntry_Game_MixMaster: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/MixMaster/TaskList_MixMaster";
        protected override Type _dynamicType => typeof(MergeMixMaster);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return MixMasterModel.Instance.ShowTaskEntrance();
        }
    }
}