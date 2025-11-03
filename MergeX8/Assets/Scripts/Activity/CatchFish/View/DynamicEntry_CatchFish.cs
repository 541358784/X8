using System;
using Dynamic;
using UnityEngine;

namespace Activity.CatchFish.UI
{
    public class DynamicEntry_Home_CatchFish : DynamicEntryBase
    {
        protected override string _entryPath => CatchFishModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_CatchFish);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return CatchFishModel.Instance.ShowAuxItem();
        }
    }
    
    public class DynamicEntry_Game_CatchFish : DynamicEntryBase
    {
        protected override string _entryPath => CatchFishModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeCatchFish);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return CatchFishModel.Instance.ShowTaskEntrance();
        }
    }
}