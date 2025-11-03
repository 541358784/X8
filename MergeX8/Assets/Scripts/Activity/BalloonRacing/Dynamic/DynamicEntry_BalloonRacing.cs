using System;
using DragonU3DSDK.Asset;
using Dynamic;
using UnityEngine;

namespace Activity.BalloonRacing.Dynamic
{
    public class DynamicEntry_Home_BalloonRacing : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/BalloonRacing/DynamicHome_BalloonRacing";
        protected override Type _dynamicType => typeof(Home_BalloonRacing);
    
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        
        protected override bool CanCreateEntry()
        {
            return BalloonRacingModel.Instance.IsOpened() && BalloonRacingModel.Instance.Storage.IsJoin;
        }
    }
    
    public class DynamicEntry_Game_BalloonRacing: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/BalloonRacing/DynamicGame_BalloonRacing";
        protected override Type _dynamicType => typeof(Game_BalloonRacing);
    
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;
        
        protected override bool CanCreateEntry()
        {
            return BalloonRacingModel.Instance.IsOpened() && BalloonRacingModel.Instance.Storage.IsJoin;
        }
    }
}


