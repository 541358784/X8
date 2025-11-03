using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using RecoverCoin;
using UnityEngine;
using UnityEngine.UI;

public class UIRecoverCoinMainController:UIWindowController
{
   
    // public enum CoinRushMainShowType
    // {
    //     Open,
    //     // Failed,
    // }
    private bool IsInBind => _storageWeek != null;
    private StorageRecoverCoinWeek _storageWeek;
    private Button _buttonClose;
    private Button _buttonPlay;
    private Button _buttonCollect;
    private Button _buttonShop;
    private Button _buttonHelp;
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _timeGroupText;
    private Dictionary<int,RankNode> _rankNodeDictionary;
    private Transform _playerDefaultItem;
    private RectTransform _content;
    // private ScrollRect _scrollRect;
    // private SkeletonGraphic _skeletonGraphic;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _buttonPlay = GetItem<Button>("Root/PlayButton");
        _buttonPlay.onClick.AddListener(OnPlayBtn);
        _buttonCollect = GetItem<Button>("Root/ReceiveButton");
        _buttonCollect.onClick.AddListener(OnCollectBtn);
        _buttonShop = GetItem<Button>("Root/BuyButton");
        _buttonShop.onClick.AddListener(OnShopBtn);
        _buttonHelp = GetItem<Button>("Root/HelpButton");
        _buttonHelp.onClick.AddListener(OnHelpBtn);
        
        _timeGroup = GetItem<Transform>("Root/TimeGroup");
        _timeGroupText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _playerDefaultItem = GetItem<Transform>("Root/Scroll View/Viewport/Content/Rank");
        _playerDefaultItem.gameObject.SetActive(false);
        
        _content = GetItem<RectTransform>("Root/Scroll View/Viewport/Content");

