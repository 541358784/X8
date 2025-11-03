using DragonPlus.Config.CardCollect;

public partial class EventEnum
{
    public const string CARD_THEME_COMPLETE = "CARD_THEME_COMPLETE";
    public const string CARD_BOOK_COMPLETE = "CARD_BOOK_COMPLETE";
    public const string COLLECT_NEW_CARD_ITEM = "COLLECT_NEW_CARD_ITEM";
    public const string CARD_STAR_VALUE_CHANGE = "CARD_STAR_VALUE_CHANGE";
    public const string VIEW_NEW_CARD = "VIEW_NEW_CARD";
    public const string GET_CARD_PACKAGE = "GET_CARD_PACKAGE";
    public const string WILD_CARD_COUNT_CHANGE = "WILD_CARD_COUNT_CHANGE";
    public const string CARD_COUNT_CHANGE = "CARD_COUNT_CHANGE";
    public const string CARD_THEME_UNLOCK = "CARD_THEME_UNLOCK";
}
public class EventCardThemeComplete : BaseEvent
{
    public CardCollectionCardThemeState CardThemeState;

    public EventCardThemeComplete() : base(EventEnum.CARD_THEME_COMPLETE) { }

    public EventCardThemeComplete(CardCollectionCardThemeState cardThemeState) : base(EventEnum.CARD_THEME_COMPLETE)
    {
        CardThemeState = cardThemeState;
    }
}
public class EventCardBookComplete : BaseEvent
{
    public CardCollectionCardBookState CardBookState;

    public EventCardBookComplete() : base(EventEnum.CARD_BOOK_COMPLETE) { }

    public EventCardBookComplete(CardCollectionCardBookState cardBookState) : base(EventEnum.CARD_BOOK_COMPLETE)
    {
        CardBookState = cardBookState;
    }
}

public class EventCollectNewCardItem : BaseEvent
{
    public CardCollectionCardItemState CardItemState;
    public GetCardSource Source;

    public EventCollectNewCardItem() : base(EventEnum.COLLECT_NEW_CARD_ITEM) { }

    public EventCollectNewCardItem(CardCollectionCardItemState cardItemState,GetCardSource source) : base(
        EventEnum.COLLECT_NEW_CARD_ITEM)
    {
        CardItemState = cardItemState;
        Source = source;
    }
}

public class EventCardStarValueChange : BaseEvent
{
    private int ChangeValue;
    public EventCardStarValueChange() : base(EventEnum.CARD_STAR_VALUE_CHANGE) { }
    public EventCardStarValueChange(int changeValue) : base(
        EventEnum.CARD_STAR_VALUE_CHANGE)
    {
        ChangeValue = changeValue;
    }
}
public class EventViewNewCard : BaseEvent
{
    public CardCollectionCardItemState CardItemState;
    public EventViewNewCard() : base(EventEnum.VIEW_NEW_CARD) { }
    public EventViewNewCard(CardCollectionCardItemState cardItemState) : base(
        EventEnum.VIEW_NEW_CARD)
    {
        CardItemState = cardItemState;
    }
}
public class EventGetCardPackage : BaseEvent
{
    public TableCardCollectionCardPackage CardPackage;
    public EventGetCardPackage() : base(EventEnum.GET_CARD_PACKAGE) { }
    public EventGetCardPackage(TableCardCollectionCardPackage cardPackage) : base(
        EventEnum.GET_CARD_PACKAGE)
    {
        CardPackage = cardPackage;
    }
}
public class EventWildCardCountChange : BaseEvent
{
    public TableCardCollectionWildCard WildCard;
    public int ChangeValue;
    public EventWildCardCountChange() : base(EventEnum.WILD_CARD_COUNT_CHANGE) { }
    public EventWildCardCountChange(TableCardCollectionWildCard wildCard,int changeValue) : base(
        EventEnum.WILD_CARD_COUNT_CHANGE)
    {
        WildCard = wildCard;
        ChangeValue = changeValue;
    }
}

public class EventCardCountChange : BaseEvent
{
    public CardCollectionCardItemState CardItem;
    public EventCardCountChange() : base(EventEnum.CARD_COUNT_CHANGE) { }
    public EventCardCountChange(CardCollectionCardItemState cardItem) : base(
        EventEnum.CARD_COUNT_CHANGE)
    {
        CardItem = cardItem;
    }
}

public class EventCardThemeUnLock : BaseEvent
{
    public CardCollectionCardThemeState CardTheme;
    public EventCardThemeUnLock() : base(EventEnum.CARD_THEME_UNLOCK) { }
    public EventCardThemeUnLock(CardCollectionCardThemeState cardTheme) : base(
        EventEnum.CARD_THEME_UNLOCK)
    {
        CardTheme = cardTheme;
    }
}