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

public class MergeFishCulture : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private FishCultureRedPoint RedPoint;
    private LocalizeTextMeshProUGUI NumText;
    
    private StorageFishCulture Storage;
    private LocalizeTextMeshProUGUI RankText;
    private void SetStorage(StorageFishCulture storage)
    {
        Storage = storage;
        RedPoint?.Init(Storage);
        RefreshView();
    }

    private void Awake()
    {
        NumText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _countDownTime = transform.Find("Root/TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint")?.gameObject.AddComponent<FishCultureRedPoint>();
        RedPoint?.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        RankText = transform.Find("Root/RankText").GetComponent<LocalizeTextMeshProUGUI>();
        SetStorage(FishCultureModel.Instance.CurStorageFishCultureWeek);
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.FishCultureEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupFishCulturePreviewController.Open(Storage);
        }
        else
        {
            if (Storage.IsStart)
            {
                UIFishCultureMainController.Open();
            }
            else
            {
                FishCultureModel.CanShowStartPopup();
            }
        }
    }

    public void RefreshView()
    {
        gameObject.SetActive(false);
        if (Storage == null)
            return;
        gameObject.SetActive(Storage.ShowTaskEntrance());
        if (!gameObject.activeSelf)
            return;
        RedPoint?.UpdateUI();
        if (!IsLockNum)
            NumText.SetText("x"+Storage.CurScore);
        var leaderBoard = FishCultureLeaderBoardModel.Instance.GetLeaderBoardStorage(Storage.ActivityId);
        RankText.gameObject.SetActive(leaderBoard.IsStorageWeekInitFromServer());
        if (RankText.gameObject.activeSelf)
        {
            RankText.SetText("No."+leaderBoard.SortController().MyRank);
        }
        
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (Storage.GetPreheatLeftTime() > 0)
        {
            _countDownTime.SetText(Storage.GetPreheatLeftTimeText());
        }
        else
        {
            _countDownTime.SetText(Storage.GetLeftPreEndTimeText());
        }
    }

    private bool IsLockNum = false;
    public void LockNum()
    {
        IsLockNum = true;
    }
    public void SetText(int num)
    {
        IsLockNum = false;
        NumText.SetText("x"+Storage.CurScore);
    }
    private void OnDestroy()
    {
    }
}