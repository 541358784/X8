package com.my.player;

import android.content.ComponentName;
import android.content.Context;
import android.content.pm.PackageManager;

public class AppIconBridge
{
    public static void SetAppIcon(Context context, String activityAlias)
    {
        PackageManager pm = context.getPackageManager();

        pm.setComponentEnabledSetting(
                new ComponentName(context, "com.unity3d.player.UnityPlayerActivity"),
                PackageManager.COMPONENT_ENABLED_STATE_DISABLED,
                PackageManager.DONT_KILL_APP
        );

        pm.setComponentEnabledSetting(
                new ComponentName(context, "com.my.player.IconApp"),
                PackageManager.COMPONENT_ENABLED_STATE_DISABLED,
                PackageManager.DONT_KILL_APP
        );

        if(!activityAlias.isEmpty())
        {
            pm.setComponentEnabledSetting(
                    new ComponentName(context, activityAlias),
                    PackageManager.COMPONENT_ENABLED_STATE_ENABLED,
                    PackageManager.DONT_KILL_APP);
        }
    }
}
