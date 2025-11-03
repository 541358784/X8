using System.ComponentModel;
using DragonU3DSDK.Storage;
using Gameplay;

public partial class SROptions
{
    private const string GiftBagProgress = "进步礼包";
    [Category(GiftBagProgress)]
    [DisplayName("重置")]
    public void ResetGiftBagProgress()
    {
        HideDebugPanel();
        foreach (var pair in StorageManager.Instance.GetStorage<StorageHome>().GiftBagProgress)
        {
            {
                if (MergeGiftBagProgressBubble_Creator.CreatorDic.TryGetValue(pair.Value, out var creator))
                {
                    creator.Release();
                }
            }
        }
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagProgress.Clear();
        if (GiftBagProgressModel.Instance.IsInitFromServer())
        {
            GiftBagProgressModel.Instance.InitStorage();
            GiftBagProgressModel.Instance.InitDailyTaskConfig();
            GiftBagProgressModel.Instance.Storage.CreateMergeBubble();   
        }
    }
    
    public int _GiftBagProgressCollectType;
    [Category(GiftBagProgress)]
    [DisplayName("类型")]
    public int GiftBagProgressCollectType
    {
        get
        {
            return _GiftBagProgressCollectType;
        }
        set
        {
            _GiftBagProgressCollectType = value;
        }
    }

    public int _GiftBagProgressCollectCount;
    [Category(GiftBagProgress)]
    [DisplayName("数量")]
    public int GiftBagProgressCollectCount
    {
        get
        {
            return _GiftBagProgressCollectCount;
        }
        set
        {
            _GiftBagProgressCollectCount = value;
        }
    }

    [Category(GiftBagProgress)]
    [DisplayName("消耗")]
    public void GiftBagProgressConsume()
    {
        GiftBagProgressModel.Instance.OnConsumeRes((UserData.ResourceId)_GiftBagProgressCollectType,_GiftBagProgressCollectCount);
    }
    [Category(GiftBagProgress)]
    [DisplayName("获得")]
    public void GiftBagProgressAddRes()
    {
        GiftBagProgressModel.Instance.OnAddRes((UserData.ResourceId)_GiftBagProgressCollectType,_GiftBagProgressCollectCount);
    }
}