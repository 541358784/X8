using DragonU3DSDK.Storage;
using UnityEngine.UI;

public abstract class UIBlindBoxRewardBase:UIWindowController
{
    public StorageBlindBox Storage;
    public BlindBoxThemeConfig Config;
    public BlindBoxModel Model => BlindBoxModel.Instance;
    public Button CloseBtn;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        InitCloseBtn();
        CloseBtn.onClick.AddListener(()=>
        {
            CloseBtn.interactable = false;
            AnimCloseWindow();
        });
        Storage = objs[0] as StorageBlindBox;
        Config = Model.ThemeConfigDic[Storage.ThemeId];
    }
    public abstract void InitCloseBtn();
}