using System;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using SnakeLadderLeaderBoard;
using UnityEngine;
using UnityEngine.UI;

public partial class UISnakeLadderMainController
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
            public override void BindPlayer(SnakeLadderLeaderBoardPlayer player)
            {
                var rewards = player.Rewards;
                for (var i = 0; i < _rewardNodes.Count; i++)
                {
                    _rewardNodes[i].UpdateState((i < rewards.Count && i<1 )? rewards[i] : null);
                }
            }
        }
        private Button RankBtn;
        private SingleRewardGroupNode RewardGroup;
        private LocalizeTextMeshProUGUI Text;
        private Transform Label;
        private Transform Icon;
        LocalizeTextMeshProUGUI TipText;
        private Transform Tip;
        // private Transform UnlockEffect;
        private void Awake()
        {
            RankBtn = transform.GetComponent<Button>();
            RankBtn.onClick.AddListener(OnClickRankBtn);
            RewardGroup = new SingleRewardGroupNode(transform.Find("RewardGroup"));
            Text = transform.Find("TextGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Tip = transform.Find("Tip");
            TipText = transform.Find("Tip/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Tip.gameObject.SetActive(false);
            Label = transform.Find("TextGroup/RankText");
            Icon = transform.Find("TextGroup/Image");
            // UnlockEffect = transform.Find("vfx");
            // UnlockEffect.gameObject.SetActive(false);
            
        }

        private StorageSnakeLadder Storage;
        private UISnakeLadderMainController MainUI;
        
        public void Init(StorageSnakeLadder storage,UISnakeLadderMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            lastState = Storage.LeaderBoardStorage.IsInitFromServer();
            UpdateUI();
            Storage.LeaderBoardStorage.SortController().BindRankChangeAction(OnRankChange);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderScoreChange>(OnScoreChange);
        }

        private void OnDestroy()
        {
            if (Storage != null)
            {
                Storage.LeaderBoardStorage.SortController().UnBindRankChangeAction(OnRankChange);
            }
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderScoreChange>(OnScoreChange);
        }
        private bool lastState = false;
        public void UpdateUI()
        {
            // TipText.SetText( LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_EasterEggActive_Shop_Ranking_Desc",SnakeLadderLeaderBoardModel.Instance.LeastEnterBoardScore.ToString()));
            TipText.SetTermFormats(SnakeLadderLeaderBoardModel.Instance.LeastEnterBoardScore.ToString());
            var newState = Storage.LeaderBoardStorage.IsInitFromServer();
            if (!lastState && newState)
            {
                // UnlockEffect.DOKill();
                // UnlockEffect.gameObject.SetActive(true);
            }
            lastState = newState;
            if (Storage.LeaderBoardStorage.IsInitFromServer())
            {
                RewardGroup.transform.gameObject.SetActive(true);
                RewardGroup.BindPlayer(Storage.LeaderBoardStorage.SortController().Me);
                Text.SetText(Storage.LeaderBoardStorage.SortController().MyRank.ToString());
                Label.gameObject.SetActive(true);
                Icon.gameObject.SetActive(false);
            }
            else
            {
                RewardGroup.transform.gameObject.SetActive(false);
                Text.SetText(Storage.LeaderBoardStorage.StarCount + "/" + SnakeLadderLeaderBoardModel.Instance.LeastEnterBoardScore);
                Label.gameObject.SetActive(false);
                Icon.gameObject.SetActive(true);
            }
        }

        public void OnClickRankBtn()
        {
            if (MainUI.IsPlaying())
                return;
            if (!Storage.LeaderBoardStorage.IsInitFromServer())
            {
                Tip.DOKill();
                Tip.gameObject.SetActive(false);
                Tip.gameObject.SetActive(true);
                DOVirtual.DelayedCall(2f, () => Tip.gameObject.SetActive(false)).SetTarget(Tip);
                return;
            }
            // UnlockEffect.gameObject.SetActive(false);
            SnakeLadderLeaderBoardModel.OpenMainPopup(Storage.LeaderBoardStorage);
        }

        public void OnRankChange(SnakeLadderLeaderBoardPlayer player)
        {
            if (player.IsMe)
            {
                Action<Action> performAction = (callback) =>
                {
                    UpdateUI();
                    callback();
                };
                MainUI.PushPerformAction(performAction);
            }
        }

        public void OnScoreChange(EventSnakeLadderScoreChange evt)
        {
            if (!Storage.LeaderBoardStorage.IsInitFromServer() && evt.ChangeValue > 0)
            {
                Action<Action> performAction = (callback) =>
                {
                    UpdateUI();
                    callback();
                };
                MainUI.PushPerformAction(performAction);
            }
        }
    }
}