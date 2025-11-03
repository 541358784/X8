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

public class MergeSnakeLadder : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _eggCountText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;

    private LocalizeTextMeshProUGUI _rankText;
    // private Transform _rewardGroup;
    private bool _isAnim = false;

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _eggCountText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _eggCountText.gameObject.SetActive(true);
        _rankText = transform.Find("Root/RankText").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText.gameObject.SetActive(false);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("RefreshCountDown", 0, 1f);
    }

    // public void Update()
    // {
    //     RefreshView();
    // }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SnakeLadderInfo);
        SnakeLadderModel.CanShowMainPopup();
    }

    private void OnEnable()
    {
        _isAnim = false;
        RefreshView();
        StopAllCoroutines();
    }

    private void RefreshView()
    {
        gameObject.SetActive(SnakeLadderModel.Instance.IsStart());
        if (!SnakeLadderModel.Instance.IsStart())
            return;
        
        if(!_isAnim)
            _eggCountText.SetText("x"+SnakeLadderModel.Instance.GetTurntableCount());
        
        _rankText.gameObject.SetActive(SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.LeaderBoardStorage.IsInitFromServer());
        if (_rankText.gameObject.activeSelf)
        {
            _rankText.SetText("No."+SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.LeaderBoardStorage.SortController().MyRank);
        }
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (SnakeLadderModel.Instance.IsStart())
            _countDownTime.SetText(SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.GetLeftTimeText());
    }
    public void SetText(int oldValue)
    {
        _isAnim = true;
        var newValue = UserData.Instance.GetRes(UserData.ResourceId.SnakeLadderTurntable);
        DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
        {
            _isAnim = false;
            _eggCountText.SetText("x"+oldValue.ToString());
        }).SetDelay(1f);

    }
}