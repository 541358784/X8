
using DragonPlus;
using TotalRecharge_New;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupTotalRecharge_NewController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    private Button _buttonClose;
    private Transform taskItem;

    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/TimeGroup/TimeText");
        _buttonClose = GetItem<Button>("Root/TopGroup/CloseButton");
        _buttonClose.onClick.AddListener(OnBtnClose);
        taskItem = transform.Find("Root/ContentGroup/ScrollView/Viewport/Content/CollectionTaskItem");
        taskItem.gameObject.SetActive(false);
        InvokeRepeating("RefreshTime", 0, 1);
        Init();

        var tipBtn = transform.Find("Root/TipBtn").GetComponent<Button>();
        tipBtn.onClick.AddListener(() =>
        {
            tipBtn.gameObject.SetActive(false);
            tipBtn.gameObject.SetActive(true);
        });
    }

    public void Init()
    {
        var configs = GlobalConfigManager.Instance.GatTotalRechargeConfig();
        for (int i = 0; i < configs.Count; i++)
        {
            var obj= Instantiate(taskItem, taskItem.parent);
            var scr= obj.gameObject.AddComponent<TotalRechargeTaskItem_New>();
            scr.gameObject.SetActive(true);
            scr.Init(configs[i]);
        }
    }

    public void RefreshTime()
    {
        _timeText.SetText(TotalRechargeModel_New.Instance.GetLeftTimeString());
        if (!TotalRechargeModel_New.Instance.IsOpen())
        {
            AnimCloseWindow();
        }
    }
    
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
}
