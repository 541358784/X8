using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPopupBlindBoxRecycleTipController:UIWindowController
{
    public static UIPopupBlindBoxRecycleTipController Open(StorageBlindBox storage)
    {
        var openView = UIManager.Instance.GetOpenedUIByPath<UIPopupBlindBoxRecycleTipController>(UINameConst.UIPopupBlindBoxRecycleTip);
        if (openView)
        {
            openView.SetStorage(storage);
            return openView;
        }
        openView = UIManager.Instance.OpenUI(UINameConst.UIPopupBlindBoxRecycleTip,storage) as UIPopupBlindBoxRecycleTipController;
        return openView;
    }
    private BlindBoxModel Model => BlindBoxModel.Instance;
    private Button CloseBtn;
    private Button RecycleBtn;
    private LocalizeTextMeshProUGUI ValueText;
    private LocalizeTextMeshProUGUI BoxCountText;
    private StorageBlindBox Storage;
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        RecycleBtn = transform.Find("Root/Button").GetComponent<Button>();
        RecycleBtn.onClick.AddListener(() =>
        {
            Storage.RecycleRepeatItem();
            AnimCloseWindow();
            EventDispatcher.Instance.SendEventImmediately(new EventBlindBoxRecycleItem(Storage));
        });
        ValueText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        BoxCountText = transform.Find("Root/BGGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        SetStorage(objs[0] as StorageBlindBox);
    }

    public void SetStorage(StorageBlindBox storage)
    {
        Storage = storage;
        var recycleValue = Storage.GetRecycleCount(out var boxCount);
        RecycleBtn.interactable = recycleValue > 0;
        ValueText.SetText(recycleValue.ToString());
        BoxCountText.SetTermFormats(boxCount.ToString(),recycleValue.ToString());
    }
}