using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DragonU3DSDK.Asset;
using Game;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupLevelRankingMainController : UIWindowController
{
    private Button _buttonClose;
    private LocalizeTextMeshProUGUI _resetTimeText;
    private LocalizeTextMeshProUGUI _rankNum;
    private LocalizeTextMeshProUGUI _playText;
    
    private Animator _animator;
    private RectTransform _content;

    private GameObject _itemOther;
    private GameObject _itemSelf;

    private List<UIDailyRankItem> _dailyRankItems = new List<UIDailyRankItem>();

    private bool isWin = false;
    private int rank = 0;

    private StorageDailyRank dailyRank = null;
    
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/MainGroup/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseClicked);
        
        _buttonClose = GetItem<Button>("Root/MainGroup/PlayButton");
        _buttonClose.onClick.AddListener(OnCloseClicked);
        
        _animator = transform.GetComponent<Animator>();
        
        _resetTimeText = GetItem<LocalizeTextMeshProUGUI>("Root/MainGroup/TimeGroup/TimeText");
        _content = GetItem<RectTransform>("Root/MainGroup/Scroll View/Viewport/Content");

        _rankNum = GetItem<LocalizeTextMeshProUGUI>("Root/MainGroup/ShowGroup/RankGroup/Num/Text");
        _playText= GetItem<LocalizeTextMeshProUGUI>("Root/MainGroup/PlayButton/ButtonText");
        
        EventDispatcher.Instance.AddEventListener(EventEnum.DAILY_RANK_UPDATE, RefreshItems);
        
        _itemOther = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/LevelRanking/LevelRankingOtherItem");
        _itemSelf = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/LevelRanking/LevelRankingSelfItem");
        
        //InvokeRepeating("RepeatingTime", 0, 1);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.DAILY_RANK_UPDATE,RefreshItems);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);

        dailyRank = (StorageDailyRank)objs[0];
        
        RepeatingTime();
        InitView();

        for (int i = 0; i < _dailyRankItems.Count; i++)
        {
            isWin = _dailyRankItems[i]._isSelf;
            rank = i+1;
            if(isWin)
                break;
        }

        if (rank > 3)
            isWin = false;
        
        _playText.SetTerm(isWin ? "UI_button_claim_text" : "UI_button_continue");
        
        DailyRankModel.Instance.SendActivityBi(dailyRank);
        if (isWin)
        {
            var ret = DailyRankModel.Instance.GetRankReward(rank, dailyRank);
            foreach (var reward in ret)
            {
                if (!UserData.Instance.IsResource(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonDailyRank,
                        itemAId = reward.id,
                        ItemALevel = reward.count,
                        isChange = true,
                    });
                }
            }
            
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.DailyRank);
            reasonArgs.data1 = "rank";
            UserData.Instance.AddRes(ret, reasonArgs);
        }
    }

    private void InitView()
    {
        var itemSelf = Instantiate(_itemSelf, _content);
        _dailyRankItems.Add(itemSelf.AddComponent<UIDailyRankItem>());
        _dailyRankItems[0].InitData(1, LocalizationManager.Instance.GetLocalizedString("ui_common_me"), dailyRank.CurScore, true, canvas.sortingOrder, dailyRank);

        int index = 1;
        foreach (var robot in dailyRank.Robots)
        {
            var item = Instantiate(_itemOther, _content);
            _dailyRankItems.Add(item.AddComponent<UIDailyRankItem>());
            _dailyRankItems[index].InitData(++index, robot.RobotName, robot.CurScore, false, canvas.sortingOrder, dailyRank);
        }

        RefreshItems(null);
    }

    private void OnCloseClicked()
    {
        if (isWin)
        {
            var ret = DailyRankModel.Instance.GetRankReward(rank, dailyRank);
            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, false, animEndCall:() =>
            {
                CloseWindow(() =>
                {
                    UIPopupLevelRankingShowController.CanShowUI();
                });
            });

            isWin = false;
            return;
        }

        CloseWindow(() =>
        {
            UIPopupLevelRankingShowController.CanShowUI();
        });
    }

    private void CloseWindow(Action endCall)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            endCall?.Invoke();
            CloseWindowWithinUIMgr(true);
        }));
    }
    private void RefreshItems(BaseEvent baseEvent)
    {
        if(_dailyRankItems == null)
            return;
        
        _dailyRankItems.Sort((x, y) => { return y._scoreValue - x._scoreValue; });
        if (_dailyRankItems[1]._isSelf && _dailyRankItems[1]._scoreValue == _dailyRankItems[0]._scoreValue)
        {
            UIDailyRankItem temp = _dailyRankItems[1];
            _dailyRankItems[1] = _dailyRankItems[0];
            _dailyRankItems[0] = temp;
        }
        
        for (int i = 0; i < _dailyRankItems.Count; i++)
        {
            if (_dailyRankItems[i]._isSelf)
                _rankNum.SetText((i+1).ToString());
            
            _dailyRankItems[i].UpdateData(i + 1, _dailyRankItems[i]._scoreValue);
            _dailyRankItems[i].transform.SetSiblingIndex(i);
        }
    }

    private void RepeatingTime()
    {
        _resetTimeText.SetText("00:00");
    }
    
    public override void ClickUIMask()
    {
        OnCloseClicked();
    }
}