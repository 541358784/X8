using System;
using Dynamic;
using UnityEngine;

namespace SnakeLadderLeaderBoard
{
    public class DynamicEntry_Home_SnakeLadder : DynamicEntryBase
    {
        protected override string _entryPath => SnakeLadderModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_SnakeLadder);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return SnakeLadderModel.Instance.IsPrivateOpened();
        }
    }
    
    public class DynamicEntry_Game_SnakeLadder : DynamicEntryBase
    {
        protected override string _entryPath => SnakeLadderModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeSnakeLadder);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return SnakeLadderModel.Instance.ShowEntrance();
        }
    }
}