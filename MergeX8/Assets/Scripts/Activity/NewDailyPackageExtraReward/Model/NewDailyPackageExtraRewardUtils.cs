using DragonU3DSDK.Storage;

public static class NewDailyPackageExtraRewardUtils
{
    public static void AddSkinUIWindowInfo(this StorageNewDailyPackageExtraReward weekStorage)
    {
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UINewDailyPackageExtraReward), UIWindowLayer.Normal,
            true);
    }
    public static string GetSkinName(this StorageNewDailyPackageExtraReward weekStorage)
    {
        if (weekStorage == null)
            return "Skin1";
        if (weekStorage.SkinName == "")
            weekStorage.SkinName = "Skin1";
        return weekStorage.SkinName;
    }

    public const string ConnectKeyWord = "llllll";
    public static string GetAssetPathWithSkinName(this StorageNewDailyPackageExtraReward weekStorage, string assetBasePath)
    {
        // Debug.LogError("周期storage.SkinName="+weekStorage.GetSkinName());
        return assetBasePath.Replace("/NewDailyPackageExtraReward/",
            "/NewDailyPackageExtraReward" + NewDailyPackageExtraRewardUtils.ConnectKeyWord + weekStorage.GetSkinName() + "/");
    }

    public static string GetAtlasNameWithSkinName(this StorageNewDailyPackageExtraReward weekStorage)
    {
        return "NewDailyPackageExtraReward" + NewDailyPackageExtraRewardUtils.ConnectKeyWord + weekStorage.GetSkinName() + "Atlas";
    }
    
    public static string GetAuxItemAssetPath(this StorageNewDailyPackageExtraReward storage)
    {
        return storage.GetAssetPathWithSkinName("Prefabs/Activity/NewDailyPackageExtraReward/Aux_NewDailyPackageExtraReward");
    }
}