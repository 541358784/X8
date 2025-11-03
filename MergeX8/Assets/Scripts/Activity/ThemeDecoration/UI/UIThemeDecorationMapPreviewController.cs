using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIThemeDecorationMapPreviewController : UIWindowController
{
    public static UIThemeDecorationMapPreviewController Open(StorageThemeDecoration storage)
    {
        return UIManager.Instance.OpenUI(storage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationMapPreview), storage) as
            UIThemeDecorationMapPreviewController;
    }

    private StorageThemeDecoration Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageThemeDecoration;
    }

    public override void PrivateAwake()
    {
        var closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        closeButton.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.NODE_PREVIEW_END);
                ThemeDecorationModel.CanShowShopUI();
            });
        });
    }
}