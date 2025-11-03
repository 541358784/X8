using System;
using Dynamic;
using UnityEngine;

namespace DogHopeLeaderBoard
{
    public class DynamicEntry_Game_DogHope : DynamicEntryBase
    {
        protected override string _entryPath => DogHopeModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeDogHope);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return DogHopeModel.Instance.IsOpenActivity();
        }
    }
}