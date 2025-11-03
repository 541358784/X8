using DragonU3DSDK.Storage;

public static class CoinRushUtils
{
    public static string GetSkinName(this StorageCoinRush weekStorage)
    {
        if (weekStorage == null)
            return "Skin1";
        if (weekStorage.SkinName == "")
            weekStorage.SkinName = "Skin1";
        return weekStorage.SkinName;
    }

    public const string ConnectKeyWord = "llllll";

    public static string GetAssetPathWithSkinName(this StorageCoinRush weekStorage, string assetBasePath)
    {
        // Debug.LogError("周期storage.SkinName="+weekStorage.GetSkinName());
        return assetBasePath.Replace("/CoinRush/",
            "/CoinRush" + CoinRushUtils.ConnectKeyWord + weekStorage.GetSkinName() + "/");
    }

    public static string GetAtlasNameWithSkinName(this StorageCoinRush weekStorage)
    {
        return "CoinRush" + CoinRushUtils.ConnectKeyWord + weekStorage.GetSkinName() + "Atlas";
    }

    public static void AddSkinUIWindowInfo(this StorageCoinRush weekStorage)
    {
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UICoinRushMain), UIWindowLayer.Normal,
            true);
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UICoinRushTaskCompleted), UIWindowLayer.Tips,false);
    }

    public static string GetAuxItemAssetPath(this StorageCoinRush storage)
    {
        return storage.GetAssetPathWithSkinName("Prefabs/Activity/CoinRush/Aux_CoinRush");
    }

    public static string GetTaskItemAssetPath(this StorageCoinRush storage)
    {
        return storage.GetAssetPathWithSkinName("Prefabs/Activity/CoinRush/TaskList_CoinRush");
    }
}