using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPopupDogPlayController : UIWindowController
{
    public static UIPopupDogPlayController Instance;
    public static UIPopupDogPlayController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupDogPlay) as UIPopupDogPlayController;
        return Instance;
    }

    private Slider Slider;
    private Button CollectBtn;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI SliderText;
    private StorageDogPlay Storage => DogPlayModel.Instance.Storage;
    
    public override void PrivateAwake()
    {
        Slider = transform.Find("Root/Slider").GetComponent<Slider>();
        SliderText = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        CollectBtn = transform.Find("Root/Button").GetComponent<Button>();
        CollectBtn.onClick.AddListener(async () =>
        {
            AnimCloseWindow();
            DogPlayModel.Instance.HideTaskItemGroup = true;
            await DogPlayModel.Instance.CollectRewards();
            DogPlayModel.Instance.HideTaskItemGroup = false;
            MergeDogPlay.Instance?.PerformInitFly();
        });
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
            if (PerformCloseEffect)
            {
                DogPlayModel.Instance.HideTaskItemGroup = true;
                foreach (var pair in Storage.OrderActiveState)
                {
                    var taskItem = MergeTaskTipsController.Instance.GetTaskItem(pair.Key);
                    if (!taskItem)
                        continue;
                    var target = taskItem.DogPlayGroup;
                    target.Refresh();
                }
                DogPlayModel.Instance.HideTaskItemGroup = false;
                MergeDogPlay.Instance?.PerformInitFly();
            }
        });
    }

    public void RefreshView()
    {
        Slider.value = (float)Storage.CurCount / Storage.MaxCount;
        SliderText.SetText(Storage.CurCount + "/" + Storage.MaxCount);
        CollectBtn.interactable = Storage.CurCount >= Storage.MaxCount;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        RefreshView();
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
    }

    private bool PerformCloseEffect = false;
    public void CloseEffect()
    {
        PerformCloseEffect = true;
    }
}