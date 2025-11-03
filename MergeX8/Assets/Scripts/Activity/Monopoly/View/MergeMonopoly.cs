using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeMonopoly : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _eggCountText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;

    private LocalizeTextMeshProUGUI _rankText;
    // private Transform _rewardGroup;
    private MonopolyDiceRedPoint RedPoint;
    private bool _isAnim = false;
    
    private StorageMonopoly Storage;
    private void SetStorage(StorageMonopoly storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
        RefreshView();
    }

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _eggCountText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _eggCountText.gameObject.SetActive(true);
        _rankText = transform.Find("Root/RankText").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText.gameObject.SetActive(false);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<MonopolyDiceRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        
        SetStorage(MonopolyModel.Instance.CurStorageMonopolyWeek);
    }

    // public void Update()
    // {
    //     RefreshView();
    // }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyInfo);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupMonopolyPreviewController.Open(Storage);
        }
        else
        {
            UIMonopolyMainController.Open(Storage);
        }
    }

    private void OnEnable()
    {
        _isAnim = false;
        StopAllCoroutines();
    }
    
    public void RefreshView()
    {
        gameObject.SetActive(Storage != null && Storage.ShowEntrance() && Storage.GetPreheatLeftTime()<=0);
        if (!gameObject.activeSelf)
            return;
        
        if(!_isAnim)
            _eggCountText.SetText(Storage.DiceCount.ToString());

        var leaderBoard = MonopolyLeaderBoardModel.Instance.GetLeaderBoardStorage(Storage.ActivityId);
        _rankText.gameObject.SetActive(leaderBoard != null && leaderBoard.IsInitFromServer());
        if (_rankText.gameObject.activeSelf)
        {
            _rankText.SetText("No."+leaderBoard.SortController().MyRank);
        }
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (Storage.GetPreheatLeftTime() > 0)
        {
            _countDownTime.SetText(Storage.GetPreheatLeftTimeText());
            RedPoint.gameObject.SetActive(false);
        }
        else
        {
            _countDownTime.SetText(Storage.GetLeftTimeText());
            RedPoint.UpdateUI();
        }
    }
    public void SetText(int oldValue)
    {
        _isAnim = true;
        var newValue = UserData.Instance.GetRes(UserData.ResourceId.MonopolyDice);
        DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
        {
            _isAnim = false;
            _eggCountText.SetText(oldValue.ToString());
        }).SetDelay(1f);

    }

    private void OnDestroy()
    {
    }
}