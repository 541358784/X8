using System;
using System.Collections.Generic;

[System.Serializable]
public class GiftBagProgressGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// SHOPID
    public int ShopId { get; set; }// 礼包内容ID
    public List<int> RewardId { get; set; }// 礼包内容数量
    public List<int> RewardNum { get; set; }// 显示局内入口所需完成任务数量
    public int TaskCompleteShowCount { get; set; }// 标签文本
    public string TagText { get; set; }// 任务组
    public List<int> Content { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
