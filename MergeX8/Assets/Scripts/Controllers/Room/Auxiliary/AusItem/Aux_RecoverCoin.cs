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

public class Aux_RecoverCoin : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _rankText;
    private LocalizeTextMeshProUGUI _timeText;
    // private LocalizeTextMeshProUGUI _offText;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    private string SkinName;
    private string SkinPath => SkinName + "/";
    private Transform SkinNode;
    protected override void Awake()
    {
        base.Awake();
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        RefreshSkin();

        InvokeRepeating("UpdateUI", 0, 1);
        InvokeRepeating("UpdateTime", 0, 1);
        
        // List<Transform> topLayer = new List<Transform>();
        // topLayer.Add(transform);
        // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.RecoverCoinStart, transform as RectTransform, topLayer: topLayer);
    }

    public void RefreshSkin()
    {
        var curSkinName = RecoverCoinModel.GetSkinName();
        if (curSkinName == SkinName)//切皮肤
        {
            return;
        }
        if (SkinNode != null)
            SkinNode.gameObject.SetActive(false);
        SkinName = curSkinName;
        SkinNode = transform.Find(SkinName);
        SkinNode.gameObject.SetActive(true);
        _rankText  = transform.Find(SkinPath+"LvText").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText = transform.Find(SkinPath+"FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find(SkinPath+"RedPoint");
        _redPointText = transform.Find(SkinPath+"RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint.gameObject.SetActive(false);
    }

    public override void UpdateUI()
    {
        gameObject.SetActive(RecoverCoinModel.Instance.IsStart());
        
        if (!gameObject.activeSelf)
            return;
        RefreshSkin();
        _timeText.SetText(RecoverCoinModel.Instance.CurStorageRecoverCoinWeek.GetLeftTimeText());
        _rankText.SetText(RecoverCoinModel.Instance.CurStorageRecoverCoinWeek.SortController().MyRank.ToString());
        // var unCollectRewardsCount = 0;
        // _redPoint.gameObject.SetActive(unCollectRewardsCount > 0);
        // _redPointText.SetText(unCollectRewardsCount.ToString());
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (!RecoverCoinModel.Instance.IsStart())
            return;
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.RecoverCoinStart, null);
        RecoverCoinModel.OpenMainPopup(RecoverCoinModel.Instance.CurStorageRecoverCoinWeek);
    }
    public void UpdateTime()
    {
        RecoverCoinModel.Instance.UpdateTime();
    }
    private void OnDestroy()
    {
    }
}
