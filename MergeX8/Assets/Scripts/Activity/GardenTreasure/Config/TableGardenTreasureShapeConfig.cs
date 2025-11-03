using System;
using System.Collections.Generic;

[System.Serializable]
public class GardenTreasureShapeConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 形状图片名字
    public string ShapeName { get; set; }// 形状图片名字
    public string RandomShapeName { get; set; }// Z轴旋转角度
    public int RotateZ { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
