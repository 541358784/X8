

using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class StarChallengeGateView : UIView
    {
        [ComponentBinder("")] private Button gateButton;
        [ComponentBinder("TimeText")] private LocalizeTextMeshProUGUI timeText;
        [ComponentBinder("ClaimButton")] private Button claimButton;

        [ComponentBinder("UIActivityAddItems")]
        private Animator addItems;

        [ComponentBinder("RankingText")] private LocalizeTextMeshProUGUI rankingText;
        [ComponentBinder("Stars")] private Transform stars;

        private bool isOpen = false;
        private bool isInCollecting = false;
        private int myRank = 0;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            gateButton.onClick.AddListener(OnGateButtonClicked);
            claimButton.onClick.AddListener(OnGateButtonClicked);
            Refresh();

            EventDispatcher.Instance.AddEventListener(EventEnum.StarChallengeRefresh, OnRefresh);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
            EventDispatcher.Instance.AddEventListener(EventEnum.LobbyRefreshShow, OnLobbyRefresh);
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            EventDispatcher.Instance.RemoveEventListener(EventEnum.StarChallengeRefresh, OnRefresh);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LobbyRefreshShow, OnLobbyRefresh);
        }

        private void OnRefresh(BaseEvent evt)
        {
            Refresh();
        }

        private void OnLobbyRefresh(BaseEvent evt)
        {
            Refresh();
        }

        private void Refresh()
        {
            // isOpen = StarChallengeActivityModel.Instance.IsActivityOpened();
            // gameObject.SetActive(isOpen);
            // if (isOpen)
            // {
            //     isInCollecting = StarChallengeActivityModel.Instance.IsInCollecting();
            //     claimButton.gameObject.SetActive(!isInCollecting);
            // }

            RefreshRankingText();
        }

        private void OnGateButtonClicked()
        {
            // var myStar = StarChallengeActivityModel.Instance.GetMyCollectedStars();
            // DragonPlus.GameBIManager.SendGameEvent(BiEventMatchFrenzy.Types.GameEventType.GameEventStarTournamentClick, 
            //     data1: myRank.ToString(), data2: myStar.ToString());
            // if (isInCollecting &&  myStar== 0)
            // {
            //     UIViewSystem.Instance.Open<StarChallengeLockPopup>();
            // }
            // else
            // {
            //     UIViewSystem.Instance.Open<StarChallengeMainPopup>();
            // }
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            if (!isOpen) return;
            // if (isInCollecting)
            // {
            //     var leftTime = StarChallengeActivityModel.Instance.GetCollectingLeftTime();
            //     if (leftTime > 0)
            //         timeText.SetText(CommonUtils.FormatPropItemTime((long)leftTime));
            //     else 
            //         Refresh();
            // }
            // else
            // {
            //     var rewardLeftTime = StarChallengeActivityModel.Instance.GetRewardShowLeftTime();
            //     if (rewardLeftTime == 0)
            //     {
            //         StarChallengeActivityModel.Instance.TryToResetStorage();
            //         Refresh();
            //     }
            // }
        }

        private async void OnTMatchResultExecute(BaseEvent evt)
        {
            TMatchResultExecuteEvent realEvt = evt as TMatchResultExecuteEvent;
            if (realEvt.ExecuteType != TMatchResultExecuteType.Last) return;
            if (!realEvt.LevelData.win) return;
            if (!isOpen || !isInCollecting) return;

            var cnt = StarCurrencyModel.Instance.GetStarCnt(realEvt.LevelData.layoutCfg.levelTimes, realEvt.LevelData.LastTimes);

            addItems.gameObject.SetActive(false);
            addItems.gameObject.SetActive(true);
            addItems.transform.Find("Root/NumberText").GetComponent<TextMeshProUGUI>().SetText($"+{cnt}");

            addItems.Play("appear01");
            addItems.Update(0);
            RefreshRankingText();
        }

        private void RefreshRankingText()
        {
            if (isOpen)
            {
                // myRank = StarChallengeActivityModel.Instance.GetMyRank();
                // rankingText.gameObject.SetActive(myRank > 0);
                // stars.gameObject.SetActive(myRank == 0);
                // rankingText.SetText("#" + myRank);
            }
        }
    }
}