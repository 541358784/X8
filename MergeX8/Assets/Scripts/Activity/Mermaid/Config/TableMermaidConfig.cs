using System;
using System.Collections.Generic;

[System.Serializable]
public class MermaidConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 延期购买等待时间
    public int ExtendBuyWaitTime { get; set; }// 延期购买时间
    public int ExtendBuyTime { get; set; }// 延期购买SHOPID
    public int ExtendBuyShopId { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
