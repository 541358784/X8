using System;
using Dynamic;
using UnityEngine;

namespace TMatch
{
    public class DynamicEntry_Home_Kapibala : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Kapibala/Aux_Kapibala";
        protected override Type _dynamicType => typeof(Aux_Kapibala);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return KapibalaModel.Instance.ShowAuxItem();
        }
    }
    
    
    public class DynamicEntry_Game_Kapibala: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Kapibala/TaskList_Kapibala";
        protected override Type _dynamicType => typeof(MergeKapibala);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return KapibalaModel.Instance.ShowTaskEntrance();
        }
    }
}