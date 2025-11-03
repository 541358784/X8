using System;
using System.Collections.Generic;

[System.Serializable]
public class Easter2024CardConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 类型(1:分数,2:乘倍,3:多球)
    public int CardType { get; set; }// 分数
    public int Score { get; set; }// 倍数
    public int MultiValue { get; set; }// 球数量
    public int BallCount { get; set; }// 权重
    public int Weight { get; set; }// 大图资源名
    public string AssetName { get; set; }// 标题翻译表
    public string TitleText { get; set; }// 描述翻译表
    public string DescribeText { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
