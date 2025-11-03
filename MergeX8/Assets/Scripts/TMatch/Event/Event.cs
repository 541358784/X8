// using DragonPlus.Config.Game;
using DragonPlus.Config.TMatchShop;
namespace TMatch
{
    public class IAPSuccessEvent : BaseEvent
    {
        public Shop shop;
        public object userData;

        public IAPSuccessEvent(Shop shop, object userData) : base(EventEnum.IAPSuccess)
        {
            this.shop = shop;
            this.userData = userData;
        }
    }
    
    public class RestorePurchasesSuccessEvent : BaseEvent
    { 
        public RestorePurchasesSuccessEvent(Shop shop, object userData) : base(EventEnum.RestorePurchasesSuccess)
        {
        }
    }

    public class LobbyMainShowStateEvent : BaseEvent
    {
        public bool enable;

        public LobbyMainShowStateEvent(bool enable) : base(EventEnum.LobbyMainShowState)
        {
            this.enable = enable;
        }
    }

    public class CurrencyFlyAniEnd : BaseEvent
    {
        public ItemType itemType;

        public CurrencyFlyAniEnd(ItemType itemType) : base(EventEnum.CurrencyFlyAniEnd)
        {
            this.itemType = itemType;
        }
    }

    public class LobbyNavigationActiveTypeEvent : BaseEvent
    {
        public UILobbyNavigationBarView.UIType type;

        public LobbyNavigationActiveTypeEvent(UILobbyNavigationBarView.UIType type) : base(EventEnum
            .LobbyNavigationActiveType)
        {
            this.type = type;
        }
    }

    public class JumpToLobbyNavigationTypeEvent : BaseEvent
    {
        public UILobbyNavigationBarView.UIType type;

        public JumpToLobbyNavigationTypeEvent(UILobbyNavigationBarView.UIType type) : base(EventEnum
            .JumpToLobbyNavigationType)
        {
            this.type = type;
        }
    }

    public class OnApplicationPauseEvent : BaseEvent
    {
        public bool pause;

        public OnApplicationPauseEvent(bool pause) : base(EventEnum.OnApplicationPause)
        {
            this.pause = pause;
        }
    }

    public enum TMatchResultExecuteType
    {
        Star, //星星
        WeeklyChallenge, //周挑战
        Last, //剩余
    }

    public class TMatchResultExecuteEvent : BaseEvent
    {
        public TMatchLevelData LevelData;
        public TMatchResultExecuteType ExecuteType;

        public TMatchResultExecuteEvent(TMatchLevelData LevelData, TMatchResultExecuteType ExecuteType) : base(
            EventEnum.TMatchResultExecute)
        {
            this.LevelData = LevelData;
            this.ExecuteType = ExecuteType;
        }
    }

    public class WeeklyChellengeAddCollectCntEvent : BaseEvent
    {
        public int cnt;

        public WeeklyChellengeAddCollectCntEvent(int cnt) : base(EventEnum.WeeklyChellengeAddCollectCnt)
        {
            this.cnt = cnt;
        }
    }
}