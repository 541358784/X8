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

public class MergeCommonResourceLeaderBoard : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _eggCountText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;

    private LocalizeTextMeshProUGUI _rankText;
    
    private StorageCommonLeaderBoard Storage;
    public void SetStorage(StorageCommonLeaderBoard storage)
    {
        Storage = storage;
        RefreshView();
        Storage.SortController().BindRankChangeAction(OnRankChange);
        EventDispatcher.Instance.AddEvent<EventCommonLeaderBoardScoreChange>(OnScoreChange);
    }

    public void OnScoreChange(EventCommonLeaderBoardScoreChange evt)
    {
        if (evt.Storage != Storage)
            return;
        RefreshView();
    }
    private void OnDestroy()
    {
        if (Storage != null)
        {
            Storage.SortController().UnBindRankChangeAction(OnRankChange);   
            EventDispatcher.Instance.RemoveEvent<EventCommonLeaderBoardScoreChange>(OnScoreChange);
            MergeTaskEntrance_CommonResourceLeaderBoard.CreatorDic.Remove(Storage);
        }
    }

    public void OnRankChange(CommonLeaderBoardPlayer player)
    {
        if (player.IsMe)
            RefreshView();
    }

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _eggCountText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _eggCountText.gameObject.SetActive(true);
        _rankText = transform.Find("Root/LvText").GetComponent<LocalizeTextMeshProUGUI>();
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
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CommonResourceLeaderBoardInfo);
        if (!Storage.IsResExist())
            return;
        if (!Storage.IsTimeOut())
        {
            Storage.GetModelInstance().OpenMainPopup(Storage);
        }
    }

    public void RefreshView()
    {
        if (Storage == null)
            return;
        gameObject.SetActive(Storage.IsActive());
        if (!gameObject.activeSelf)
            return;
        
        _eggCountText.SetText(Storage.StarCount.ToString());
        
        _rankText.gameObject.SetActive(Storage.IsStorageWeekInitFromServer());
        if (_rankText.gameObject.activeSelf)
        {
            _rankText.SetText("No."+Storage.SortController().MyRank);
        }
    }
    private void RefreshCountDown()
    {
        if (Storage.IsActive())
            _countDownTime.SetText(Storage.GetLeftTimeText());
    }
}