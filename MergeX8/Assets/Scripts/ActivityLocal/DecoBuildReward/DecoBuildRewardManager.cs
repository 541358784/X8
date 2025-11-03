using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;

namespace ActivityLocal.DecoBuildReward
{
    public class DecoBuildRewardManager : Singleton<DecoBuildRewardManager>
    {
        private StorageDictionary<string, StorageDecoBuildReward> DecoBuildRewards
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().DecoBuildRewards;
            }
        }
        public void InitDecoBuild(string buildId)
        {
            if(!DecoBuildRewards.ContainsKey(buildId))
                DecoBuildRewards.Add(buildId, new StorageDecoBuildReward());
            
            StorageDecoBuildReward buildReward = DecoBuildRewards[buildId];
            buildReward.DecoBuildId = buildId;
            buildReward.Index = 0;
            buildReward.GetTime = 0;
            buildReward.IsFinish = false;
        }

        public bool CanGetReward(string buildId)
        {
            if (!DecoBuildRewards.ContainsKey(buildId))
                return false;
            
            StorageDecoBuildReward buildReward = DecoBuildRewards[buildId];
            if (buildReward.IsFinish)
                return false;

            return !DragonU3DSDK.Utils.IsSameDay(buildReward.GetTime / 1000, (long)APIManager.Instance.GetServerTime() / 1000);
        }


        public List<ResData> GetRewardInfo(string buildId)
        {
            if (!CanGetReward(buildId))
                return null;
            
            StorageDecoBuildReward buildReward = DecoBuildRewards[buildId];
            if (buildReward.Index >= GlobalConfigManager.Instance.TableTotalRechargeNewRewardsList.Count)
                return null;
            
            var rewards = GlobalConfigManager.Instance.TableTotalRechargeNewRewardsList[buildReward.Index];
            return CommonUtils.FormatReward(rewards.rewardId, rewards.rewardNum);
        }
        
        public List<ResData> GetReward(string buildId)
        {
            if (!CanGetReward(buildId))
                return null;

            StorageDecoBuildReward buildReward = DecoBuildRewards[buildId];
            switch (buildId)
            {
                case "10001":
                {
                    if (buildReward.Index >= GlobalConfigManager.Instance.TableTotalRechargeNewRewardsList.Count)
                    {
                        buildReward.IsFinish = true;
                        return null;
                    }

                    buildReward.GetTime = (long)APIManager.Instance.GetServerTime();
                    
                    var rewards = GlobalConfigManager.Instance.TableTotalRechargeNewRewardsList[buildReward.Index];

                    buildReward.Index++;
                    if (buildReward.Index >= GlobalConfigManager.Instance.TableTotalRechargeNewRewardsList.Count)
                        buildReward.IsFinish = true;
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTotalRechargeNewShip,buildReward.Index.ToString(),XUtility.ArrayToString(rewards.rewardId));
                    return CommonUtils.FormatReward(rewards.rewardId, rewards.rewardNum);
                }
            }

            return null;
        }
    }
}