using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string BuyFlashSale = "BuyFlashSale";
}
public class EventBuyFlashSale : BaseEvent
{
    public StorageStoreItem StoreItem;

    public EventBuyFlashSale() : base(EventEnum.BuyFlashSale) { }

    public EventBuyFlashSale(StorageStoreItem storeItem) : base(EventEnum.BuyFlashSale)
    {
        StoreItem = storeItem;
    }
}