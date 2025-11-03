using System;
using Dynamic;
using UnityEngine;

namespace Activity.SaveTheWhales
{
    public class DynamicEntry_Game_SaveTheWhales: DynamicEntryBase
    {
        protected override string _entryPath => SaveTheWhalesModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeSaveTheWhales);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return SaveTheWhalesModel.Instance.IsJoin();
        }
    }
}