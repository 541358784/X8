using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;


public class UIPopupLevelRankingStartController : UIWindowController
{
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _timeText;
    private Button _helpBtn;
    private Button _playBtn;
    private IEnumerator dailyIEnumerator = null;
    private Animator _animator;
    private List<Image> _rewardIcons = new List<Image>();
    
    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();
        
        _closeBtn = GetItem<Button>("Root/MainGroup/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnClose);
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/MainGroup/TimeGroup/TimeText");
        _helpBtn = GetItem<Button>("Root/MainGroup/HelpButton");
        _playBtn = GetItem<Button>("Root/MainGroup/PlayButton");
        _playBtn.onClick.AddListener(OnBtnPlay);
        
        for (int i= 1; i <= 5; i++)
        {
            _rewardIcons.Add(transform.Find("Root/BGGroup/reward_" + i + "/Image").gameObject.GetComponent<Image>());
        }
        
        InvokeRepeating("UpdateUI", 0 , 1);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDailyRankPop);
        
        int index = 0;
        foreach (var resData in DailyRankModel.Instance.GetRankReward(1, null))
        {
            _rewardIcons[index++].sprite = UserData.GetResourceIcon(resData.id, UserData.ResourceSubType.Big);
        }

        DailyRankModel.Instance._curDailyRank.IsShowStartView = true;
    }

    private void OnBtnPlay()
    {
        OnCloseClicked(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupLevelRankingShow);
        });
    }

    private void OnBtnClose()
    {
        OnBtnPlay();
    }

    public  void UpdateUI()
    {
        _timeText.SetText(DailyRankModel.Instance.GetActiveTime());
    }
    
    private void OnCloseClicked(Action action)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            action?.Invoke();
            CloseWindowWithinUIMgr(true);
        }));
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnCloseClicked(null);
    }
}