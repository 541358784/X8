using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UINewDailyPackageExtraRewardController : UIWindowController
{
    private LocalizeTextMeshProUGUI TimeText;
    private Button StartBtn;
    private Button CloseBtn;
    public override void PrivateAwake()
    {
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(OnClickStartBtn);
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        EventDispatcher.Instance.AddEvent<EventNewDailyPackageExtraRewardEnd>(OnActivityEnd);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventNewDailyPackageExtraRewardEnd>(OnActivityEnd);
    }

    public void OnActivityEnd(EventNewDailyPackageExtraRewardEnd evt)
    {
        AnimCloseWindow();
    }
    public void OnClickStartBtn()
    {
        AnimCloseWindow(() =>
        {
            UIPopupNewDailyGiftController.CanShowUIWithOpenWindow();
        });
    }
    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public void UpdateTime()
    {
        TimeText.SetText(NewDailyPackageExtraRewardModel.Instance.GetActivityLeftTimeString());
    }
    public static UINewDailyPackageExtraRewardController Open(StorageNewDailyPackageExtraReward storage)
    {
        return UIManager.Instance.OpenUI( storage.GetAssetPathWithSkinName(UINameConst.UINewDailyPackageExtraReward)) as UINewDailyPackageExtraRewardController;
    }
}