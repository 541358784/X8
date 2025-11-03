using System;
using System.Collections.Generic;

[System.Serializable]
public class SnakeLadderCardConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 类型(1:步数,2:分,3:分乘,4:步乘,5:任意,6:护盾)
    public int CardType { get; set; }// 步数
    public int Step { get; set; }// 分数
    public int Score { get; set; }// 分乘倍数
    public int ScoreMultiValue { get; set; }// 步乘倍数
    public int StepMultiValue { get; set; }// 权重
    public int Weight { get; set; }// 大图资源名
    public string AssetName { get; set; }// 标题翻译表
    public string TitleText { get; set; }// 描述翻译表
    public string DescribeText { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
