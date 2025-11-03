using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string KeepPetTurkey_SCORE_CHANGE = "KeepPetTurkey_SCORE_CHANGE";
    public const string KeepPetTurkey_BUY_STORE_ITEM = "KeepPetTurkey_BUY_STORE_ITEM";
}
public class EventKeepPetTurkeyScoreChange : BaseEvent
{
    public int ChangeValue;
    public bool NeedWait;

    public EventKeepPetTurkeyScoreChange() : base(EventEnum.KeepPetTurkey_SCORE_CHANGE) { }

    public EventKeepPetTurkeyScoreChange(int changeValue,bool needWait = false) : base(EventEnum.KeepPetTurkey_SCORE_CHANGE)
    {
        ChangeValue = changeValue;
        NeedWait = needWait;
    }
}

public class EventKeepPetTurkeyBuyStoreItem : BaseEvent
{
    public KeepPetTurkeyStoreItemConfig StoreItemConfig;

    public EventKeepPetTurkeyBuyStoreItem() : base(EventEnum.KeepPetTurkey_BUY_STORE_ITEM) { }

    public EventKeepPetTurkeyBuyStoreItem(KeepPetTurkeyStoreItemConfig storeItemConfig) : base(EventEnum.KeepPetTurkey_BUY_STORE_ITEM)
    {
        StoreItemConfig = storeItemConfig;
    }
}