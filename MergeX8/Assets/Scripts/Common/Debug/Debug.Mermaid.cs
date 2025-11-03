using System.ComponentModel;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Storage;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;


public partial class SROptions
{
    private const string Mermaid = "美人鱼";
    
    [Category(Mermaid)]
    [DisplayName("重置美人鱼")]
    public void RestMermaid()
    {
        StorageManager.Instance.GetStorage<StorageHome>().Mermaid.Clear();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "Mermaid");
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "MermaidExtend");
        AreaId = 999;
        ResetArea();
    }

    private int addMermaidStore = 0;
    [Category(Mermaid)]
    [DisplayName("增加分数")]
    public int AddMermaidStore
    {
        get { return addMermaidStore; }
        set { addMermaidStore = value ; }
    }
    
        
    [Category(Mermaid)]
    [DisplayName("增加分数")]
    public void AddMermaidPassStore()
    {
        MermaidModel.Instance.AddScore(AddMermaidStore);

    }        
    [Category(Mermaid)]
    [DisplayName("测试奖励")]
    public void PopReward()
    {
        MermaidModel.Instance.StorageMermaid.ExchangeCount = 12;
        MermaidModel.Instance.ClaimStateReward(() =>
        {
        });


    }
}