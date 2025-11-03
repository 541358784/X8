using DragonU3DSDK.Storage;

public partial class MergeManager : Manager<MergeManager>
{
    private const string adapt_key = "AdaptStackNum";
    private int[] oldStackId = new[] { 90101, 90201, 90703 };
    
    public void AdaptStackNum()
    {
        var home = StorageManager.Instance.GetStorage<StorageHome>();
            
        if(home.RcoveryRecord.ContainsKey(adapt_key))
            return;
        home.RcoveryRecord.Add(adapt_key, true);
        
        foreach (var item in storageBoard.Bags)
        {
            RestStorage(item);
        }
        
        foreach (var item in storageBoard.VipBags)
        {
            RestStorage(item);
        }
        
        foreach (var item in storageBoard.Rewards)
        {
            RestStorage(item);
        }
        
        foreach (var item in storageBoard.Items)
        {
            RestStorage(item);
        }
    }

    private void RestStorage(StorageMergeItem item)
    {
        var config = GameConfigManager.Instance.GetItemConfig(item.Id);
        if (config == null)
        {
            item.BoosterFactor = 0;
            return;
        }

        if (config.canStacking)
        {
            if (oldStackId.Contains(item.Id))
            {
                item.StackNum = item.BoosterFactor > 0 ? item.BoosterFactor : config.defaultStackNum;
                item.BoosterFactor = 0;
            }
            else
            {
                item.StackNum = config.defaultStackNum;
            }
        }
        else
        {
            item.StackNum = 0;
            item.BoosterFactor = 0;
        }
    }
}