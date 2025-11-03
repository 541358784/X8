using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_DogHope : Aux_ItemBase
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
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(  DogHopeModel.Instance.IsOpenActivity()/*&& !DogHopeModel.Instance.CurStorageDogHopeWeek.IsShowEndView*/ );
        
        if (!gameObject.activeSelf)
            return;
        
        _timeText.SetText(DogHopeModel.Instance.GetActivityLeftTimeString());
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.DogHopeLeaderBoardHomeEntrance) &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            DogHopeModel.Instance.CurStorageDogHopeWeek.LeaderBoardStorage.IsInitFromServer()&& 
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home || 
             SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.DogHopeLeaderBoardHomeEntrance, transform as RectTransform, topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.DogHopeLeaderBoardHomeEntrance, ""))
            {
                GuideSubSystem.Instance.ForceFinished(732);   
            }
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.DogHopeLeaderBoardHomeEntrance);
        if (DogHopeModel.Instance.CanShowStartView())
        {
            UIManager.Instance.OpenUI(UINameConst.UIDogStart);
            return;
        }
      
        UIManager.Instance.OpenUI(UINameConst.UIDogMain);
    }

    private void OnDestroy()
    {
    }
}
