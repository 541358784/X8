using System;
using System.Collections.Generic;

[System.Serializable]
public class TMReviveGiftPackLevel : TableBase
{   
    // #
    public int Id { get; set; }// 分组ID
    public int Groupid { get; set; }// 开启等级
    public int OpenLevel { get; set; }// 初始展示第几组礼包
    public int FirstShow { get; set; }// 展示冷却（秒）
    public int Cd { get; set; }// 复活礼包组1; （组内每次等概率随机取一个）
    public List<int> Gift1 { get; set; }// 复活礼包组2
    public List<int> Gift2 { get; set; }// 复活礼包组3
    public List<int> Gift3 { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
