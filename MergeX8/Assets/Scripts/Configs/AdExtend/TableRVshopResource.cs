using System;
using System.Collections.Generic;

[System.Serializable]
public class RVshopResource : TableBase
{   
    // #
    public int Id { get; set; }// 1. 普通资源; 2. 物品; 3. 建筑; 4. 箱子; 5. 道具; 6. 混合
    public int RewardType { get; set; }// DATA表调用RESOURCEID
    public List<int> RewardID { get; set; }// 奖励数量（无限体力单位为秒）
    public List<int> Amount { get; set; }// 消耗货币类型; 1.广告; 2.钻石; 3.免费获得; 4.充值; 5.金币
    public int ConsumeType { get; set; }// 消耗参数
    public int ConsumeAmount { get; set; }// 显示ICON; COMMON_UI_DAILYRV_BG_10 蓝色; COMMON_UI_DAILYRV_BG_3 金色; COMMON_UI_DAILYRV_BG_1 紫色
    public string Icon { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
