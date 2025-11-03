using System;
using System.ComponentModel;
using Activity.DiamondRewardModel.Model;


public partial class SROptions
{
    private const string DiamondReward = "钻石抽奖";
    [Category(DiamondReward)]
    [DisplayName("重置钻石抽奖")]
    public void RestDiamondReward()
    {
        HideDebugPanel();
        DiamondRewardModel.Instance.DiamondReward.Clear();
    }
    
    [Category(DiamondReward)]
    [DisplayName("池子id")]
    public int diamondRewardPoolId
    {
        get { return DiamondRewardModel.Instance.DiamondReward.PoolId; }
        set
        {
            DiamondRewardModel.Instance.DiamondReward.PoolId = value;
        }
    }
    
    [Category(DiamondReward)]
    [DisplayName("池子索引")]
    public int diamondRewardPoolIndex
    {
        get { return DiamondRewardModel.Instance.DiamondReward.PoolIndex; }
    }
    
    [Category(DiamondReward)]
    [DisplayName("是否忽略弹窗")]
    public bool isIgnorePopUI
    {
        get { return DiamondRewardModel.Instance.DiamondReward.IsIgnorePopUI; }
        set
        {
            DiamondRewardModel.Instance.DiamondReward.IsIgnorePopUI = value;
        }
    }
}