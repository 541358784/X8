
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine.UI;

public class UIPopupHappyGoyuilanController: UIWindowController
{
    private Image _icon;
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _icon = GetItem<Image>("Root/Image");
        _closeBtn = GetItem<Button>("Root/ContentGroup/BGGroup/CloseButton");
        _closeBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        string iconName;
        if (objs != null && objs.Length > 0)
        {
            iconName = objs[0] as string;
            _icon.sprite=ResourcesManager.Instance.GetSpriteVariant("HappyGoAtlas", iconName+"_1");
        }
    }
}
