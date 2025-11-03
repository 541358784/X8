using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using DogHopeLeaderBoard;
using UnityEngine;
using UnityEngine.UI;

public partial class UIDogMainController
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
        LeaderBoard.Init(DogHopeModel.Instance.CurStorageDogHopeWeek,this);
    }
    public class SingleRewardGroupNode : RewardGroupNode
    {
        public SingleRewardGroupNode(Transform transform) : base(transform)
        {
        }
        public override void BindPlayer(DogHopeLeaderBoardPlayer player)
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

        private StorageDogHope Storage;
        private UIDogMainController MainUI;
        
        public void Init(StorageDogHope storage,UIDogMainController mainUI)
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
            // TipText.SetText( LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_EasterEggActive_Shop_Ranking_Desc",DogHopeLeaderBoardModel.Instance.LeastEnterBoardScore.ToString()));
            TipText.SetTermFormats(DogHopeLeaderBoardModel.Instance.LeastEnterBoardScore.ToString());
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
                Text.SetText(Storage.LeaderBoardStorage.StarCount + "/" + DogHopeLeaderBoardModel.Instance.LeastEnterBoardScore);
                Label.gameObject.SetActive(false);
                Icon.gameObject.SetActive(true);
            }
            
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.DogHopeLeaderBoardMainEntrance) &&
                !GuideSubSystem.Instance.IsShowingGuide() &&
                DogHopeModel.Instance.CurStorageDogHopeWeek.LeaderBoardStorage.IsInitFromServer())
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(RankBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.DogHopeLeaderBoardMainEntrance, RankBtn.transform as RectTransform, topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.DogHopeLeaderBoardMainEntrance, ""))
                {
                    GuideSubSystem.Instance.ForceFinished(731);
                    GuideSubSystem.Instance.ForceFinished(732);
                }
            }
        }

        public void OnClickRankBtn()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.DogHopeLeaderBoardMainEntrance);
            if (!Storage.LeaderBoardStorage.IsInitFromServer())
            {
                Tip.DOKill();
                Tip.gameObject.SetActive(false);
                Tip.gameObject.SetActive(true);
                DOVirtual.DelayedCall(2f, () => Tip.gameObject.SetActive(false)).SetTarget(Tip);
                return;
            }
            // UnlockEffect.gameObject.SetActive(false);
            DogHopeLeaderBoardModel.OpenMainPopup(Storage.LeaderBoardStorage);
        }

        public void OnRankChange(DogHopeLeaderBoardPlayer player)
        {
            if (player.IsMe)
            {
                UpdateUI();
            }
        }
    }
}