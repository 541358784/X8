using System;
using UnityEngine;

public static class BlueBlockUtils
{
    public static void BuildShapeState(TableBlueBlockShape shapeConfig,out BlueBlockType[][] shapeState)
    {
        shapeState = new BlueBlockType[shapeConfig.width][];
        for (var i = 0; i < shapeState.Length; i++)
        {
            shapeState[i] = new BlueBlockType[shapeConfig.height];
        }
        for (var i = 0; i < shapeConfig.shape.Length; i++)
        {
            var y = i / shapeConfig.width;
            var x = i%shapeConfig.width;
            shapeState[x][y] = (BlueBlockType)shapeConfig.shape[i];
        }
    }

    public static BlueBlockType GetOpposite(this BlueBlockType target)
    {
        if (target == BlueBlockType.DownConcave)
            return BlueBlockType.DownConvex;
        if (target == BlueBlockType.DownConvex)
            return BlueBlockType.DownConcave;
        if (target == BlueBlockType.UpConcave)
            return BlueBlockType.UpConvex;
        if (target == BlueBlockType.UpConvex)
            return BlueBlockType.UpConcave;
        if (target == BlueBlockType.LeftConcave)
            return BlueBlockType.LeftConvex;
        if (target == BlueBlockType.LeftConvex)
            return BlueBlockType.LeftConcave;
        if (target == BlueBlockType.RightConcave)
            return BlueBlockType.RightConvex;
        if (target == BlueBlockType.RightConvex)
            return BlueBlockType.RightConcave;
        Debug.LogError("无法获取对应类型 target="+target);
        return BlueBlockType.Empty;
    }
    public static BlueBlockType Add(this BlueBlockType back, BlueBlockType target)
    {
        if (target == BlueBlockType.Empty)
            return back;
        if (back == BlueBlockType.Empty)
        {
            Debug.LogError("格子状态叠加错误，底="+back+" 叠加="+target);
            return BlueBlockType.Empty;   
        }
        if (back == BlueBlockType.Normal)
        {
            if (target == BlueBlockType.Normal)
                return BlueBlockType.Empty;
            return target.GetOpposite();
        }
        if (back != target)
            Debug.LogError("格子状态叠加错误，底="+back+" 叠加="+target);
        return BlueBlockType.Empty;
    }

    public static BlueBlockType Reduce(this BlueBlockType back, BlueBlockType target)
    {
        if (target == BlueBlockType.Empty)
            return back;
        if (back == BlueBlockType.Normal)
        {
            Debug.LogError("格子状态移除错误，底="+back+" 移除="+target);
            return BlueBlockType.Normal;   
        }
        if (back == BlueBlockType.Empty)
        {
            return target;
        }
        if (back != target.GetOpposite())
        {
            Debug.LogError("格子状态移除错误，底="+back+" 移除="+target);
        }
        return BlueBlockType.Normal;
    }
}