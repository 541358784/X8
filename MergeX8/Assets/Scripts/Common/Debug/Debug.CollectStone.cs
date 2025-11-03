using System.ComponentModel;
using Activity.CollectStone.Model;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;

public partial class SROptions
{
    private const string Stone = "1收集宝石";

    [Category(Stone)]
    [DisplayName("宝石数量")]
    public int StoneNum
    {
        get { return CollectStoneModel.Instance.GetStone(); }
        set { CollectStoneModel.Instance.CollectStone.StoneNum = value; }
    }
        
    [Category(Stone)]
    [DisplayName("重置活动-重启")]
    public void ResetStone()
    {
        CollectStoneModel.Instance.CollectStone.Clear();
    }
}