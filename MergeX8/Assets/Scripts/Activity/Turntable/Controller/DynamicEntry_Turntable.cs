using System;
using Dynamic;
using Activity.Turntable.Model;
using UnityEngine;

namespace Activity.Turntable.Controller
{
    public class DynamicEntry_Home_Turntable : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Turntable/Aux_Turntable";
        protected override Type _dynamicType => typeof(Aux_Turntable);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return TurntableModel.Instance.IsOpened();
        }
    }
    
    public class DynamicEntry_Game_Turntable : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Turntable/TaskList_Turntable";
        protected override Type _dynamicType => typeof(MergeTurntableEntry);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return TurntableModel.Instance.IsOpened();
        }
    }
}