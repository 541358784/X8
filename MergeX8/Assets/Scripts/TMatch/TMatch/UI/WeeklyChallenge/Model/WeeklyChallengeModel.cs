using System.Collections.Generic;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.WeeklyChallenge;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

namespace DragonPlus.Config.TMatch
{
    public partial class TMatchConfigManager
    {
        public WeeklyChallenge GetWeeklyChallenge(int weekId)
        {
            return WeeklyChallengeList.Find(x => x.week == weekId);
        }

        public List<WeeklyChallengeReward> GetWeeklyChallengeRewards(int weekId)
        {
            WeeklyChallenge cfg = GetWeeklyChallenge(weekId);
            if (cfg == null)
                return null;
            
            return WeeklyChallengeRewardList.FindAll(x => x.RewardGroup == cfg.rewardGroup);
        }
    
        public int GetSuitableWeekId(long timestamp)
        {
            var cfgList = DragonPlus.Config.WeeklyChallenge.WeeklyChallengeConfigManager.Instance.EventWeeklyChallengeList;
            var weeklyChallenge = cfgList.Find(x => timestamp >= x.starTimeSec.ToLong() && timestamp < x.endTimeSec.ToLong());
            return weeklyChallenge != null ? weeklyChallenge.week : 1;
        }
    }
}


namespace TMatch
{
    public class WeeklyChallengeModel
    {
        public StorageWeeklyChallenge stoage;

        private EventWeeklyChallenge _challengeConfig = null;
        public WeeklyChallengeModel()
        {
            stoage = StorageManager.Instance.GetStorage<StorageTMatch>().WeeklyChallenge;
        }

        public void StarNextWeek()
        {
            stoage.CurWeekId = TMatchConfigManager.Instance.GetSuitableWeekId((long)APIManager.Instance.GetServerTime());
            stoage.CurLevel = 1;
            stoage.CurCollectItemNum = 0;
            stoage.CurClaimCnt = 0;
            stoage.Popup = false;
        }

        public bool IsUnlock(bool isDependentActivity = true)
        {
            if (isDependentActivity)
                return TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].WeeklyChallengeUnlcok && WeeklyChallengeController.Instance.IsOpen();

            return TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].WeeklyChallengeUnlcok;
        }

        public bool IsClaimedAll()
        {
            var rewards = GetCurWeekRewards();
            if (rewards == null)
                return false;
                
            return stoage.CurClaimCnt >= GetCurWeekRewards().Count;
        }

        
        public long GetCurWeekLeftTime()
        {
            if (stoage.CurWeekId == 0) 
                return 0;
            
            if (_challengeConfig == null || _challengeConfig.week != stoage.CurWeekId)
            {
                var cfgList = DragonPlus.Config.WeeklyChallenge.WeeklyChallengeConfigManager.Instance.EventWeeklyChallengeList;
                _challengeConfig = cfgList.Find(x => x.week == stoage.CurWeekId);
            }

            if (_challengeConfig == null)
                return 0;
                
            var left = _challengeConfig.endTimeSec.ToLong() - (long)APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            
            return left;
        }

        public string GetCurWeekLeftTimeString()
        {
            return CommonUtils.FormatLongToTimeStr(GetCurWeekLeftTime(), false);
        }

        public int GetCurLevelCollectNum()
        {
            List<WeeklyChallengeReward> rewards = WeeklyChallengeController.Instance.model.GetCurWeekRewards();
            int subCollectCnt = stoage.CurCollectItemNum;
            for (int i = 1; i < stoage.CurLevel; i++)
            {
                subCollectCnt -= rewards[i - 1].collectNum;
            }

            WeeklyChallengeReward weeklyChallengeReward = GetCurLevelReward();
            if (subCollectCnt > weeklyChallengeReward.collectNum)
            {
                subCollectCnt = weeklyChallengeReward.collectNum;
            }

            return subCollectCnt;
        }

        public float GetCurLevelProgress()
        {
            return GetCurLevelCollectNum() * 1.0f / GetCurLevelReward().collectNum;
        }

        public string GetCurLevelProgressString()
        {
            return $"{GetCurLevelCollectNum()}/{GetCurLevelReward().collectNum}";
        }

        public DragonPlus.Config.TMatch.WeeklyChallenge GetCurWeeklyChallengeCfg()
        {
            return TMatchConfigManager.Instance.GetWeeklyChallenge(stoage.CurWeekId);
        }

        public List<WeeklyChallengeReward> GetCurWeekRewards()
        {
            return TMatchConfigManager.Instance.GetWeeklyChallengeRewards(stoage.CurWeekId);
        }

        public WeeklyChallengeReward GetCurLevelReward()
        {
            List<WeeklyChallengeReward> rewards = GetCurWeekRewards();
            return rewards[stoage.CurLevel - 1];
        }
    }
}