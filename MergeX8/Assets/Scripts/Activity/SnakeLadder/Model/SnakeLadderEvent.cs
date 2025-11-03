using System.Collections.Generic;

public partial class EventEnum
{
    public const string SNAKE_LADDER_SCORE_CHANGE = "SNAKE_LADDER_SCORE_CHANGE";
    public const string SNAKE_LADDER_EGG_COUNT_CHANGE = "SNAKE_LADDER_EGG_COUNT_CHANGE";
    public const string SNAKE_LADDER_CARD_COUNT_CHANGE = "SNAKE_LADDER_CARD_COUNT_CHANGE";
    public const string SNAKE_LADDER_SCORE_MULTI_CHANGE = "SNAKE_LADDER_SCORE_MULTI_CHANGE";
    public const string SNAKE_LADDER_STEP_MULTI_CHANGE = "SNAKE_LADDER_STEP_MULTI_CHANGE";
    public const string SNAKE_LADDER_BUY_STORE_ITEM = "SNAKE_LADDER_BUY_STORE_ITEM";
    public const string SNAKE_LADDER_LEVEL_UP = "SNAKE_LADDER_LEVEL_UP";
    public const string SNAKE_LADDER_UI_MOVE_STEP = "SNAKE_LADDER_UI_MOVE_STEP";
    public const string SNAKE_LADDER_UI_MOVE_LADDER = "SNAKE_LADDER_UI_MOVE_LADDER";
    public const string SNAKE_LADDER_UI_MOVE_SNAKE = "SNAKE_LADDER_UI_MOVE_SNAKE";
    public const string SNAKE_LADDER_UI_GET_BLOCK_SCORE = "SNAKE_LADDER_UI_GET_BLOCK_SCORE";
    public const string SNAKE_LADDER_UI_GET_BLOCK_REWARD = "SNAKE_LADDER_UI_GET_BLOCK_REWARD";
    public const string SNAKE_LADDER_UI_PLAY_TURNTABLE = "SNAKE_LADDER_UI_PLAY_TURNTABLE";//转盘转动表演
    public const string SNAKE_LADDER_UI_GET_CARD = "SNAKE_LADDER_UI_GET_CARD";//获得卡弹窗表演
}
public class EventSnakeLadderScoreChange : BaseEvent
{
    public int ChangeValue;
    public bool NeedWait;

    public EventSnakeLadderScoreChange() : base(EventEnum.SNAKE_LADDER_SCORE_CHANGE) { }

    public EventSnakeLadderScoreChange(int changeValue,bool needWait = false) : base(EventEnum.SNAKE_LADDER_SCORE_CHANGE)
    {
        ChangeValue = changeValue;
        NeedWait = needWait;
    }
}

public class EventSnakeLadderTurntableCountChange : BaseEvent
{
    public int ChangeValue;
    public int TotalValue;
    public EventSnakeLadderTurntableCountChange() : base(EventEnum.SNAKE_LADDER_EGG_COUNT_CHANGE) { }

    public EventSnakeLadderTurntableCountChange(int changeValue,int totalValue) : base(EventEnum.SNAKE_LADDER_EGG_COUNT_CHANGE)
    {
        ChangeValue = changeValue;
        TotalValue = totalValue;
    }
}
public class EventSnakeLadderCardCountChange : BaseEvent
{
    public SnakeLadderCardState CardState;
    public int ChangeCount;
    public int TotalCount;

    public EventSnakeLadderCardCountChange() : base(EventEnum.SNAKE_LADDER_CARD_COUNT_CHANGE) { }

    public EventSnakeLadderCardCountChange(SnakeLadderCardState cardState,int changeCount,int totalCount) : base(EventEnum.SNAKE_LADDER_CARD_COUNT_CHANGE)
    {
        CardState = cardState;
        ChangeCount = changeCount;
        TotalCount = totalCount;
    }
}
public class EventSnakeLadderScoreMultiChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventSnakeLadderScoreMultiChange() : base(EventEnum.SNAKE_LADDER_SCORE_MULTI_CHANGE) { }

    public EventSnakeLadderScoreMultiChange(int oldValue,int newValue) : base(EventEnum.SNAKE_LADDER_SCORE_MULTI_CHANGE)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventSnakeLadderStepMultiChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventSnakeLadderStepMultiChange() : base(EventEnum.SNAKE_LADDER_STEP_MULTI_CHANGE) { }

    public EventSnakeLadderStepMultiChange(int oldValue,int newValue) : base(EventEnum.SNAKE_LADDER_STEP_MULTI_CHANGE)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventSnakeLadderBuyStoreItem : BaseEvent
{
    public SnakeLadderStoreItemConfig StoreItemConfig;

    public EventSnakeLadderBuyStoreItem() : base(EventEnum.SNAKE_LADDER_BUY_STORE_ITEM) { }

    public EventSnakeLadderBuyStoreItem(SnakeLadderStoreItemConfig storeItemConfig) : base(EventEnum.SNAKE_LADDER_BUY_STORE_ITEM)
    {
        StoreItemConfig = storeItemConfig;
    }
}
public class EventSnakeLadderLevelUp : BaseEvent
{
    public SnakeLadderLevelConfig LevelConfig;
    public EventSnakeLadderLevelUp() : base(EventEnum.SNAKE_LADDER_LEVEL_UP) { }

