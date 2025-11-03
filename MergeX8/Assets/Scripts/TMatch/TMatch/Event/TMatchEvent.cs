namespace TMatch
{


    public class TMatchGameChangeStateEvent : BaseEvent
    {
        public TMatchStateType stateType;
        public TMatchStateParam param;

        public TMatchGameChangeStateEvent(TMatchStateType stateType, TMatchStateParam param) : base(EventEnum
            .TMATCH_GAME_CHANGE_STATE)
        {
            this.stateType = stateType;
            this.param = param;
        }
    }

    public class TMatchGameTimePauseEvent : BaseEvent
    {
        public string name;
        public bool pause;

        public TMatchGameTimePauseEvent(string name, bool pause) : base(EventEnum.TMATCH_GAME_TIME_PAUSE)
        {
            this.name = name;
            this.pause = pause;
        }
    }

    public class TMatchTripleEvent : BaseEvent
    {
        public int id;
        public int deltaCnt; //负数表示消除 正数表示撤销

        public TMatchTripleEvent(int id, int deltaCnt) : base(EventEnum.TMATCH_GAME_TRIPLE)
        {
            this.id = id;
            this.deltaCnt = deltaCnt;
        }
    }

    public class TMatchTripleBoostEvent : BaseEvent
    {
        public int id;
        public int collectorPosIndex;

        public TMatchTripleBoostEvent(int id, int collectorPosIndex) : base(EventEnum.TMATCH_GAME_TRIPLE_BOOST)
        {
            this.id = id;
            this.collectorPosIndex = collectorPosIndex;
        }
    }

    public class TMatchTripleBoostHandleEvent : BaseEvent
    {
        public int id;
        public int cnt;

        public TMatchTripleBoostHandleEvent(int id, int cnt) : base(EventEnum.TMATCH_GAME_TRIPLE_BOOST_HANDLE)
        {
            this.id = id;
            this.cnt = cnt;
        }
    }

    public class TMatchCollectorItemChange : BaseEvent
    {
        public int cnt;

        public TMatchCollectorItemChange(int cnt) : base(EventEnum.TMATCH_COLLECTOR_ITEMS_CHANGE)
        {
            this.cnt = cnt;
        }
    }

    public class GameItemUseEvent : BaseEvent
    {
        public int itemId;
        public bool forFree;//0元购

        public GameItemUseEvent(int itemId,bool forFree = false) : base(EventEnum.GAME_ITEM_USE)
        {
            this.itemId = itemId;
            this.forFree = forFree;
        }
    }

    public class GameItemChangeEvent : BaseEvent
    {
        public int itemId;

        public GameItemChangeEvent(int itemId) : base(EventEnum.GAME_ITEM_CHANGE)
        {
            this.itemId = itemId;
        }
    }

    public class TMatchNeedCollectItemEvent : BaseEvent
    {
        public int itemId;

        public TMatchNeedCollectItemEvent(int itemId) : base(EventEnum.TMATCH_NEED_COLLECT_ITEM)
        {
            this.itemId = itemId;
        }
    }
}
