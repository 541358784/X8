using System;
using System.Collections.Generic;

[System.Serializable]
public class MixMasterGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 礼包限购次数
    public int GiftBagBuyLimit { get; set; }// 配方版本
    public int FormulaVersion { get; set; }// 调制材料数量
    public int MaterialNeedCount { get; set; }// 奖励ID
    public List<int> FailRewardId { get; set; }// 奖励数量
    public List<int> FailRewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
