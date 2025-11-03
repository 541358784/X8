using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupThemeDecorationMultipleScoreController : UIWindowController
{
    public static UIPopupThemeDecorationMultipleScoreController Open(StorageThemeDecoration storage)
    {
        return UIManager.Instance.OpenUI(storage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationMultipleScore), storage) as
            UIPopupThemeDecorationMultipleScoreController;
    }
    private LocalizeTextMeshProUGUI _coolTime;
    private StorageThemeDecoration Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageThemeDecoration;
    }

    public override void PrivateAwake()
    {
        _coolTime = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        
        GetItem<Button>("Root/ButtonClose").onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        
        GetItem<Button>("Root/Button").onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        
        InvokeRepeating("InvokeUpdate", 0, 1);
    }

    private void InvokeUpdate()
    {
        _coolTime.SetText(MultipleScoreModel.Instance.GetActiveTime(MultipleScoreModel.InfluenceFuncType.ThemeDecoration));
    }
}