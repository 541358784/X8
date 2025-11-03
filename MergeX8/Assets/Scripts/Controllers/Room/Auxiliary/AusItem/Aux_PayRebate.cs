using System.Collections;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

public class Aux_PayRebate : Aux_ItemBase
{
    private IEnumerator dailyIEnumerator = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        InvokeRepeating("UpdateUI", 0, 1);
        UpdateUI();
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(PayRebateModel.Instance.IsOpened());
        _redPoint.gameObject.SetActive(PayRebateModel.Instance.IsCanClaim());
        if (gameObject.activeSelf)
        {
            _timeText.SetText(PayRebateModel.Instance.GetActivityLeftTimeString());
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
        UIManager.Instance.OpenUI(UINameConst.UIPopupPayRebate);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRebateOpen);

    }


    private void OnDestroy()
    {
        StopDelayWork();
    }

}