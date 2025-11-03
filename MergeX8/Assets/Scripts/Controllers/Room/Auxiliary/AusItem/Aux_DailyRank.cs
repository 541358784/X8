using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_DailyRank : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
        InvokeRepeating("UpdateRobotInfo", 10, 10);
    }

    public override void UpdateUI()
    {
        if(BackHomeControl.isFristPopUI && !GuideSubSystem.Instance.IsShowingGuide())
            DailyRankModel.Instance.UpdateActivity();
        
        gameObject.SetActive(DailyRankModel.Instance.IsOpenActivity());

        if (!gameObject.activeSelf)
            return;
    
        _timeText.SetText(DailyRankModel.Instance.GetActiveTime());
        _redPoint.gameObject.SetActive(false);
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();

        if (DailyRankModel.Instance._curDailyRank == null)
            return;
        
        if (!DailyRankModel.Instance._curDailyRank.IsShowStartView)
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupLevelRankingStart);
            DailyRankModel.Instance._curDailyRank.IsShowStartView = true;
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupLevelRankingShow);
        }
        
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDailyRankPop);
    }

    private void OnDestroy()
    {
    }

    private void UpdateRobotInfo()
    {
        if(!DailyRankModel.Instance.IsOpenActivity())
            return;

        DailyRankModel.Instance.UpdateRobotInfo(false, null);
    }
}
