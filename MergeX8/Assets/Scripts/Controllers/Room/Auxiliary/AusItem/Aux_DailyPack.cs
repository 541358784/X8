using System.Collections;

using DragonPlus;

public class Aux_DailyPack : Aux_ItemBase
{
    private IEnumerator dailyIEnumerator = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;

    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEventListener(EventEnum.Daily_Pack_Begin, OnPackBegin);
        DailyPackModel.Instance.RefreshPack();
        InvokeRepeating("UpdateUI", 0, 1);
    }


    public override void UpdateUI()
    {
        gameObject.SetActive(DailyPackModel.Instance.IsOpen() );
        if (gameObject.activeSelf)
        {
            int leftTime = DailyPackModel.Instance.GetPackCoolTime() * 1000;
            if (leftTime <= 0)
                DailyPackModel.Instance.RefreshPack();
            _timeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
            // StopDelayWork();
            // dailyIEnumerator = CommonUtils.DelayWork(1, () => { UpdateUI(); });
            // StartCoroutine(dailyIEnumerator);
        }
    }

    private void StopDelayWork()
    {
        if (dailyIEnumerator == null)
            return;

        StopCoroutine(dailyIEnumerator);
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();   
        DailyPackModel.Instance.RefreshPack();
        UIManager.Instance.OpenUI(UINameConst.UIDailyPack2, "pack");
    }


    private void OnPackBegin(BaseEvent e)
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        StopDelayWork();
        EventDispatcher.Instance.RemoveEventListener(EventEnum.Daily_Pack_Begin, OnPackBegin);
    }
}