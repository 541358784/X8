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

public class MergeEaster2024 : MonoBehaviour
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
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Easter2024Info);
        Easter2024Model.CanShowMainPopup();
    }

    private void OnEnable()
    {
        _isAnim = false;
        RefreshView();
        StopAllCoroutines();
    }

    public void RefreshView()
    {
        gameObject.SetActive(Easter2024Model.Instance.IsStart());
        if (!Easter2024Model.Instance.IsStart())
            return;
        
        if(!_isAnim)
            _eggCountText.SetText("x"+Easter2024Model.Instance.GetEgg());
        
        _rankText.gameObject.SetActive(Easter2024Model.Instance.CurStorageEaster2024Week.LeaderBoardStorage.IsInitFromServer());
        if (_rankText.gameObject.activeSelf)
        {
            _rankText.SetText("No."+Easter2024Model.Instance.CurStorageEaster2024Week.LeaderBoardStorage.SortController().MyRank);
        }
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (Easter2024Model.Instance.IsStart())
            _countDownTime.SetText(Easter2024Model.Instance.CurStorageEaster2024Week.GetLeftTimeText());
    }
    public void SetText(int oldValue)
    {
        _isAnim = true;
        var newValue = UserData.Instance.GetRes(UserData.ResourceId.Easter2024Egg);
        DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
        {
            _isAnim = false;
            _eggCountText.SetText("x"+oldValue.ToString());
        }).SetDelay(1f);

    }
}