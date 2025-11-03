using UnityEngine;

namespace Decoration
{
    public class DeviceCheck
{
    public static bool GraphicMemeryLeak()
    {
        if (
            //小米
            SystemInfo.deviceModel.Contains("Redmi S2") ||
            SystemInfo.deviceModel.Contains("Redmi Y2") ||
            SystemInfo.deviceModel.Contains("Redmi 8A") ||
            SystemInfo.deviceModel.Contains("Redmi 7A") ||
            //moto
            SystemInfo.deviceModel.Contains("moto g(5)") ||
            SystemInfo.deviceModel.Contains("Moto G (5S)") ||
            SystemInfo.deviceModel.Contains("Moto G (5) Plus") ||
            SystemInfo.deviceModel.Contains("moto g(6)") ||
            SystemInfo.deviceModel.Contains("Moto Z Play Droid") ||
            SystemInfo.deviceModel.Contains("moto g(7)") ||
            SystemInfo.deviceModel.Contains("Moto Z (2) Play") ||
            SystemInfo.deviceModel.Contains("Moto Z Play") ||
            SystemInfo.deviceModel.Contains("REVVLRY") ||
            SystemInfo.deviceModel.Contains("XT1799-2") ||
            SystemInfo.deviceModel.Contains("moto e5 plus") ||
            SystemInfo.deviceModel.Contains("Moto G (5th Gen)") ||
            //三星
            SystemInfo.deviceModel.Contains("SM-J610FN") ||
            SystemInfo.deviceModel.Contains("Galaxy J4") ||
            SystemInfo.deviceModel.Contains("Galaxy A6+") ||
            SystemInfo.deviceModel.Contains("Galaxy J8") ||
            //LG
            SystemInfo.deviceModel.Contains("LG Stylo 4") ||
            SystemInfo.deviceModel.Contains("LG Q7+") ||
            SystemInfo.deviceModel.Contains("L-03K") ||
            SystemInfo.deviceModel.Contains("Q8") ||
            SystemInfo.deviceModel.Contains("QStylus") ||
            SystemInfo.deviceModel.Contains("LG Q6") ||
            SystemInfo.deviceModel.Contains("DM-01K") ||
            SystemInfo.deviceModel.Contains("LG Stylus2 Plus") ||

            //华为
            SystemInfo.deviceModel.Contains("HUAWEI MAR-LX2") ||
            SystemInfo.deviceModel.Contains("HUAWEI STK-L22") ||

            //other
            SystemInfo.deviceModel.Contains("vsmart") ||
            SystemInfo.deviceModel.Contains("VIBE K6 Power") ||
            SystemInfo.deviceModel.Contains("Aquaris V") ||
            SystemInfo.deviceModel.Contains("honor 6A Pro") ||
            SystemInfo.deviceModel.Contains("Y7")
        )
        {
            return true;
        }

        return false;
    }
    }
}
