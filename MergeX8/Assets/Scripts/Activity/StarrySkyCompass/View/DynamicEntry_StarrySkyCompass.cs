using System;
using Dynamic;
using UnityEngine;

namespace Activity.StarrySkyCompass.View
{
    public class DynamicEntry_Home_StarrySkyCompass : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/StarrySkyCompass/Aux_StarrySkyCompass";
        protected override Type _dynamicType => typeof(Aux_StarrySkyCompass);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return StarrySkyCompassModel.Instance.ShowAuxItem();
        }
    }
    
    
    public class DynamicEntry_Game_StarrySkyCompass: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/StarrySkyCompass/TaskList_StarrySkyCompass";
        protected override Type _dynamicType => typeof(MergeStarrySkyCompass);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return StarrySkyCompassModel.Instance.ShowTaskEntrance();
        }
    }
}