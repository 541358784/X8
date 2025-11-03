using System;
using Dynamic;
using UnityEngine;

namespace Activity.CoinRush
{
    public class DynamicEntry_Game_CoinRush: DynamicEntryBase
    {
        protected override string _entryPath => CoinRushModel.Instance.GetMergeItemAssetPath();
        protected override Type _dynamicType => typeof(MergeCoinRush);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return CoinRushModel.Instance.IsOpened();
        }
    }
}