using System;
using Dynamic;
using UnityEngine;

namespace Activity.GiftBagProgress.View
{
    public class DynamicEntry_Home_GiftBagProgress : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/GiftBagProgress/Aux_GiftBagProgress";
        protected override Type _dynamicType => typeof(Aux_GiftBagProgress);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return GiftBagProgressModel.Instance.ShowAuxItem();
        }
    }
    
    
    public class DynamicEntry_Game_GiftBagProgress : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/GiftBagProgress/TaskList_GiftBagProgress";
        protected override Type _dynamicType => typeof(MergeGiftBagProgress);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return GiftBagProgressModel.Instance.ShowTaskEntrance();
        }
    }
}