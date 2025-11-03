using System;
using Dynamic;
using UnityEngine;

namespace Activity.LuckyGoldenEgg.Controller
{
    public class DynamicEntry_Home_LuckyGoldenEgg : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/LuckyGoldenEgg/Aux_Hen";
        protected override Type _dynamicType => typeof(Aux_LuckyGoldenEgg);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return LuckyGoldenEggModel.Instance.IsOpened();
        }
    }
    public class DynamicEntry_Game_LuckyGoldenEgg : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/LuckyGoldenEgg/TaskList_LuckyGoldenEgg";
        protected override Type _dynamicType => typeof(MergeLuckyGoldenEgg);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return LuckyGoldenEggModel.Instance.IsOpened() && !LuckyGoldenEggModel.Instance.IsPreheating();
        }
    }
}