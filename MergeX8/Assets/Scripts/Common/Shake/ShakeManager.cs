using System.Collections;
using System.Collections.Generic;
using DragonPlus.Haptics;
using UnityEngine;

public class ShakeManager : Manager<ShakeManager>
{
    public bool ShakeClose => SettingManager.Instance.ShakeClose;

    //震动
    public void ShakeLight()
    {
        if (ShakeClose)
            return;

        HapticsManager.Haptics(HapticTypes.Light);
    }

    public void ShakeSelection()
    {
        if (ShakeClose)
            return;
        HapticsManager.Haptics(HapticTypes.Selection);
    }

    public void ShakeMedium()
    {
        if (ShakeClose)
            return;

        HapticsManager.Haptics(HapticTypes.Medium);
    }
}