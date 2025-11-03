using System;
using Dynamic;
using UnityEngine;

namespace Activity.FishCulture.View
{
    public class DynamicEntry_Home_FishCulture : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/FishCulture/Aux_FishCulture";
        protected override Type _dynamicType => typeof(Aux_FishCulture);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return FishCultureModel.Instance.ShowAuxItem();
        }
    }
    
    
    
    public class DynamicEntry_Game_FishCulture: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/FishCulture/TaskList_FishCulture";
        protected override Type _dynamicType => typeof(MergeFishCulture);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return FishCultureModel.Instance.ShowTaskEntrance();
        }
    }
}