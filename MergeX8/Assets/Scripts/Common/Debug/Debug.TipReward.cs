using System.ComponentModel;
using ActivityLocal.TipReward.Module;
using Difference;
using DragonU3DSDK.Storage;
using Merge.Order;

public partial class SROptions
{
    private const string TipReward = "1TipReward";

    [Category(TipReward)]
    [DisplayName("CD 时间")]
    public string TipReward_CoolTime
    {
        get
        {
            return TipRewardModule.Instance.GetCoolTime().ToString();
        }
    }
    
    [Category(TipReward)]
    [DisplayName("强制开启小费奖励")]
    public bool ForceOpenTipsReward
    {
        get
        {
            return TipRewardModule.Instance.IsDebugOpen;
        }
        set
        {
            TipRewardModule.Instance.IsDebugOpen = value;
        }
    }
}