using System;
using System.Collections.Generic;

[System.Serializable]
public class TMIceBreakPackChain : TableBase
{   
    // 编号
    public int Id { get; set; }// 分组
    public int Groupid { get; set; }// 礼包名称
    public string PackName { get; set; }// 礼包描述
    public string PackDesc { get; set; }// 礼包列表
    public List<int> ChainPacks { get; set; }// 持续时间（秒）
    public int Duration { get; set; }// 前置CD（秒）; 再次开启时检查
    public int Cd { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
