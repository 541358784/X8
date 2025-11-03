
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public enum ExtraOrderRewardCouponType
{
    ClimbTree=1,
    Coin=2,
    DogHope=3,
    SnakeLadder=4,
    ThemeDecoration = 5,
    Parrot=6,
    FlowerField = 7,
}
public static class ExtraOrderRewardCouponUtils
{
    
    // public static ulong Offset => 0 * XUtility.Hour;
    // public static int CurDay => (int)((APIManager.Instance.GetServerTime()-Offset) / XUtility.DayTime);
    // public static ulong CurDayLeftTime => XUtility.DayTime - ((APIManager.Instance.GetServerTime() - Offset) % XUtility.DayTime);
    // public static string CurDayLeftTimeString => CommonUtils.FormatLongToTimeStr((long)CurDayLeftTime);
    public static int GetDayId(this TableExtraOrderRewardCouponConfig config)
    {
        var offset = (ulong)config.eachDayRefreshTime*XUtility.Hour;
        var curDay = (int)((APIManager.Instance.GetServerTime()-offset) / XUtility.DayTime);
        return curDay;
    }

    public static ulong GetCurDayLeftTime(this TableExtraOrderRewardCouponConfig config)
    {
        var offset = (ulong)config.eachDayRefreshTime*XUtility.Hour;
        var curDayLeftTime = XUtility.DayTime - ((APIManager.Instance.GetServerTime() - offset) % XUtility.DayTime);
        return curDayLeftTime;
    }

    public static ulong GetLeftTime(this StorageExtraOrderRewardCouponItem storageItem)
    {
        if (storageItem.IsStart)
        {
            return storageItem.EndTime - APIManager.Instance.GetServerTime();
        }
        else
        {
            var config = ExtraOrderRewardCouponModel.Instance.Config[storageItem.CouponId];
            if (config.GetDayId() == storageItem.DayId)
            {
                return config.GetCurDayLeftTime();
            }
            else
                return 0;
        }
    }
}