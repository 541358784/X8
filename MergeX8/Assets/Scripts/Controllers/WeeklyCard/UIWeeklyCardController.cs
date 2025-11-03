
using UnityEngine.UI;

public class UIWeeklyCardController : UIWindowController
{
    private Button _closeBtn;
    private WeeklyCardItem _ordinary;
    private WeeklyCardItem _senior;
    private DailyBundleController _dailyBundleController;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnCloseBtn);
        _ordinary = transform.Find("Root/Ordinary").GetOrCreateComponent<WeeklyCardItem>();
        _senior = transform.Find("Root/Senior").GetOrCreateComponent<WeeklyCardItem>();
        _dailyBundleController = transform.Find("Root/DailyBundle").GetOrCreateComponent<DailyBundleController>();
        Init();
    }

    private void Init()
    {
        WeeklyCardModel.Instance.RefreshWeeklyCard();
        var configs=GlobalConfigManager.Instance.tableWeeklyCards;
        _ordinary.Init(configs[0]);
        _senior.Init(configs[1]);
        _dailyBundleController.Init();
    }
    private void OnCloseBtn()
    {
        AnimCloseWindow();
    }
        
    private static string coolTimeKey = "WeekCard";
    public static bool CanShowUI()
    {

        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.WeeklyCard))
            return false;

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            UIManager.Instance.OpenUI(UINameConst.UIWeeklyCard);
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
}
