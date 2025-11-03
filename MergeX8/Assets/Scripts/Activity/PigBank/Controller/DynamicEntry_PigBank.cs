using System;
using Dynamic;
using UnityEngine;

namespace Activity.PigBank.Controller
{
    public class DynamicEntry_Game_PigBank : DynamicEntryBase
    {
        protected override string _entryPath => PigBankModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergePigBoxInTaskController);
        protected override Transform _parent =>  MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return PigBankModel.Instance.IsOpened();
        }
    }
}