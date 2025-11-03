using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;
using TMPro;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace StoryMovie
{
    public class Action_AddReward : ActionBase
    {
        private int rewardId = -1;
        private int rewardNum = -1;
        protected override void Init()
        {
        }

        protected override void Start()
        {
            List<ResData> resDatas = new List<ResData>();
            ResData res = new ResData(rewardId, rewardNum);
            resDatas.Add(res);
            FlyGameObjectManager.Instance.FlyObject(resDatas, () =>
            {
                UIRoot.Instance.EnableEventSystem = true;
            });

            BiEventAdventureIslandMerge.Types.MergeEventType eventType;
            if (rewardId == 1100101)
                eventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonSealBuildingGet;
            else
                eventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonDolphinBuildingGet;
                    
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = eventType,
                itemAId = rewardId,
                ItemALevel = rewardNum,
                isChange = true,
            });
            
            var mergeItem = MergeManager.Instance.GetEmptyItem();
            mergeItem.Id = rewardId;
            mergeItem.State = 1;
            MergeManager.Instance.AddRewardItem(mergeItem, MergeBoardEnum.Main,rewardNum, true);
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
        }

        protected override void Stop()
        {
        }
        
        protected override void ParsingParam()
        {
            string[] param = _config.actionParam.Split(',');
            int.TryParse(param[0], out rewardId);
            int.TryParse(param[1], out rewardNum);
        }
    }
}