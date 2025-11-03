using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeCoinLeaderBoard : MonoBehaviour
{
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _starText;
    private LocalizeTextMeshProUGUI _rankText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butCoinRush;
    // private Transform _rewardGroup;


    private void Awake()
    {
        _timeGroup = transform.Find("Root/TimeGroup");
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);

        _starText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText = transform.Find("Root/LvText").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText.gameObject.SetActive(true);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        transform.Find("Root/RedPoint").gameObject.SetActive(false);

        InvokeRepeating("RefreshCountDown", 0, 1f);
    }

    public void Update()
    {
        RefreshView();
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CoinLeaderBoardInfo);
        CoinLeaderBoardModel.OpenMainPopup(CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek);
    }

    private void OnEnable()
    {
        RefreshView();
        StopAllCoroutines();
    }

    public void RefreshView()
    {
        if (!CoinLeaderBoardModel.Instance.IsStart())
            return;
        _starText.SetText(CoinLeaderBoardModel.Instance.GetStar().ToString());
        if (CoinLeaderBoardModel.Instance.IsStartAndStorageInitFromServer())
        {
            _rankText.gameObject.SetActive(true);
            _rankText.SetText(CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek.SortController().MyRank+"th");
        }
        else
        {
            _rankText.gameObject.SetActive(false);
        }
    }
    private void RefreshCountDown()
    {
        if (CoinLeaderBoardModel.Instance.IsStart())
            _countDownTime.SetText(CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek.GetLeftTimeText());
    }
}