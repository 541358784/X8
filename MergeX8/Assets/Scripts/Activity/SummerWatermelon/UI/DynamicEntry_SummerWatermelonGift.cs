using System;
using Dynamic;
using UnityEngine;

namespace Activity.SummerWatermelon.UI
{
    public class DynamicEntry_Home_SummerWatermelonGift: DynamicEntryBase
    {
        protected override string _entryPath => SummerWatermelonModel.Instance.GetPackageAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_SummerWatermelonGift);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            var package = SummerWatermelonModel.Instance.GetCurrentPackage();
            return package!=null;
        }
    }
}