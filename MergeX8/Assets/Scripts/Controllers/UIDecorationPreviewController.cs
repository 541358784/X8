using System;
using System.Linq;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIDecorationPreviewController : UIWindowController
{
    public static UIDecorationPreviewController Open(Action callback)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIDecorationPreview, callback) as
            UIDecorationPreviewController;
    }

    private Action Callback;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs.Length>0)
            Callback = objs[0] as Action;
    }

    public override void PrivateAwake()
    {
        var closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        closeButton.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.NODE_PREVIEW_END);
                Callback?.Invoke();
            });
        });
    }
}