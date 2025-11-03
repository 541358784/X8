using System;
using Dynamic;
using UnityEngine;

namespace Activity.BattlePass_2
{
    public class DynamicEntry_Home_BattlePass2 : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/BPTwo/Aux_BattlePass";
        protected override Type _dynamicType => typeof(Aux_BattlePass2);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return BattlePassModel.Instance.IsOpened();
        }
    }
    public class DynamicEntry_Game_BattlePass2 : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/BPTwo/TaskList_BattlePass";
        protected override Type _dynamicType => typeof(MergeBattlePass2);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return BattlePassModel.Instance.IsOpened();
        }
    }
}