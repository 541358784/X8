using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPopupTurtlePangNoItemController:UIWindowController
{
    private Button CloseBtn;
    private Button StartBtn;
    private Button BuyBtn;
    private LocalizeTextMeshProUGUI NumText;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(()=>AnimCloseWindow());
        StartBtn = GetItem<Button>("Root/ButtonPlay");
        StartBtn.onClick.AddListener(()=>
        {
            var mainUI = UITurtlePangMainController.Instance;
            if (mainUI)
                mainUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        });
        BuyBtn = GetItem<Button>("Root/ButtonBuy");
        BuyBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                UITurtlePangGiftBagController.Open();
            });
        });
        NumText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var storage = objs[0] as StorageTurtlePang;
        NumText.SetText(storage.PackageCount.ToString());
    }

    public static UIPopupTurtlePangNoItemController Open(StorageTurtlePang storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupTurtlePangNoItem,storage) as
            UIPopupTurtlePangNoItemController;
    }
}