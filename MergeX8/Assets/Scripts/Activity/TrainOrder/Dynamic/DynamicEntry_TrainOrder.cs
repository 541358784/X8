using System;
using Dynamic;
using UnityEngine;

namespace Activity.TrainOrder
{
    public class DynamicEntry_MergeTrainOrder: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/TrainOrder/TaskList_TrainOrder";
        protected override Type _dynamicType => typeof(Merge_TrainOrder);

        protected override Transform _parent =>  MergeTaskTipsController.Instance.DynamicParent;
        
        protected override bool CanCreateEntry()
        {
            return TrainOrderModel.Instance.IsOpened();
        }
    }
}