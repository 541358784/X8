using System.Collections;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

public class Aux_BattlePass2 : Aux_ItemBase
{
    private IEnumerator dailyIEnumerator = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    protected override void Awake()
    {
        base.Awake();

        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointText = _redPoint.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPointText.gameObject.SetActive(true);
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI",0,1);
    }

    public override void UpdateUI()
    {
        bool isOpen = Activity.BattlePass_2.BattlePassModel.Instance.IsOpened();
        gameObject.SetActive(isOpen);
        if (gameObject.activeSelf)
        {
            _timeText.SetText(Activity.BattlePass_2.BattlePassModel.Instance.GetActivityLeftTimeString());
        }
        int num = Activity.BattlePass_2.BattlePassTaskModel.Instance.GetCompleteNum();
        num += Activity.BattlePass_2.BattlePassModel.Instance.CanGetRewardCount();
        _redPoint.gameObject.SetActive(num>0);
        _redPointText.SetText(num.ToString());
    }

    protected override void OnButtonClick()
    {
        if (!Activity.BattlePass_2.BattlePassModel.Instance.storageBattlePass.IsShowStart)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPop);
            UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePass2Start);
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIBattlePass2Main);
        }
        
    }
}