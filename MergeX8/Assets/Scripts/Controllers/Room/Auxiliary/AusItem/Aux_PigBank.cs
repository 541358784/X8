using System.Collections;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

public class Aux_PigBank : Aux_ItemBase
{
    private IEnumerator dailyIEnumerator = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private Transform _full;
    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _full = transform.Find("Full");
        InvokeRepeating("UpdateUI", 0, 1);
        UpdateUI();
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(PigBankModel.Instance.IsOpened());
        _redPoint.gameObject.SetActive(PigBankModel.Instance.IsCanBuy());
        _full.gameObject.SetActive(PigBankModel.Instance.IsFull());
        if (gameObject.activeSelf)
        {
            _timeText.SetText(PigBankModel.Instance.GetActivityLeftTimeString());
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
        UIManager.Instance.OpenUI(UINameConst.UIPopupPigBox);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPiggyBankOpen);
    }


    private void OnDestroy()
    {
        StopDelayWork();
    }

}