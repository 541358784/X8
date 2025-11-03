using System.ComponentModel;
using DragonU3DSDK.Storage;

public partial class SROptions
{
    private const string IconChange = "更换Icon";

    [Category(IconChange)]
    [DisplayName("显示新icon")]
    public void ChangeNew()
    {
        IconChanger.OnChangeNewIcon();
    }
    
    [Category(IconChange)]
    [DisplayName("显示老icon")]
    public void ChangeOd()
    {
        IconChanger.OnChangeOldIcon();
    }
    [Category(IconChange)]
    [DisplayName("是否改变了icon")]
    public bool IsChangeIconSuccess
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().IsChangeIconSuccess;
        }
        set
        {
            StorageManager.Instance.GetStorage<StorageHome>().IsChangeIconSuccess = value;
        }
    }
    
}