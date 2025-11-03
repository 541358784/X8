using System.Collections.Generic;
using System.Linq;
using Activity.BattlePass;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;

namespace Activity.BattlePass_2
{
    public static class BattlePassUtils
    {
        private static Dictionary<StorageBattlePass, List<BattlePass.TableBattlePassLoopRewardConfig>> LoopRewardConfigDic = new Dictionary<StorageBattlePass, List<BattlePass.TableBattlePassLoopRewardConfig>>();
        public static List<BattlePass.TableBattlePassLoopRewardConfig> GetLoopRewardConfigs(this StorageBattlePass storage)
        {
            if (storage.LoopRewardConfigToJson.IsEmptyString())
                return null;
            if (!LoopRewardConfigDic.TryGetValue(storage, out var result))
            {
                result = JsonConvert.DeserializeObject<List<BattlePass.TableBattlePassLoopRewardConfig>>(storage.LoopRewardConfigToJson);
                LoopRewardConfigDic.Add(storage,result);
            }
            return result;
        }
        public static int GetFrontLoopRewardScore(this StorageBattlePass storage)
        {
            if (storage == null)
                return 0;
            return storage.Reward.Last().UnlockScore + storage.LoopRewardCollectTimes * storage.LoopRewardScore;
        }

        public static int GetNextLoopRewardScore(this StorageBattlePass storage)
        {
            if (storage == null)
                return 0;
            return storage.Reward.Last().UnlockScore + (storage.LoopRewardCollectTimes+1) * storage.LoopRewardScore;
        }
        public static bool LoopRewardIsOpened(this StorageBattlePass storage)
        {
            return storage.IsPurchase;
        }
        public static List<ResData> GetCurLoopRewards(this StorageBattlePass storage)
        {
            if (storage == null)
                return null;
            var index = (storage.LoopRewardCollectTimes) % storage.LoopRewardList.Count;
            var loopRewardId = storage.LoopRewardList[index];
            var loopRewardConfig = storage.GetLoopRewardConfigs().Find((a) => a.id == loopRewardId);
            var rewards = CommonUtils.FormatReward(loopRewardConfig.rewardId,loopRewardConfig.rewardNum);
            return rewards;
        }
    }
}