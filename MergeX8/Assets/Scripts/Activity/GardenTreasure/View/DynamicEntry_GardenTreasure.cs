using System;
using Dynamic;
using Activity.GardenTreasure.Model;
using UnityEngine;

namespace Activity.GardenTreasure.View
{
    public class DynamicEntry_Home_GardenTreasure: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/GardenTreasure/Aux_GardenTreasure";
        protected override Type _dynamicType => typeof(Aux_GardenTreasure);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return GardenTreasureModel.Instance.IsOpened();
        }
    }
    
    public class DynamicEntry_Game_GardenTreasure : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/GardenTreasure/TaskList_GardenTreasure";
        protected override Type _dynamicType => typeof(MergeGardenTreasureEntry);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return GardenTreasureModel.Instance.IsOpened() && GardenTreasureModel.Instance.IsPreheatEnd();
        }
    }
}