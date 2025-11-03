public static class CommonResourceLeaderBoardUtils
{
    private const string ConnectKeyWord = "llllll";
    public static string GetAssetPathWithSkinName(this SingleCommonResourceLeaderBoardConfigStruct configStruct,string assetBasePath)
    {
        // Debug.LogError("周期storage.SkinName="+weekStorage.GetSkinName());
        return assetBasePath.Replace("/CommonResourceLeaderBoard/", "/CommonResourceLeaderBoard"+ConnectKeyWord + configStruct.GlobalConfig.SkinName + "/");
    }
}