using System;
using Dynamic;
using UnityEngine;

namespace Activity.TurtlePang.View
{
    public class DynamicEntry_Home_TurtlePang : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/TurtlePang/Aux_TurtlePang";
        protected override Type _dynamicType => typeof(Aux_TurtlePang);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return TurtlePangModel.Instance.ShowAuxItem();
        }
    }
    
    public class DynamicEntry_Game_TurtlePang: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/TurtlePang/TaskList_TurtlePang";
        protected override Type _dynamicType => typeof(MergeTurtlePang);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return TurtlePangModel.Instance.ShowTaskEntrance();
        }
    }
}