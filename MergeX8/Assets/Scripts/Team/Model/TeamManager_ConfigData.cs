using DragonPlus.Config.Team;
using Gameplay;

namespace Scripts.UI
{
    public struct TeamEnergyGiftInfoExtra
    {
        public EnergyGiftRewardExtra RewardExtra;
        public EnergyGiftDataExtra DataExtra;
    }

    public struct EnergyGiftRewardExtra
    {
        public int ItemId;
        public int ItemCnt;
    }
    
    public struct EnergyGiftDataExtra
    {
        public int TotalCount;
        public int ShowIconIndex;
        public string GiftNameKey;
    }

    public struct TeamLikeInfoExtra
    {
        public ActivityInfoExtra ActivityExtra;
    }

    public struct ActivityInfoExtra
    {
        public string ActivityType;
        public int Ranking;
    }
    public partial class TeamManager
    {
        

        public int CreateCoins()
        {
            if (RemoteTeamConfig == null)
                return TeamConfigManager.Instance.LocalTeamConfig.CreateCoins;
            return RemoteTeamConfig.CreateCoins;
        }

        public UserData.ResourceId CreateResourceID()
        {
            return UserData.ResourceId.Coin;
        }

        public int TeamNameMax()
        {
            if (RemoteTeamConfig == null)
                return TeamConfigManager.Instance.LocalTeamConfig.TeamNameMax;
            return RemoteTeamConfig.TeamNameMax;
        }
        
        public int TeamNameMin()
        {
            if (RemoteTeamConfig == null)
                return TeamConfigManager.Instance.LocalTeamConfig.TeamNameMin;
            return RemoteTeamConfig.TeamNameMin;
        }

        public int TeamDescMax()
        {
            if (RemoteTeamConfig == null)
                return TeamConfigManager.Instance.LocalTeamConfig.TeamDescMax;
            return RemoteTeamConfig.TeamDescMax;
        }

        public int SystemChatTimeLimit()
        {
            return TeamConfigManager.Instance.LocalTeamConfig.SystemChatTimeLimit;
        }
    }
}
