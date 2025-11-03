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

public class Aux_CoinLeaderBoard : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _rankText;
    private LocalizeTextMeshProUGUI _timeText;
    // private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    
    protected override void Awake()
    {
        base.Awake();
        _rankText  = transform.Find("LvText").GetComponent<LocalizeTextMeshProUGUI>();
   
        _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("RedPoint");
        _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
        
        InvokeRepeating("UpdateUI", 0, 1);
        InvokeRepeating("UpdateTime", 0, 1);
        
        // List<Transform> topLayer = new List<Transform>();
        // topLayer.Add(transform);
        // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CoinLeaderBoardStart, transform as RectTransform, topLayer: topLayer);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(CoinLeaderBoardModel.Instance.IsStart());
        
        if (!gameObject.activeSelf)
            return;
        
        _timeText.SetText(CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek.GetLeftTimeText());
        if (CoinLeaderBoardModel.Instance.IsStartAndStorageInitFromServer())
        {
            _rankText.gameObject.SetActive(true);
            _rankText.SetText(CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek.SortController().MyRank.ToString());   
        }
        else
        {
            _rankText.gameObject.SetActive(false);
        }
        // var unCollectRewardsCount = 0;
        // _redPoint.gameObject.SetActive(unCollectRewardsCount > 0);
        // _redPointText.SetText(unCollectRewardsCount.ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (!CoinLeaderBoardModel.Instance.IsStart())
            return;
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CoinLeaderBoardStart, null);
        CoinLeaderBoardModel.OpenMainPopup(CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek);
    }
    public void UpdateTime()
    {
        CoinLeaderBoardModel.Instance.UpdateTime();
    }
    private void OnDestroy()
    {
    }
}
