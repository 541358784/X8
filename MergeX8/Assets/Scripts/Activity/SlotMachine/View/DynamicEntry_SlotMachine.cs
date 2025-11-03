using System;
using Dynamic;
using UnityEngine;

namespace Activity.SlotMachine.View
{
    public class DynamicEntry_Home_SlotMachine: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/SlotMachine/Aux_SlotMachine";
        protected override Type _dynamicType => typeof(Aux_SlotMachine);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return SlotMachineModel.Instance.ShowEntrance();
        }
    }
    
    public class DynamicEntry_Game_SlotMachine : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/SlotMachine/TaskList_SlotMachine";
        protected override Type _dynamicType => typeof(MergeSlotMachine);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return SlotMachineModel.Instance.ShowEntrance();
        }
    }
}