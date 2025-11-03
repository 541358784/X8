using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public static class LevelUpPackageUtils
{
    public static bool IsActive(this StorageLevelUpPackageSinglePackage packageStorage)
    {
        var config = packageStorage.PackageConfig();
        if (packageStorage.BuyTimes >= config.buyTimes)
            return false;
        var curTime = APIManager.Instance.GetServerTime();
        if (curTime > packageStorage.EndTime)
            return false;
        return true;
    }

    public static TableLevelUpPackageContentConfig PackageConfig(this StorageLevelUpPackageSinglePackage packageStorage)
    {
        return LevelUpPackageModel.Instance.ContentConfig[packageStorage.PackageId];
    }

    public static ulong GetLeftTime(this StorageLevelUpPackageSinglePackage packageStorage)
    {
        var curTime = APIManager.Instance.GetServerTime();
        var leftTime = packageStorage.EndTime - curTime;
        return leftTime;
    }
}