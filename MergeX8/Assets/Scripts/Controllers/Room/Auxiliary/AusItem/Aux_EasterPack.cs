using System.Collections;

using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

public class Aux_EasterPack : Aux_ItemBase
{
    private IEnumerator dailyIEnumerator = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;

    protected override void Awake()
    {
        base.Awake();
        _redPoint = transform.Find("RedPoint");
        _redPoint.gameObject.SetActive(false);
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI", 0, 1);
    }


    public override void UpdateUI()
    {
        gameObject.SetActive(EasterGiftModel.Instance.IsOpened() &&  EasterModel.Instance.IsStart());
        if (gameObject.activeSelf)
        {
            _timeText.SetText(EasterGiftModel.Instance.GetActivityRewardLeftTimeString());
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
        UIManager.Instance.OpenUI(UINameConst.UIEasterPack, "Click");
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPackageOpen);

    }

    private void OnDestroy()
    {
        StopDelayWork();
    }
}