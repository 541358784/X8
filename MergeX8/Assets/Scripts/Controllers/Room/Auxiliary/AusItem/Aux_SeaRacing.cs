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

public class Aux_SeaRacing : Aux_ItemBase
{
    // private LocalizeTextMeshProUGUI _rankText;
    private LocalizeTextMeshProUGUI _timeText;
    // private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    
    protected override void Awake()
    {
        base.Awake();
        // _rankText  = transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
        InvokeRepeating("UpdateUI", 0, 1);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SeaRacingHomeEntrance, transform as RectTransform, topLayer: topLayer);
        gameObject.SetActive(false);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(SeaRacingModel.Instance.IsPrivateOpened() && !SeaRacingModel.Instance.CurStorageSeaRacingWeek.IsFinish);
        if (!gameObject.activeSelf)
            return;

        if (!SeaRacingModel.Instance.IsStart())
        {
            _timeText.SetText(SeaRacingModel.Instance.CurStorageSeaRacingWeek.GetPreheatLeftTimeText());
            // _rankText.SetText("1");  
            ShowIcon(1);
        }
        else if (SeaRacingModel.Instance.IsStart() && SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound() != null)
        {
            _timeText.SetText(SeaRacingModel.Instance.CurStorageSeaRacingWeek.GetLeftTimeText());
            // _rankText.SetText(SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound().SortController().MyRank.ToString());   
            ShowIcon(SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound().SortController().MyRank);
        }
        else
        {
            _timeText.SetText(SeaRacingModel.Instance.CurStorageSeaRacingWeek.GetLeftTimeText());
            // _rankText.SetText("1");
            ShowIcon(1);
        }
    }

    public void ShowIcon(int rank)
    {
        for (var i = 1; i <= 3; i++)
        {
            transform.Find("Icon" + i).gameObject.SetActive(i == rank);
        }
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (!(SeaRacingModel.Instance.IsPrivateOpened()&&!SeaRacingModel.Instance.CurStorageSeaRacingWeek.IsFinish))
        {
            return;
        }
        if (!SeaRacingModel.Instance.IsStart())
        {
            SeaRacingModel.CanShowPreheatPopup();
            return;
        }
        if (SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound() == null)
            return;
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SeaRacingHomeEntrance, null);
        if (SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound().IsStart)
        {
            SeaRacingModel.CanShowMainPopup();   
        }
        else
        {
            SeaRacingModel.CanShowStartPopup();
        }
    }
    private void OnDestroy()
    {
    }
}
