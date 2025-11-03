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

public class MergeThemeDecorationLeaderBoard : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _eggCountText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;

    private LocalizeTextMeshProUGUI _rankText;
    
    private StorageThemeDecorationLeaderBoard Storage;
    private void SetStorage(StorageThemeDecorationLeaderBoard storage)
    {
        Storage = storage;
        RefreshView();
        Storage.SortController().BindRankChangeAction(OnRankChange);
        EventDispatcher.Instance.AddEvent<EventThemeDecorationLeaderBoardScoreChange>(OnScoreChange);
    }

    public void OnScoreChange(EventThemeDecorationLeaderBoardScoreChange evt)
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
            EventDispatcher.Instance.RemoveEvent<EventThemeDecorationLeaderBoardScoreChange>(OnScoreChange);
        }
    }

    public void OnRankChange(ThemeDecorationLeaderBoardPlayer player)
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

        SetStorage(ThemeDecorationLeaderBoardModel.Instance.CurStorageThemeDecorationLeaderBoardWeek);
        
    }

    // public void Update()
    // {
    //     RefreshView();
    // }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ThemeDecorationLeaderBoardInfo);
        if (!Storage.IsResExist())
            return;
        if (!Storage.IsTimeOut())
        {
            ThemeDecorationLeaderBoardModel.OpenMainPopup(Storage);
        }
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage != null && Storage.ShowEntrance());
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
        RefreshView();
        if (ThemeDecorationLeaderBoardModel.Instance.IsStart())
            _countDownTime.SetText(ThemeDecorationLeaderBoardModel.Instance.CurStorageThemeDecorationLeaderBoardWeek.GetLeftTimeText());
    }
}