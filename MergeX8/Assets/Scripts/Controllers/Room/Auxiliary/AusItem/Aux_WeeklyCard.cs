using System.Collections;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Game;
using Manager;
using UnityEngine.UI;

public class Aux_WeekCard : Aux_ItemBase
{
    private Image redPoint;
    private LocalizeTextMeshProUGUI redPointLabel;
    private LocalizeTextMeshProUGUI timeLabel;
    

    protected override void Awake()
    {
        base.Awake();

        redPoint = transform.Find("RedPoint").GetComponent<Image>();
        redPointLabel = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        timeLabel = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        redPointLabel.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
    }


    public override void UpdateUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.WeeklyCard))
        {
            gameObject.SetActive(false);
            return;
        }
        var state1 = WeeklyCardModel.Instance.IsCanClaim(GlobalConfigManager.Instance.tableWeeklyCards[0]);
        var state2 = WeeklyCardModel.Instance.IsCanClaim(GlobalConfigManager.Instance.tableWeeklyCards[1]);
        var state3 = false;
        for (var i = 1; i <= DailyBonusModel.DailyDays; ++i)
        {
            var state = DailyBonusModel.Instance.GetDailyBonusState(i);
            if (state == DailyBonusModel.BonusState.CanClaim)
            {
                state3 = true;
                break;
            }
        }
        var totalState = state1 || state2 || state3;
        gameObject.SetActive(totalState);
        
        if (!gameObject.activeSelf)
            return;
        redPoint.gameObject.SetActive(UnlockManager.IsOpen(UnlockManager.MergeUnlockType.WeeklyCard)&& WeeklyCardModel.Instance.IsHaveCanClaim()
                                      || (DailyBonusModel.Instance.CheckIsHaveCanClaim()&&FunctionsSwitchManager.Instance.FunctionOn(FunctionsSwitchManager.FuncType.SignIn)));
        timeLabel.transform.parent.gameObject.SetActive(WeeklyCardModel.Instance.IsBuy());
        timeLabel.SetText(WeeklyCardModel.Instance.GetRestTimeString());
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
 
        UIManager.Instance.OpenUI(UINameConst.UIWeeklyCard);
    }


 

}