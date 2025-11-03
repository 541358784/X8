using System;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using Easter2024LeaderBoard;
using UnityEngine;
using UnityEngine.UI;

public partial class UIEaster2024MainController
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
    public class SingleRewardGroupNode : RewardGroupNode
    {
        public SingleRewardGroupNode(Transform transform) : base(transform)
        {
        }
        public override void BindPlayer(Easter2024LeaderBoardPlayer player)
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
        private Transform UnlockEffect;
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
            UnlockEffect = transform.Find("vfx");
            UnlockEffect.gameObject.SetActive(false);
        }

        private StorageEaster2024 Storage;
        private UIEaster2024MainController MainUI;
        
        public void Init(StorageEaster2024 storage,UIEaster2024MainController mainUI)
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
            // TipText.SetText( LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_EasterEggActive_Shop_Ranking_Desc",Easter2024LeaderBoardModel.Instance.LeastEnterBoardScore.ToString()));
            TipText.SetTermFormats(Easter2024LeaderBoardModel.Instance.LeastEnterBoardScore.ToString());
            var newState = Storage.LeaderBoardStorage.IsInitFromServer();
            if (!lastState && newState)
            {
                UnlockEffect.DOKill();
                UnlockEffect.gameObject.SetActive(true);
                // DOVirtual.DelayedCall(2f, () => UnlockEffect.gameObject.SetActive(false)).SetTarget(UnlockEffect);
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
                Text.SetText(Storage.LeaderBoardStorage.StarCount + "/" + Easter2024LeaderBoardModel.Instance.LeastEnterBoardScore);
                Label.gameObject.SetActive(false);
                Icon.gameObject.SetActive(true);
            }
        }

        public void OnClickRankBtn()
        {
            if (MainUI.Game.IsPlaying())
                return;
            if (!Storage.LeaderBoardStorage.IsInitFromServer())
            {
                Tip.DOKill();
                Tip.gameObject.SetActive(false);
                Tip.gameObject.SetActive(true);
                DOVirtual.DelayedCall(2f, () => Tip.gameObject.SetActive(false)).SetTarget(Tip);
                return;
            }
            UnlockEffect.gameObject.SetActive(false);
            Easter2024LeaderBoardModel.OpenMainPopup(Storage.LeaderBoardStorage);
        }

        public void OnRankChange(Easter2024LeaderBoardPlayer player)
        {
            if (player.IsMe)
            {
                UpdateUI();
            }
        }
    }
}