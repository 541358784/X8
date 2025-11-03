using System;
using Dynamic;
using UnityEngine;

namespace ClimbTreeLeaderBoard
{
    public class DynamicEntry_Game_ClimbTree : DynamicEntryBase
    {
        protected override string _entryPath => ClimbTreeModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeClimbTree);
        protected override Transform _parent =>  MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return ClimbTreeModel.Instance.ShowEntrance();
        }
    }
}