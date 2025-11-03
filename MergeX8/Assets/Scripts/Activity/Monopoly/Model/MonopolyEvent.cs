using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string MONOPOLY_SCORE_CHANGE = "MONOPOLY_SCORE_CHANGE";
    public const string MONOPOLY_DICE_COUNT_CHANGE = "MONOPOLY_DICE_COUNT_CHANGE";
    public const string MONOPOLY_CARD_COUNT_CHANGE = "MONOPOLY_CARD_COUNT_CHANGE";
    public const string MONOPOLY_SCORE_MULTI_CHANGE = "MONOPOLY_SCORE_MULTI_CHANGE";
    public const string MONOPOLY_STEP_MULTI_CHANGE = "MONOPOLY_STEP_MULTI_CHANGE";
    public const string MONOPOLY_BUY_STORE_ITEM = "MONOPOLY_BUY_STORE_ITEM";
    public const string MONOPOLY_LEVEL_UP = "MONOPOLY_LEVEL_UP";
    public const string MONOPOLY_UI_MOVE_STEP = "MONOPOLY_UI_MOVE_STEP";
    public const string MONOPOLY_UI_GET_BLOCK_SCORE = "MONOPOLY_UI_GET_BLOCK_SCORE";
    public const string MONOPOLY_UI_GET_BLOCK_REWARD = "MONOPOLY_UI_GET_BLOCK_REWARD";
    public const string MONOPOLY_UI_THROW_MULTIPLE_DICE = "MONOPOLY_UI_THROW_MULTIPLE_DICE";//扔多个骰子
    public const string MONOPOLY_UI_GET_CARD = "MONOPOLY_UI_GET_CARD";//获得卡弹窗表演
    public const string MONOPOLY_UI_BUY_BLOCK = "MONOPOLY_UI_BUY_BLOCK";//购买地块
    public const string MONOPOLY_UI_PLAY_MINI_GAME = "MONOPOLY_UI_PLAY_MINI_GAME";//玩小游戏
    public const string MONOPOLY_UI_ADD_REWARD_BOX_SCORE = "MONOPOLY_UI_ADD_REWARD_BOX_SCORE";//增加宝箱积分
    public const string MONOPOLY_UI_COLLECT_REWARD_BOX = "MONOPOLY_UI_COLLECT_REWARD_BOX";//领取宝箱奖励
    public const string MONOPOLY_UI_POP_BUY_BLOCK = "MONOPOLY_UI_POP_BUY_BLOCK";//弹出购买地块弹窗
    public const string MONOPOLY_UI_BET_CHANGE = "MONOPOLY_UI_BET_CHANGE";//押注变化
}
public class EventMonopolyScoreChange : BaseEvent
{
    public int ChangeValue;
    public bool NeedWait;

    public EventMonopolyScoreChange() : base(EventEnum.MONOPOLY_SCORE_CHANGE) { }

    public EventMonopolyScoreChange(int changeValue,bool needWait = false) : base(EventEnum.MONOPOLY_SCORE_CHANGE)
    {
        ChangeValue = changeValue;
        NeedWait = needWait;
    }
}

public class EventMonopolyDiceCountChange : BaseEvent
{
    public int ChangeValue;
    public int TotalValue;
    public EventMonopolyDiceCountChange() : base(EventEnum.MONOPOLY_DICE_COUNT_CHANGE) { }

    public EventMonopolyDiceCountChange(int changeValue,int totalValue) : base(EventEnum.MONOPOLY_DICE_COUNT_CHANGE)
    {
        ChangeValue = changeValue;
        TotalValue = totalValue;
    }
}
public class EventMonopolyCardCountChange : BaseEvent
{
    public MonopolyCardState CardState;
    public int ChangeCount;
    public int TotalCount;

    public EventMonopolyCardCountChange() : base(EventEnum.MONOPOLY_CARD_COUNT_CHANGE) { }

    public EventMonopolyCardCountChange(MonopolyCardState cardState,int changeCount,int totalCount) : base(EventEnum.MONOPOLY_CARD_COUNT_CHANGE)
    {
        CardState = cardState;
        ChangeCount = changeCount;
        TotalCount = totalCount;
    }
}
public class EventMonopolyScoreMultiChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventMonopolyScoreMultiChange() : base(EventEnum.MONOPOLY_SCORE_MULTI_CHANGE) { }

    public EventMonopolyScoreMultiChange(int oldValue,int newValue) : base(EventEnum.MONOPOLY_SCORE_MULTI_CHANGE)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventMonopolyStepMultiChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventMonopolyStepMultiChange() : base(EventEnum.MONOPOLY_STEP_MULTI_CHANGE) { }

    public EventMonopolyStepMultiChange(int oldValue,int newValue) : base(EventEnum.MONOPOLY_STEP_MULTI_CHANGE)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventMonopolyBuyStoreItem : BaseEvent
{
    public MonopolyStoreItemConfig StoreItemConfig;

    public EventMonopolyBuyStoreItem() : base(EventEnum.MONOPOLY_BUY_STORE_ITEM) { }

    public EventMonopolyBuyStoreItem(MonopolyStoreItemConfig storeItemConfig) : base(EventEnum.MONOPOLY_BUY_STORE_ITEM)
    {
        StoreItemConfig = storeItemConfig;
    }
}
public class EventMonopolyLevelUp : BaseEvent
{
    public EventMonopolyLevelUp() : base(EventEnum.MONOPOLY_LEVEL_UP) { }
}
public class EventMonopolyUIMoveStep : BaseEvent
{
    public int StartIndex;
    public int MoveIndex;
    public EventMonopolyUIMoveStep() : base(EventEnum.MONOPOLY_UI_MOVE_STEP) { }

