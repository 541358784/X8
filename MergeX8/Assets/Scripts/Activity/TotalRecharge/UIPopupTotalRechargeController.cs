
using Activity.TotalRecharge;
using DragonPlus;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class UIPopupTotalRechargeController : UIWindowController
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
    }

    public void Init()
    {
        var configs = TotalRechargeModel.Instance.TotalRechargeRewards();
        for (int i = 0; i < configs.Count; i++)
        {
            var obj= Instantiate(taskItem, taskItem.parent);
            var scr= obj.gameObject.AddComponent<TotalRechargeTaskItem>();
            scr.gameObject.SetActive(true);
            scr.Init(configs[i]);
        }
    }

    public void RefreshTime()
    {
        _timeText.SetText(TotalRechargeModel.Instance.GetActivityLeftTimeString());
        if (!TotalRechargeModel.Instance.IsOpen())
        {
            AnimCloseWindow();
        }
    }
    
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
}
