using System;
using Dynamic;
using UnityEngine;

namespace Activity.CoinCompetition
{
    public class DynamicEntry_Game_CoinCompetition : DynamicEntryBase
    {
        protected override string _entryPath => CoinCompetitionModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeCoinCompetition);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return CoinCompetitionModel.Instance.ShowEntrance();
        }
    }
}