        _rankNodeDictionary = new Dictionary<int, RankNode>();
        for (var i = 1; i <= 3; i++)
        {
            var topRankNode = new TopRankNode(transform.Find("Root/TopRanking/" + i));
            _rankNodeDictionary.Add(i,topRankNode);
        }
        InvokeRepeating("UpdateTime", 0, 1);
    }

    protected override async void OnOpenWindow(params object[] objs)  
    {
        base.OnOpenWindow(objs);
        RefreshUI();
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.RecoverCoinStart, _buttonPlay.transform as RectTransform, topLayer: new List<Transform>()
        {
            _buttonPlay.transform
        });
        if (!GuideSubSystem.Instance.Trigger(GuideTriggerPosition.RecoverCoinDes, null))
        {
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.RecoverCoinBuy, _buttonShop.transform as RectTransform, topLayer: new List<Transform>()
            {
                _buttonShop.transform
            });
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.RecoverCoinBuy, null);
        }
    }
    
    public void BindStorageWeek(StorageRecoverCoinWeek storageWeek)
    {
        if (_storageWeek == storageWeek)
            return;
        var lasPlayerCount = 0;
        if (_storageWeek != null)
        {
            lasPlayerCount = _storageWeek.SortController().PlayerCount;
            _storageWeek.SortController().UnBindRankChangeAction(OnRankChange);
        }
        _storageWeek = storageWeek;
        if (lasPlayerCount > _storageWeek.SortController().PlayerCount)
        {
            for (var i = Math.Max(4,_storageWeek.SortController().PlayerCount+1); i <= lasPlayerCount; i++)
            {
                var removeRankNode = _rankNodeDictionary[i];
                _rankNodeDictionary.Remove(i);
                removeRankNode.Destroy();
            }  
        }
        else if (lasPlayerCount < _storageWeek.SortController().PlayerCount)
        {
            for (var i = Math.Max(4,lasPlayerCount+1); i <= _storageWeek.SortController().PlayerCount; i++)
            {
            
                var normalRankNodeObj = GameObject.Instantiate(_playerDefaultItem.gameObject, _playerDefaultItem.parent);
                normalRankNodeObj.name = "Rank" + i;
                normalRankNodeObj.SetActive(true);
                var normalRankNode = new NormalRankNode(normalRankNodeObj.transform);
                _rankNodeDictionary.Add(i,normalRankNode);
            }   
        }

        if (_storageWeek.SortController().PlayerCount < 3)
        {
            for (var i = 1; i <= _storageWeek.SortController().PlayerCount; i++)
            {
                _rankNodeDictionary[i].Show();
            }
            for (var i = _storageWeek.SortController().PlayerCount+1; i <= 3; i++)
            {
                _rankNodeDictionary[i].Hide();
            }
        }
        //初始化更新rank,后续都通过单个rank改变触发更新
        for (var i = 1; i <= _storageWeek.SortController().PlayerCount; i++)
        {
            _rankNodeDictionary[i].BindPlayer(_storageWeek.SortController().Players[i]);
        }
        _storageWeek.SortController().BindRankChangeAction(OnRankChange);
        
        RefreshUI();
    }

    public void OnRankChange(RecoverCoinPlayer player)
    {
        if (!_rankNodeDictionary.ContainsKey(player.Rank))
        {
            var normalRankNodeObj = GameObject.Instantiate(_playerDefaultItem.gameObject, _playerDefaultItem.parent);
            normalRankNodeObj.name = "Rank" + player.Rank;
            normalRankNodeObj.SetActive(true);
            var normalRankNode = new NormalRankNode(normalRankNodeObj.transform);
            _rankNodeDictionary.Add(player.Rank,normalRankNode);
        }
        _rankNodeDictionary[player.Rank].BindPlayer(player);
    }
    public void RefreshUI()
    {
        if (!IsInBind)
            return;
        _buttonPlay.gameObject.SetActive(!_storageWeek.IsTimeOut());
        _buttonClose.gameObject.SetActive(!_storageWeek.CanStorageRecoverCoinWeekGetReward());
        _buttonCollect.gameObject.SetActive(_storageWeek.CanStorageRecoverCoinWeekGetReward());
        _buttonShop.gameObject.SetActive(!_storageWeek.IsTimeOut());
        //更新ui排名逻辑
        _content.anchoredPosition = GetContentPosition(_storageWeek.SortController().MyRank);
    }
    private const int MinHeight = 0;
    public Vector2 GetContentPosition(int rank)
    {
        var y = Math.Max(MinHeight,115 * (rank-6));
        return new Vector2(0, y);
    }
    private void OnCloseBtn()
    {
        CloseWindowWithinUIMgr(false);
    }

    void OnPlayBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.RecoverCoinStart);
        CloseWindowWithinUIMgr(false,() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                return;
            
            if (RecoverCoinModel.Instance.IsStart() && 
                ((!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.RecoverCoinTask)) || 
                (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.RecoverCoinInfo))))
            {
                GuideSubSystem.Instance.InGuideChain = true;
                RecoverCoinModel.InGuideChain = true;   
            }
            
            SceneFsm.mInstance.TransitionGame();
        });
    }

    async void OnCollectBtn()
    {
        if (!IsInBind)
        {
            CloseWindowWithinUIMgr();
            return;
        }
        if (_storageWeek.CanStorageRecoverCoinWeekGetReward())
        {
            if (_storageWeek.IsRealPeopleLeaderBoard())
            {
                var success = await _storageWeek.QuitLeaderBoardFromServer();
                if (!success)
                    return;   
            }
            //没有BI暂时注释掉
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CoinoverGet};
            var sortController = _storageWeek.SortController();
            DragonPlus.GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverLeadertop,
                data1: sortController.MyRank.ToString());
            var unCollectRewards = _storageWeek.RewardConfig().GetRewardsByRank(sortController.MyRank);
            _storageWeek.CompletedStorageActivity();
            CommonRewardManager.Instance.PopCommonReward(unCollectRewards, CurrencyGroupManager.Instance.currencyController,
                true, reasonArgs, () =>
                {
                    for (int i = 0; i < unCollectRewards.Count; i++)
                    {
                        if (!UserData.Instance.IsResource(unCollectRewards[i].id))
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonCoinoverGet,
                                isChange = false,
                                itemAId = unCollectRewards[i].id
                            });
                        }

                        DragonPlus.GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverOffreward,
                            data1: unCollectRewards[i].id.ToString(),
                            data2: unCollectRewards[i].count.ToString());
                    }

                    CloseWindowWithinUIMgr();
                    if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                    {
                        SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);   
                    }
                    CoinLeaderBoardModel.CanCreateNewStorage();
                });
            // CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reasonArgs, () =>
            // {
            //     CloseWindowWithinUIMgr();
            //     if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            //     {
            //         SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);   
            //     }
            // }, () =>
            // {
            //     for (int i = 0; i < unCollectRewards.Count; i++)
            //     {
            //         if (!UserData.Instance.IsResource(unCollectRewards[i].id))
            //         {
            //             GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            //             {
            //                 MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonCoinoverGet,
            //                 isChange = false,
            //                 itemAId = unCollectRewards[i].id
            //             });
            //         }
            //
            //         DragonPlus.GameBIManager.Instance.SendGameEvent(
            //             BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverOffreward,
            //             data1: unCollectRewards[i].id.ToString(),
            //             data2: unCollectRewards[i].count.ToString());
            //     }
            //     _storageWeek.CompletedStorageActivity();
            // });
        }
        else
        {
            CloseWindowWithinUIMgr();
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);   
            }
        }
    }

    void OnShopBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.RecoverCoinBuy);
        if (!IsInBind)
            return;
        if (_storageWeek.IsTimeOut())
            return;
        RecoverCoinModel.OpenBuyStarPopup(_storageWeek);
    }
    void OnHelpBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.RecoverCoinBuy);
        if (!IsInBind)
            return;
        if (_storageWeek.IsTimeOut())
            return;
        var startPopup = UIManager.Instance.OpenUI(_storageWeek.GetAssetPathWithSkinName(UINameConst.UIRecoverCoinStart),true) as UIRecoverCoinStartController;
    }

    public void UpdateTime()
    {
        if (!IsInBind)
            return;
        if (_storageWeek.IsTimeOut())
        {
            RefreshUI();
            CancelInvoke("UpdateTime");
            _timeGroup.gameObject.SetActive(false);
            return;
        }
        _timeGroupText.SetText(_storageWeek.GetLeftTimeText());
    }


    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        CloseWindowWithinUIMgr();
    }

    private void OnDestroy()
    {
        if (_storageWeek != null)
        {
            _storageWeek.SortController().UnBindRankChangeAction(OnRankChange);
        }
    }
}