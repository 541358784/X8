using System;
using Dynamic;
using UnityEngine;

namespace Activity.BiuBiu.UI
{
    public class DynamicEntry_Home_BiuBiu : DynamicEntryBase
    {
        protected override string _entryPath => BiuBiuModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_BiuBiu);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return BiuBiuModel.Instance.ShowEntrance();
        }
    }
    
    
    public class DynamicEntry_Game_BiuBiu : DynamicEntryBase
    {
        protected override string _entryPath => BiuBiuModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeBiuBiu);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return BiuBiuModel.Instance.ShowEntrance();
        }
    }
}