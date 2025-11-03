using System;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public abstract class UIBlindBoxOpenBoxBase:UIWindowController
{
    public StorageBlindBox Storage;
    public BlindBoxItemConfig Config;
    private Action Callback;
    public BlindBoxModel Model => BlindBoxModel.Instance;
    public Button CloseBtn;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        InitCloseBtn();
        CloseBtn.onClick.AddListener(()=>
        {
            CloseWindowWithinUIMgr(true);
            Callback?.Invoke();
        });
        Storage = objs[0] as StorageBlindBox;
        Config = objs[1] as BlindBoxItemConfig;
        Callback = objs[2] as Action;
    }
    public abstract void InitCloseBtn();
}