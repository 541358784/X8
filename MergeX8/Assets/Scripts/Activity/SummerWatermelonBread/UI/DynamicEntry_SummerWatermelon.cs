using System;
using Activity.Turntable.Model;
using Dynamic;
using UnityEngine;

namespace Activity.SummerWatermelonBread.UI
{
    public class DynamicEntry_SummerWatermelonBread : DynamicEntryBase
    {
        protected override string _entryPath => SummerWatermelonBreadModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_SummerWatermelonBread);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return SummerWatermelonBreadModel.Instance.IsStart;
        }
    }
    
    public class DynamicEntry_Home_SummerWatermelonBreadGift : DynamicEntryBase
    {
        protected override string _entryPath => SummerWatermelonBreadModel.Instance.GetPackageAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_SummerWatermelonBreadGift);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            var package = SummerWatermelonBreadModel.Instance.GetCurrentPackage();
            return package!=null;
        }
    }
    
    
    public class DynamicEntry_Game_SummerWatermelonBread : DynamicEntryBase
    {
        protected override string _entryPath => SummerWatermelonBreadModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeSummerWatermelonBread);
        protected override Transform _parent =>  MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return SummerWatermelonBreadModel.Instance.IsStart;
        }
    }
}