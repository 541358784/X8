using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string PhotoAlbum_SCORE_CHANGE = "PhotoAlbum_SCORE_CHANGE";
    public const string PhotoAlbum_BUY_STORE_ITEM = "PhotoAlbum_BUY_STORE_ITEM";
}
public class EventPhotoAlbumScoreChange : BaseEvent
{
    public int ChangeValue;
    public bool NeedWait;

    public EventPhotoAlbumScoreChange() : base(EventEnum.PhotoAlbum_SCORE_CHANGE) { }

    public EventPhotoAlbumScoreChange(int changeValue,bool needWait = false) : base(EventEnum.PhotoAlbum_SCORE_CHANGE)
    {
        ChangeValue = changeValue;
        NeedWait = needWait;
    }
}

public class EventPhotoAlbumBuyStoreItem : BaseEvent
{
    public PhotoAlbumStoreItemConfig StoreItemConfig;

    public EventPhotoAlbumBuyStoreItem() : base(EventEnum.PhotoAlbum_BUY_STORE_ITEM) { }

    public EventPhotoAlbumBuyStoreItem(PhotoAlbumStoreItemConfig storeItemConfig) : base(EventEnum.PhotoAlbum_BUY_STORE_ITEM)
    {
        StoreItemConfig = storeItemConfig;
    }
}