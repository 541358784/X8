using System;
using Dynamic;
using UnityEngine;

namespace Activity.BattlePass
{
    public class DynamicEntry_Home_BattlePass : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/BattlePass/Aux_BattlePass";
        protected override Type _dynamicType => typeof(Aux_BattlePass);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return BattlePassModel.Instance.IsOpened();
        }
    }
    
    public class DynamicEntry_Game_BattlePass : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/BattlePass/TaskList_BattlePass";
        protected override Type _dynamicType => typeof(MergeBattlePass);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return BattlePassModel.Instance.IsOpened();
        }
    }
}