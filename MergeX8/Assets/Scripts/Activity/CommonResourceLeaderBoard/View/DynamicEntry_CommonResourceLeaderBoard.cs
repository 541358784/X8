using System;
using Dynamic;
using UnityEngine;

namespace Activity.CommonResourceLeaderBoard.View
{
    public class DynamicEntry_Home_CommonResourceLeaderBoard : DynamicEntryBase
    {
        protected override string _entryPath => CommonResourceLeaderBoardModel.Instance.AuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_CommonResourceLeaderBoard);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return CommonResourceLeaderBoardModel.Instance.CanCreate();
        }
    }
}