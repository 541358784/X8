using System;
using Dynamic;
using UnityEngine;

namespace Activity.SeaRacing.UI
{
    public class DynamicEntry_Game_SeaRacing: DynamicEntryBase
    {
        protected override string _entryPath => SeaRacingModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeSeaRacing);
        protected override Transform _parent =>  MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return SeaRacingModel.Instance.IsOpened();
        }
    }
}