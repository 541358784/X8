
namespace Scripts.UI
{
    public interface IEvent{}
    public struct EventOnTeamKickBubbleEnable : IEvent
    {
        public long showPlayerId;

        public EventOnTeamKickBubbleEnable(long showPlayerId)
        {
            this.showPlayerId = showPlayerId;
        }
    }
    
    public struct EventOnTeamSelectBadge : IEvent
    {
        public int selectBadgeBackId;
        public int selectBadgeIconId;

        public EventOnTeamSelectBadge(int selectBadgeBackId,int selectBadgeIconId)
        {
            this.selectBadgeBackId = selectBadgeBackId;
            this.selectBadgeIconId = selectBadgeIconId;
        }
    }
    
    public struct EventOnTeamInfoChange : IEvent
    {
        public TeamData teamData;
        public MyTeamRequestResult result;

        public EventOnTeamInfoChange(MyTeamRequestResult result, TeamData teamData)
        {
            this.result = result;
            this.teamData = teamData;
        }

    }
    
    public struct EventOnTeamLeave : IEvent
    {
    }
    
    public struct EventOnTeamJoin : IEvent
    {
    }
    
    public struct EventOnTeamKicked : IEvent
    {
    }

    public struct EventOnTeamLoading : IEvent
    {
        public bool isShow;

        public EventOnTeamLoading(bool isShow)
        {
            this.isShow = isShow;
        }
    }
    
    public struct EventOnTeamRedPointRefresh : IEvent
    {                                                                                                         
    }
    
    public struct EventOnTeamChatsRefresh : IEvent
    {
    }
    
    public struct EventOnTeamSendHelpResult : IEvent
    {
        public bool Success;
        public long HelpId;

        public EventOnTeamSendHelpResult(bool success, long helpId)
        {
            Success = success;
            HelpId = helpId;
        }
    }
    
    public struct EventOnTeamAskHelpResult : IEvent
    {
        public bool Success;
        public EventOnTeamAskHelpResult(bool success)
        {
            Success = success;
        }
    }
    
    public struct EventOnTeamReceivePassGiftResult : IEvent
    {
        public bool Success;
        public long GiftId;

        public EventOnTeamReceivePassGiftResult(bool success, long giftId)
        {
            Success = success;
            GiftId = giftId;
        }
    }
    
    public struct EventOnTeamSendLikeResult : IEvent
    {
        public bool Success;
        public long LikeId;
        public int LikeType;

        public EventOnTeamSendLikeResult(bool success, long likeId, int likeType)
        {
            Success = success;
            LikeId = likeId;
            LikeType = likeType;
        }
    }
    
    public struct EventOnTeamSendEnergyGiftResult : IEvent
    {
        public bool Success;
        public long GiftId;

        public EventOnTeamSendEnergyGiftResult(bool success, long giftId)
        {
            Success = success;
            GiftId = giftId;
        }
    }

    public struct EventOnTeamGetRecommendListResult : IEvent
    {
        public bool Success;

        public EventOnTeamGetRecommendListResult(bool success)
        {
            Success = success;
        }
    }

    public struct EventOnTeamServerInitSuccess : IEvent
    {
        
    }
}