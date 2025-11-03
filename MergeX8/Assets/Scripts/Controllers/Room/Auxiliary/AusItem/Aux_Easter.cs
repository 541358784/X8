using System.Collections;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

public class Aux_Easter : Aux_ItemBase
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
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI",0,1);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(EasterModel.Instance.IsOpened());
        if (gameObject.activeSelf)
        {
            if (EasterModel.Instance.IsPreheating())
            {
                _timeText.SetText(EasterModel.Instance.GetActivityPreheatLeftTimeString());
            }
            else
            {
                _timeText.SetText(EasterModel.Instance.GetActivityLeftTimeString());
            }
        }
        if(BackHomeControl.isFristPopUI && !GuideSubSystem.Instance.IsShowingGuide())
            EasterModel.Instance.UpdateActivity();
 
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (EasterModel.Instance.IsPreheating())
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterAdvance);

            UIManager.Instance.OpenUI(UINameConst.UIEasterStart);
        }
        else
        {
            if (!EasterModel.Instance.IsShowStart())
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPop);
                UIManager.Instance.OpenUI(UINameConst.UIEasterStart);
            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIEasterMain);
            }
        }
    }
}