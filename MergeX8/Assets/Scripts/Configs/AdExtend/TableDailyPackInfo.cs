using System;
using System.Collections.Generic;

[System.Serializable]
public class DailyPackInfo : TableBase
{   
    // ID
    public int Id { get; set; }// 包含内容
    public List<int> Contain { get; set; }// 包含内容数量
    public List<int> Contain_num { get; set; }// 左侧图标名字
    public string Imag { get; set; }// 礼包标题
    public string Name { get; set; }// 0-普通; 1-打折促销; 2-最受欢迎
    public int Best_deal { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
