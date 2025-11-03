public partial class UIManager
{
    public UIWindow OpenWindow(string path, params object[] objs)
    {
        return OpenUI(path, objs);
    }
}

public partial class UINameConst
{
    #region TileMatch
    public static readonly string TileMatchMain = "TileMatch/Prefabs/TileMatchMain";
    public static readonly string UIPopupTileMatchFail = "TileMatch/Prefabs/UIPopupTileMatchFail";
    public static readonly string UITileMatchSuccess = "TileMatch/Prefabs/UITileMatchSuccess";
    public static readonly string UIPopupBuyItem = "TileMatch/Prefabs/UIPopupBuyItem";
    public static readonly string UIPopupBuyHp = "TileMatch/Prefabs/UIPopupBuyHp";
    public static readonly string UIPopupDifficulty = "TileMatch/Prefabs/UIPopupDifficulty";
    public static readonly string UIPopupRemoveGuide = "TileMatch/Prefabs/UIPopupRemoveGuide";
    public static readonly string UIItemsUnlocked = "TileMatch/Prefabs/UIItemsUnlocked";
    
    #endregion
}