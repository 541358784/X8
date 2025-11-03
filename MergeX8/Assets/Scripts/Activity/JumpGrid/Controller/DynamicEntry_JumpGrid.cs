using System;
using Dynamic;
using UnityEngine;

namespace Activity.JumpGrid.Controller
{
    public class DynamicEntry_Home_JumpGrid : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/JumpGrid/Aux_JumpGrid";
        protected override Type _dynamicType => typeof(Aux_JumpGrid);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();

        protected override bool CanCreateEntry()
        {
            return JumpGridModel.Instance.IsOpened();
        }
    }
    
    public class DynamicEntry_Game_JumpGrid : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/JumpGrid/TaskList_JumpGrid";
        protected override Type _dynamicType => typeof(MergeTaskItemJumpGrid);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return JumpGridModel.Instance.IsOpened() && JumpGridModel.Instance.IsShowStart() &&
                   !JumpGridModel.Instance.StorageJumpGrid.IsShowEndView;
        }
    }
}