using System;
using Dynamic;
using UnityEngine;

namespace Activity.KapiTile.View
{
    public class DynamicEntry_Home_KapiTile : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/KapiTile/Aux_KapiTile";
        protected override Type _dynamicType => typeof(Aux_KapiTile);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return KapiTileModel.Instance.ShowAuxItem();
        }
    }


    public class DynamicEntry_Game_KapiTile : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/KapiTile/TaskList_KapiTile";
        protected override Type _dynamicType => typeof(MergeKapiTile);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return KapiTileModel.Instance.ShowTaskEntrance();
        }
    }
}