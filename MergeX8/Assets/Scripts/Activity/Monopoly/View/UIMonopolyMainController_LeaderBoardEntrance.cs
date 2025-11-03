using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using CommonLeaderBoard;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    private bool InitLeaderBoardEntranceFlag = false;
    private LeaderBoardEntrance LeaderBoard;
    public void InitLeaderBoardEntrance()
    {
        if (InitLeaderBoardEntranceFlag)
            return;
        InitLeaderBoardEntranceFlag = true;
        LeaderBoard = transform.Find("Root/ButtonRank").gameObject.AddComponent<LeaderBoardEntrance>();
        LeaderBoard.Init(Storage,this);
    }
    
    public class LeaderBoardEntrance : MonoBehaviour
    {
        public class SingleRewardGroupNode : RewardGroupNode
        {
            public SingleRewardGroupNode(Transform transform) : base(transform)
            {
            }
            public override void BindPlayer(CommonLeaderBoardPlayer player)
            {
                var rewards = player.Rewards;
                for (var i = 0; i < _rewardNodes.Count; i++)
                {
                    _rewardNodes[i].UpdateState((i < rewards.Count && i<1 )? rewards[i] : null);
                }
            }
        }
        private Button RankBtn;
        // private SingleRewardGroupNode RewardGroup;
        private LocalizeTextMeshProUGUI Text;
        // private Transform Label;
        // private Transform Icon;
        LocalizeTextMeshProUGUI TipText;
        private Transform Tip;
        // private Transform UnlockEffect;
        
        // private LocalizeTextMeshProUGUI _eggCountText;
        private void Awake()
        {
            RankBtn = transform.GetComponent<Button>();
            RankBtn.onClick.AddListener(OnClickRankBtn);
            // RewardGroup = new SingleRewardGroupNode(transform.Find("RewardGroup"));
            Text = transform.Find("Root/LvText").GetComponent<LocalizeTextMeshProUGUI>();
            Tip = transform.Find("Tip");
            TipText = transform.Find("Tip/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Tip.gameObject.SetActive(false);
            // Label = transform.Find("TextGroup/RankText");
            // Icon = transform.Find("TextGroup/Image");
            // UnlockEffect = transform.Find("vfx");
            // UnlockEffect.gameObject.SetActive(false);
            // _eggCountText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
            // _eggCountText.gameObject.SetActive(true);
            
        }

        private StorageMonopoly Storage;
        private StorageCommonLeaderBoard LeaderBoardStorage;
        private UIMonopolyMainController MainUI;
        
        public void Init(StorageMonopoly storage,UIMonopolyMainController mainUI)
        {
            Storage = storage;
            LeaderBoardStorage = MonopolyLeaderBoardModel.Instance.GetLeaderBoardStorage(Storage.ActivityId);
            if (LeaderBoardStorage == null)
            {
                Debug.LogError("未找到活动存档对应的排行榜存档");
                return;
            }
            MainUI = mainUI;
            // lastState = LeaderBoardStorage.IsInitFromServer();
            RefreshView();
            LeaderBoardStorage.SortController().BindRankChangeAction(OnRankChange);
            EventDispatcher.Instance.AddEvent<EventMonopolyScoreChange>(OnScoreChange);
        }

        private void OnDestroy()
        {
            if (LeaderBoardStorage != null)
            {
                LeaderBoardStorage.SortController().UnBindRankChangeAction(OnRankChange);
            }
            EventDispatcher.Instance.RemoveEvent<EventMonopolyScoreChange>(OnScoreChange);
        }
        // private bool lastState = false;
        // public void UpdateUI()
        // {
        //     // TipText.SetText( LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_EasterEggActive_Shop_Ranking_Desc",MonopolyLeaderBoardModel.Instance.LeastEnterBoardScore.ToString()));
        //     TipText.SetTermFormats(LeaderBoardStorage.LeastStarCount.ToString());
        //     var newState = LeaderBoardStorage.IsInitFromServer();
        //     if (!lastState && newState)
        //     {
        //         // UnlockEffect.DOKill();
        //         // UnlockEffect.gameObject.SetActive(true);
        //     }
        //     lastState = newState;
        //     if (LeaderBoardStorage.IsInitFromServer())
        //     {
        //         // RewardGroup.transform.gameObject.SetActive(true);
        //         // RewardGroup.BindPlayer(LeaderBoardStorage.SortController().Me);
        //         Text.SetText(LeaderBoardStorage.SortController().MyRank.ToString());
        //         Label.gameObject.SetActive(true);
        //         Icon.gameObject.SetActive(false);
        //     }
        //     else
        //     {
        //         // RewardGroup.transform.gameObject.SetActive(false);
        //         Text.SetText(LeaderBoardStorage.StarCount + "/" + LeaderBoardStorage.LeastStarCount);
        //         Label.gameObject.SetActive(false);
        //         Icon.gameObject.SetActive(true);
        //     }
        // }
        
        public void RefreshView()
        {
            if (LeaderBoardStorage == null)
                return;
            gameObject.SetActive(LeaderBoardStorage.IsActive());
            if (!gameObject.activeSelf)
                return;
        
            // _eggCountText.SetText(LeaderBoardStorage.StarCount.ToString());
        
            Text.gameObject.SetActive(true);
            if (LeaderBoardStorage.IsStorageWeekInitFromServer())
            {
                Text.SetText("No."+LeaderBoardStorage.SortController().MyRank);
            }
            else
            {
                Text.SetText("No.--");
            }
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyLeaderBoardEntrance) && LeaderBoardStorage.IsStorageWeekInitFromServer())
            {
                Action<Action> performAction = (callback) =>
                {
                    List<Transform> topLayer = new List<Transform>();
                    topLayer.Add(RankBtn.transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MonopolyLeaderBoardEntrance, RankBtn.transform as RectTransform,
                        topLayer: topLayer);
                    if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyLeaderBoardEntrance, null))
                    {
                        GuideTriggerPosition.MonopolyLeaderBoardEntrance.WaitGuideFinish()
                            .AddCallBack(callback).WrapErrors();
                    }
                    else
                    {
                        callback();   
                    }
                };
                MainUI.PushPerformAction(performAction);
            }
        }

        public void OnClickRankBtn()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyLeaderBoardEntrance);
            if (MainUI.IsPlaying())
                return;
            // if (!LeaderBoardStorage.IsInitFromServer())
            // {
            //     Tip.DOKill();
            //     Tip.gameObject.SetActive(false);
            //     Tip.gameObject.SetActive(true);
            //     DOVirtual.DelayedCall(2f, () => Tip.gameObject.SetActive(false)).SetTarget(Tip);
            //     return;
            // }
            // UnlockEffect.gameObject.SetActive(false);
            MonopolyLeaderBoardModel.Instance.OpenMainPopup(LeaderBoardStorage);
        }

        public void OnRankChange(CommonLeaderBoardPlayer player)
        {
            if (player.IsMe)
            {
                Action<Action> performAction = (callback) =>
                {
                    RefreshView();
                    callback();
                };
                MainUI.PushPerformAction(performAction);
            }
        }

        public void OnScoreChange(EventMonopolyScoreChange evt)
        {
            if (!LeaderBoardStorage.IsInitFromServer() && evt.ChangeValue > 0)
            {
                Action<Action> performAction = (callback) =>
                {
                    RefreshView();
                    callback();
                };
                MainUI.PushPerformAction(performAction);
            }
        }
    }
}