    public EventSnakeLadderLevelUp(SnakeLadderLevelConfig levelConfig) : base(EventEnum.SNAKE_LADDER_LEVEL_UP)
    {
        LevelConfig = levelConfig;
    }
}
public class EventSnakeLadderUIMoveStep : BaseEvent
{
    public int StartIndex;
    public int MoveIndex;
    public EventSnakeLadderUIMoveStep() : base(EventEnum.SNAKE_LADDER_UI_MOVE_STEP) { }

    public EventSnakeLadderUIMoveStep(int startIndex,int moveIndex) : base(EventEnum.SNAKE_LADDER_UI_MOVE_STEP)
    {
        StartIndex = startIndex;
        MoveIndex = moveIndex;
    }
}
public class EventSnakeLadderUIMoveLadder : BaseEvent
{
    public int StartIndex;
    public int MoveIndex;
    public EventSnakeLadderUIMoveLadder() : base(EventEnum.SNAKE_LADDER_UI_MOVE_LADDER) { }

    public EventSnakeLadderUIMoveLadder(int startIndex,int moveIndex) : base(EventEnum.SNAKE_LADDER_UI_MOVE_LADDER)
    {
        StartIndex = startIndex;
        MoveIndex = moveIndex;
    }
}
public class EventSnakeLadderUIMoveSnake : BaseEvent
{
    public int StartIndex;
    public int MoveIndex;
    public EventSnakeLadderUIMoveSnake() : base(EventEnum.SNAKE_LADDER_UI_MOVE_SNAKE) { }

    public EventSnakeLadderUIMoveSnake(int startIndex,int moveIndex) : base(EventEnum.SNAKE_LADDER_UI_MOVE_SNAKE)
    {
        StartIndex = startIndex;
        MoveIndex = moveIndex;
    }
}

public class EventSnakeLadderUIGetBlockScore : BaseEvent
{
    public int BlockIndex;
    public SnakeLadderBlockConfig BlockConfig;
    public int Score;
    public EventSnakeLadderUIGetBlockScore() : base(EventEnum.SNAKE_LADDER_UI_GET_BLOCK_SCORE) { }

    public EventSnakeLadderUIGetBlockScore(SnakeLadderBlockConfig blockConfig,int score,int blockIndex) : base(EventEnum.SNAKE_LADDER_UI_GET_BLOCK_SCORE)
    {
        BlockConfig = blockConfig;
        Score = score;
        BlockIndex = blockIndex;
    }
}

public class EventSnakeLadderUIGetBlockReward : BaseEvent
{
    public SnakeLadderBlockConfig BlockConfig;
    public List<ResData> Reward;
    public EventSnakeLadderUIGetBlockReward() : base(EventEnum.SNAKE_LADDER_UI_GET_BLOCK_REWARD) { }
    public EventSnakeLadderUIGetBlockReward(SnakeLadderBlockConfig blockConfig,List<ResData> reward) : base(EventEnum.SNAKE_LADDER_UI_GET_BLOCK_REWARD)
    {
        BlockConfig = blockConfig;
        Reward = reward;
    }
}

public class EventSnakeLadderUIPlayTurntable : BaseEvent
{
    public SnakeLadderCardConfig CardConfig;
    public EventSnakeLadderUIPlayTurntable() : base(EventEnum.SNAKE_LADDER_UI_PLAY_TURNTABLE) { }
    public EventSnakeLadderUIPlayTurntable(SnakeLadderCardConfig cardConfig) : base(EventEnum.SNAKE_LADDER_UI_PLAY_TURNTABLE)
    {
        CardConfig = cardConfig;
    }
}

public class EventSnakeLadderUIGetCard : BaseEvent
{
    public SnakeLadderCardConfig CardConfig;
    public EventSnakeLadderUIGetCard() : base(EventEnum.SNAKE_LADDER_UI_GET_CARD) { }
    public EventSnakeLadderUIGetCard(SnakeLadderCardConfig cardConfig) : base(EventEnum.SNAKE_LADDER_UI_GET_CARD)
    {
        CardConfig = cardConfig;
    }
}