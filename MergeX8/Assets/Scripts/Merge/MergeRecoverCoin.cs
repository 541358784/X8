using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine;
using Spine.Unity;
using SRF;
using UnityEngine;
using UnityEngine.UI;

public class MergeRecoverCoin : MonoBehaviour
{
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _starText;
    private LocalizeTextMeshProUGUI _rankText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butCoinRush;
    private Transform StarNode;
    
    private string SkinName;
    private string SkinPath => SkinName + "/";
    private Transform SkinNode;
    // private Transform _rewardGroup;
    private void Awake()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        RefreshSkin();
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
        
        _timeGroup = transform.Find(SkinPath+"Root/TimeGroup");
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);

        _starText = transform.Find(SkinPath+"Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText = transform.Find(SkinPath+"Root/LvText").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText.gameObject.SetActive(true);
        _countDownTime = transform.Find(SkinPath+"Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        transform.Find(SkinPath+"Root/RedPoint").gameObject.SetActive(false);
        StarNode = transform.Find(SkinPath+"Root/TimeGroup/Image");
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.RecoverCoinInfo);
        RecoverCoinModel.OpenMainPopup(RecoverCoinModel.Instance.CurStorageRecoverCoinWeek);
    }

    private void OnEnable()
    {
        RefreshView();
        StopAllCoroutines();
    }

    public void RefreshView()
    {
        gameObject.SetActive(RecoverCoinModel.Instance.IsStart());
        if (!gameObject.activeSelf)
            return;
        RefreshSkin();
        _starText.SetText(RecoverCoinModel.Instance.GetStar().ToString());
        _rankText.SetText(RecoverCoinModel.Instance.CurStorageRecoverCoinWeek.SortController().MyRank+"th");
        _timeGroup.gameObject.SetActive(RecoverCoinModel.Instance.IsStart());
        _countDownTime.SetText(RecoverCoinModel.Instance.CurStorageRecoverCoinWeek.GetLeftTimeText());
    }
    public Transform GetStarNode()
    {
        return StarNode;
    }
}