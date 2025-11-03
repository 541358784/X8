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

public class MergeThemeDecoration : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _eggCountText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;

    // private LocalizeTextMeshProUGUI _rankText;
    // private Transform _rewardGroup;
    private ThemeDecorationShopRedPoint RedPoint;
    private bool _isAnim = false;
    
    private StorageThemeDecoration Storage;

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _eggCountText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _eggCountText.gameObject.SetActive(true);
        // _rankText = transform.Find("Root/RankText").GetComponent<LocalizeTextMeshProUGUI>();
        // _rankText.gameObject.SetActive(false);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<ThemeDecorationShopRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);

        Storage = ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek;
        RedPoint.Init(Storage);
        RefreshView();
    }

    // public void Update()
    // {
    //     RefreshView();
    // }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ThemeDecorationInfo);
        if (!Storage.IsResExist())
            return;
        if (!Storage.IsPreheat())
        {
            UIPopupThemeDecorationPreviewController.Open(Storage);
        }
        else if(!Storage.IsTotalTimeOut())
        {
            UIThemeDecorationShopController.Open(Storage);
        }
        else if (Storage.CanBuyEnd())
        {
            UIPopupThemeDecorationBuyPreEndController.Open(Storage);
        }
    }

    private void OnEnable()
    {
        _isAnim = false;
        StopAllCoroutines();
    }
    
    public void RefreshView()
    {
        if (Storage == null)
            return;
        gameObject.SetActive(Storage.ShowEntrance());
        if (!gameObject.activeSelf)
            return;
        
        if(!_isAnim)
            _eggCountText.SetText(Storage.Score.ToString());

        // var leaderBoard = Storage.LeaderBoardStorageList.Find(a => a.IsActive());
        // _rankText.gameObject.SetActive(leaderBoard != null && leaderBoard.IsInitFromServer());
        // if (_rankText.gameObject.activeSelf)
        // {
        //     _rankText.SetText("No."+leaderBoard.SortController().MyRank);
        // }
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (!Storage.IsPreheat())
        {
            _countDownTime.SetText(ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.GetPreheatLeftTimeText());
            RedPoint.gameObject.SetActive(false);
        }
        else if(!Storage.IsTotalTimeOut())
        {
            _countDownTime.SetText(ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.GetTotalLeftTimeText());
            RedPoint.UpdateUI();
        }
        else if (Storage.CanBuyEnd())
        {
            _countDownTime.SetText(ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.GetPreEndBuyLeftTimeText());
            RedPoint.gameObject.SetActive(false);
        }
    }
    public void SetText(int oldValue)
    {
        _isAnim = true;
        var newValue = UserData.Instance.GetRes(UserData.ResourceId.ThemeDecorationScore);
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