using System;
using Dynamic;
using UnityEngine;

namespace Activity.Monopoly.View
{
    public class DynamicEntry_Home_Monopoly : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Monopoly/Aux_Monopoly";
        protected override Type _dynamicType => typeof(Aux_Monopoly);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return MonopolyModel.Instance.ShowEntrance();
        }
    }
    
    public class DynamicEntry_Game_Monopoly : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Monopoly/TaskList_Monopoly";
        protected override Type _dynamicType => typeof(MergeMonopoly);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return MonopolyModel.Instance.ShowEntrance();
        }
    }
}