/************************************************
 * Config class : TreasureHuntStoreConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TreasureHuntStoreConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardCount { get; set; }// SHOPID
    public int ShopId { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
