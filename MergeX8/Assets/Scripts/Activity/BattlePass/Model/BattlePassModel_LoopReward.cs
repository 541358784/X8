using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

namespace Activity.BattlePass
{
    public partial class BattlePassModel
    {
        public int GetFrontLoopRewardScore()
        {
            return storageBattlePass.GetFrontLoopRewardScore();
        }

        public int GetNextLoopRewardScore()
        {
            return storageBattlePass.GetNextLoopRewardScore();
        }

        public void CollectLoopReward()
        {
            if (storageBattlePass == null)
                return;
            if (!storageBattlePass.LoopRewardIsOpened())
                return;
            if (CurScoreRatio < GetNextLoopRewardScore())
                return;
            var rewards = storageBattlePass.GetCurLoopRewards();
            storageBattlePass.LoopRewardCollectTimes++;
            for (int i = 0; i < rewards.Count; i++)
            {
                if (!UserData.Instance.IsResource(rewards[i].id))
                {
                    var itemConfig=GameConfigManager.Instance.GetItemConfig(rewards[i].id);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonBp1,
                            itemAId =itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpRewardFinalBox,data1:storageBattlePass.LoopRewardCollectTimes.ToString());
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Bp1);
            reasonArgs.data1 = "BP循环奖励";
            
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,reasonArgs, () =>
            {
            });
            EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_COLLECT_LOOP);
        }

        public List<ResData> GetCurLoopRewards()
        {
            return storageBattlePass.GetCurLoopRewards();
        }
    }
}