using System;
using DragonU3DSDK.Asset;
using Dynamic;
using UnityEngine;

namespace Activity.RabbitRacing.Dynamic
{
    public class DynamicEntry_Home_RabbitRacing : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/RabbitRacing/DynamicHome_RabbitRacing";
        protected override Type _dynamicType => typeof(Home_RabbitRacing);
    
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        
        protected override bool CanCreateEntry()
        {
            return RabbitRacingModel.Instance.IsOpened() && RabbitRacingModel.Instance.Storage.IsJoin;
        }
    }
    
    public class DynamicEntry_Game_RabbitRacing: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/RabbitRacing/DynamicGame_RabbitRacing";
        protected override Type _dynamicType => typeof(Game_RabbitRacing);
    
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;
        
        protected override bool CanCreateEntry()
        {
            return RabbitRacingModel.Instance.IsOpened() && RabbitRacingModel.Instance.Storage.IsJoin;
        }
    }
}


