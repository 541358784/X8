using System;
using UnityEngine.UI;


public class UIPopupPhotoAlbumExitController:UIWindowController
{
    public static UIPopupPhotoAlbumExitController Open(Action<bool> callback)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupPhotoAlbumExit, callback) as UIPopupPhotoAlbumExitController;
    }
    private Button Btn;
    private Button CloseBtn;
    public Action<bool> Callback;
    public override void PrivateAwake()
    {
        Btn = GetItem<Button>("Root/Button");
        Btn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
            Callback(true);
        });
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
            Callback(false);
        });
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Callback = objs[0] as Action<bool>;
    }
}