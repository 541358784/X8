using System;
using Dynamic;
using UnityEngine;

namespace Activity.Zuma.View
{
    public class DynamicEntry_Home_Zuma : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Zuma/Aux_Zuma";
        protected override Type _dynamicType => typeof(Aux_Zuma);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return ZumaModel.Instance.ShowAuxItem();
        }
    }
    
    
    
    public class DynamicEntry_Game_Zuma: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Zuma/TaskList_Zuma";
        protected override Type _dynamicType => typeof(MergeZuma);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return ZumaModel.Instance.ShowTaskEntrance();
        }
    }
}