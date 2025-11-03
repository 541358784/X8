using System;
using System.Collections.Generic;

[System.Serializable]
public class FlashSaleBox : TableBase
{   
    // ID
    public int Id { get; set; }// 广告组
    public int RvGroup { get; set; }// 奖励池
    public List<int> Reward { get; set; }// 奖励池权重
    public List<int> Weight { get; set; }// 星星下限
    public int StarMin { get; set; }// 星星上限
    public int StarMax { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
