using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string GiftBagDoubleBuyStateChange = "GiftBagDoubleBuyStateChange";
}

public class EventGiftBagDoubleBuyStateChange : BaseEvent
{
    public StorageGiftBagDouble Storage;
    public GiftBagDoubleProductConfig Product;
    public EventGiftBagDoubleBuyStateChange() : base(EventEnum.GiftBagDoubleBuyStateChange) { }

    public EventGiftBagDoubleBuyStateChange(StorageGiftBagDouble storage,GiftBagDoubleProductConfig product) : base(EventEnum.GiftBagDoubleBuyStateChange)
    {
        Storage = storage;
        Product = product;
    }
}