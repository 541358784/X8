using System;
using Dynamic;
using UnityEngine;

namespace Easter2024LeaderBoard
{
    public class DynamicEntry_Home_Easter2024 : DynamicEntryBase
    {
        protected override string _entryPath => Easter2024Model.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_Easter2024);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return Easter2024Model.Instance.IsPrivateOpened();
        }
    }
    
    public class DynamicEntry_Game_Easter2024 : DynamicEntryBase
    {
        protected override string _entryPath => Easter2024Model.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeEaster2024);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return Easter2024Model.Instance.ShowEntrance();
        }
    }
}