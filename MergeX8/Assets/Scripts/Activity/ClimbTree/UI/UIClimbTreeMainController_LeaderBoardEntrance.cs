using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using ClimbTreeLeaderBoard;
using UnityEngine;
using UnityEngine.UI;

public partial class UIClimbTreeMainController
{
    private bool InitLeaderBoardEntranceFlag = false;
    private LeaderBoardEntrance LeaderBoard;
    public void InitLeaderBoardEntrance()
    {
        // return;
        if (InitLeaderBoardEntranceFlag)
            return;
        InitLeaderBoardEntranceFlag = true;
        LeaderBoard = transform.Find("Root/ButtonRank").gameObject.AddComponent<LeaderBoardEntrance>();
        LeaderBoard.gameObject.SetActive(true);
        LeaderBoard.Init(ClimbTreeModel.Instance.CurStorageClimbTreeWeek,this);
    }
    public class SingleRewardGroupNode : RewardGroupNode
    {
        public SingleRewardGroupNode(Transform transform) : base(transform)
        {
        }
        public override void BindPlayer(ClimbTreeLeaderBoardPlayer player)
        {
            var rewards = player.Rewards;
            for (var i = 0; i < _rewardNodes.Count; i++)
            {
                _rewardNodes[i].UpdateState((i < rewards.Count && i<1 )? rewards[i] : null);
            }
        }
    }
    public class LeaderBoardEntrance : MonoBehaviour
    {
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

        private StorageClimbTree Storage;
        private UIClimbTreeMainController MainUI;
        
        public void Init(StorageClimbTree storage,UIClimbTreeMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            lastState = Storage.LeaderBoardStorage.IsInitFromServer();
            UpdateUI();
            Storage.LeaderBoardStorage.SortController().BindRankChangeAction(OnRankChange);
        }

        private void OnDestroy()
        {
            if (Storage != null)
            {
                Storage.LeaderBoardStorage.SortController().UnBindRankChangeAction(OnRankChange);
            }
        }
        private bool lastState = false;
        public void UpdateUI()
        {
            // TipText.SetText( LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_EasterEggActive_Shop_Ranking_Desc",ClimbTreeLeaderBoardModel.Instance.LeastEnterBoardScore.ToString()));
            TipText.SetTermFormats(ClimbTreeLeaderBoardModel.Instance.LeastEnterBoardScore.ToString());
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
                Text.SetText(Storage.LeaderBoardStorage.StarCount + "/" + ClimbTreeLeaderBoardModel.Instance.LeastEnterBoardScore);
                Label.gameObject.SetActive(false);
                Icon.gameObject.SetActive(true);
            }
            
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ClimbTreeLeaderBoardMainEntrance) &&
                !GuideSubSystem.Instance.IsShowingGuide() &&
                ClimbTreeModel.Instance.CurStorageClimbTreeWeek.LeaderBoardStorage.IsInitFromServer())
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(RankBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClimbTreeLeaderBoardMainEntrance, RankBtn.transform as RectTransform, topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClimbTreeLeaderBoardMainEntrance, ""))
                {
                    GuideSubSystem.Instance.ForceFinished(734);
                    GuideSubSystem.Instance.ForceFinished(735);
                }
            }
        }

        public void OnClickRankBtn()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClimbTreeLeaderBoardMainEntrance);
            if (!Storage.LeaderBoardStorage.IsInitFromServer())
            {
                Tip.DOKill();
                Tip.gameObject.SetActive(false);
                Tip.gameObject.SetActive(true);
                DOVirtual.DelayedCall(2f, () => Tip.gameObject.SetActive(false)).SetTarget(Tip);
                return;
            }
            // UnlockEffect.gameObject.SetActive(false);
            ClimbTreeLeaderBoardModel.OpenMainPopup(Storage.LeaderBoardStorage);
        }

        public void OnRankChange(ClimbTreeLeaderBoardPlayer player)
        {
            if (player.IsMe)
            {
                UpdateUI();
            }
        }
    }
}