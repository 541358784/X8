using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_ClimbTree : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    // private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    
    protected override void Awake()
    {
        base.Awake();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
        
        InvokeRepeating("UpdateUI", 0, 1);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClimbTreeStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(ClimbTreeModel.Instance.IsPrivateOpened());
        
        if (!gameObject.activeSelf)
            return;
        
        _timeText.SetText(ClimbTreeModel.Instance.GetActivityLeftTimeString());

        var unCollectRewardsCount = ClimbTreeModel.Instance.GetCurrentUnCollectRewards().Count;
        _redPoint.gameObject.SetActive(unCollectRewardsCount > 0);
        _redPointText.SetText(unCollectRewardsCount.ToString());
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ClimbTreeLeaderBoardHomeEntrance) &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            ClimbTreeModel.Instance.CurStorageClimbTreeWeek.LeaderBoardStorage.IsInitFromServer() && 
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home || 
             SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClimbTreeLeaderBoardHomeEntrance, transform as RectTransform, topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClimbTreeLeaderBoardHomeEntrance, ""))
            {
                GuideSubSystem.Instance.ForceFinished(735);   
            }
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClimbTreeLeaderBoardHomeEntrance);
        if (!ClimbTreeModel.Instance.IsPrivateOpened())
            return;
        // if (ClimbTreeModel.Instance.CanShowStartView())
        // {
        //     UIManager.Instance.OpenUI(UINameConst.UIClimbTreeStart);
        //     return;
        // }
      
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClimbTreeStart, null);
        UIManager.Instance.OpenUI(UINameConst.UIClimbTreeMain);
    }
    private void OnDestroy()
    {
    }
}
