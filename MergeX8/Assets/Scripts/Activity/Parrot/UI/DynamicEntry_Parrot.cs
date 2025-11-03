using System;
using Dynamic;
using UnityEngine;

namespace Activity.Parrot.UI
{
    public class DynamicEntry_Home_Parrot : DynamicEntryBase
    {
        protected override string _entryPath => ParrotModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_Parrot);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return ParrotModel.Instance.ShowAuxItem();
        }
    }
    
    public class DynamicEntry_Game_Parrot : DynamicEntryBase
    {
        protected override string _entryPath => ParrotModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeParrot);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return ParrotModel.Instance.ShowTaskEntrance();
        }
    }
}