using System;
using System.Collections.Generic;

[System.Serializable]
public class KapiTileGlobalConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 生命值上限
    public int MaxLife { get; set; }// 生命值恢复时间(分钟)
    public int LifeRecoverTime { get; set; }// 礼包列表
    public List<int> PackageList { get; set; }// 全包SHOPID
    public int TotalShopId { get; set; }// 折扣
    public int Discount { get; set; }// 礼包扩展显示内容
    public List<int> RewardIdShow { get; set; }// 礼包扩展显示数量
    public List<int> RewardNumShow { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
