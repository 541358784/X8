using System;
using Dynamic;
using UnityEngine;

namespace Activity.KapiScrew.View
{
    public class DynamicEntry_Home_KapiScrew : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/KapiScrew/Aux_KapibalaContest";
        protected override Type _dynamicType => typeof(Aux_KapiScrew);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return KapiScrewModel.Instance.ShowAuxItem();
        }
    }
    
    
    public class DynamicEntry_Game_KapiScrew: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/KapiScrew/TaskList_KapibalaContest";
        protected override Type _dynamicType => typeof(MergeKapiScrew);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return KapiScrewModel.Instance.ShowTaskEntrance();
        }
    }
}