    public EventMonopolyUIMoveStep(int startIndex,int moveIndex) : base(EventEnum.MONOPOLY_UI_MOVE_STEP)
    {
        StartIndex = startIndex;
        MoveIndex = moveIndex;
    }
}

public class EventMonopolyUIGetBlockScore : BaseEvent
{
    public int BlockIndex;
    public MonopolyBlockConfig BlockConfig;
    public int Score;
    public EventMonopolyUIGetBlockScore() : base(EventEnum.MONOPOLY_UI_GET_BLOCK_SCORE) { }

    public EventMonopolyUIGetBlockScore(MonopolyBlockConfig blockConfig,int score,int blockIndex) : base(EventEnum.MONOPOLY_UI_GET_BLOCK_SCORE)
    {
        BlockConfig = blockConfig;
        Score = score;
        BlockIndex = blockIndex;
    }
}

public class EventMonopolyUIGetBlockReward : BaseEvent
{
    public MonopolyBlockConfig BlockConfig;
    public List<ResData> Reward;
    public EventMonopolyUIGetBlockReward() : base(EventEnum.MONOPOLY_UI_GET_BLOCK_REWARD) { }
    public EventMonopolyUIGetBlockReward(MonopolyBlockConfig blockConfig,List<ResData> reward) : base(EventEnum.MONOPOLY_UI_GET_BLOCK_REWARD)
    {
        BlockConfig = blockConfig;
        Reward = reward;
    }
}

public class EventMonopolyUIThrowMultipleDice : BaseEvent
{
    public List<MonopolyDiceConfig> DiceConfigList;
    public EventMonopolyUIThrowMultipleDice() : base(EventEnum.MONOPOLY_UI_THROW_MULTIPLE_DICE) { }
    public EventMonopolyUIThrowMultipleDice(List<MonopolyDiceConfig> diceConfigList) : base(EventEnum.MONOPOLY_UI_THROW_MULTIPLE_DICE)
    {
        DiceConfigList = diceConfigList;
    }
}

public class EventMonopolyUIGetCard : BaseEvent
{
    public MonopolyCardConfig CardConfig;
    public int BetValue;
    public EventMonopolyUIGetCard() : base(EventEnum.MONOPOLY_UI_GET_CARD) { }
    public EventMonopolyUIGetCard(MonopolyCardConfig cardConfig,int betValue) : base(EventEnum.MONOPOLY_UI_GET_CARD)
    {
        CardConfig = cardConfig;
        BetValue = betValue;
    }
}
public class EventMonopolyUIBuyBlock : BaseEvent
{
    public MonopolyBlockConfig BlockConfig;
    public int BuyTimes;
    public EventMonopolyUIBuyBlock() : base(EventEnum.MONOPOLY_UI_BUY_BLOCK) { }
    public EventMonopolyUIBuyBlock(MonopolyBlockConfig blockConfig,int buyTimes) : base(EventEnum.MONOPOLY_UI_BUY_BLOCK)
    {
        BlockConfig = blockConfig;
        BuyTimes = buyTimes;
    }
}
public class EventMonopolyUIPlayMiniGame : BaseEvent
{
    public MonopolyMiniGameConfig MiniGameConfig;
    public int BetValue;
    public EventMonopolyUIPlayMiniGame() : base(EventEnum.MONOPOLY_UI_PLAY_MINI_GAME) { }
    public EventMonopolyUIPlayMiniGame(MonopolyMiniGameConfig miniGameConfig,int betValue) : base(EventEnum.MONOPOLY_UI_PLAY_MINI_GAME)
    {
        MiniGameConfig = miniGameConfig;
        BetValue = betValue;
    }
}


public class EventMonopolyUICollectRewardBox : BaseEvent
{
    public MonopolyRewardBoxConfig RewardBoxConfig;
    public EventMonopolyUICollectRewardBox() : base(EventEnum.MONOPOLY_UI_COLLECT_REWARD_BOX) { }
    public EventMonopolyUICollectRewardBox(MonopolyRewardBoxConfig rewardBoxConfig) : base(EventEnum.MONOPOLY_UI_COLLECT_REWARD_BOX)
    {
        RewardBoxConfig = rewardBoxConfig;
    }
}

public class EventMonopolyUIAddRewardBoxScore : BaseEvent
{
    public MonopolyRewardBoxConfig RewardBoxConfig;
    public int OldValue;
    public int NewValue;
    public EventMonopolyUIAddRewardBoxScore() : base(EventEnum.MONOPOLY_UI_ADD_REWARD_BOX_SCORE) { }
    public EventMonopolyUIAddRewardBoxScore(MonopolyRewardBoxConfig rewardBoxConfig,int oldValue,int newValue) : base(EventEnum.MONOPOLY_UI_ADD_REWARD_BOX_SCORE)
    {
        RewardBoxConfig = rewardBoxConfig;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventMonopolyUIPopBuyBlock : BaseEvent
{
    public MonopolyBlockConfig BlockConfig;
    public EventMonopolyUIPopBuyBlock() : base(EventEnum.MONOPOLY_UI_POP_BUY_BLOCK) { }
    public EventMonopolyUIPopBuyBlock(MonopolyBlockConfig blockConfig) : base(EventEnum.MONOPOLY_UI_POP_BUY_BLOCK)
    {
        BlockConfig = blockConfig;
    }
}

public class EventMonopolyUIBetChange : BaseEvent
{
    public int BetValue;
    public EventMonopolyUIBetChange() : base(EventEnum.MONOPOLY_UI_BET_CHANGE) { }
    public EventMonopolyUIBetChange(int betValue) : base(EventEnum.MONOPOLY_UI_BET_CHANGE)
    {
        BetValue = betValue;
    }
}