using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Activity.BalloonRacing.UI;
using Activity.RabbitRacing.Dynamic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.RabbitRacing.UI
{
    public class UISpeedRaceMainController : UIWindowController
    {
        private Button _buttonClose;
        private Button _buttonHelp;

        private LocalizeTextMeshProUGUI _textTargetScore;
        private LocalizeTextMeshProUGUI _textTime;
        private LocalizeTextMeshProUGUI _textRounds;

        private List<SpeedRacePlayerItem> _playerItems = new List<SpeedRacePlayerItem>();

        private bool _isClose = false;

        private bool _isCanClose = true;

        public override void PrivateAwake()
        {
            _textTime = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
            _textTargetScore = GetItem<LocalizeTextMeshProUGUI>("Root/Integral/Text");
            _textRounds = GetItem<LocalizeTextMeshProUGUI>("Root/RoundsTextCount");

            _buttonClose = GetItem<Button>("Root/CloseButton");
            _buttonHelp = GetItem<Button>("Root/HelpButton");

            _buttonClose.onClick.AddListener(delegate { AnimCloseWindow(); });
            _buttonHelp.onClick.AddListener(() =>
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupRabbitRacingStart, UIWindowType.Normal, true);
            });

            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);

            var middleTrans = transform.Find("Root/MiddleGruop/0").GetComponent<RectTransform>();
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.RabbitRacingSelf, middleTrans.transform, topLayer: middleTrans.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.RabbitRacingEnd, this.transform);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);

            for (int i = 0; i < RabbitRacingModel.Instance.Storage.PlayerList.Count; i++)
            {
                StorageBalloonRacingPlayer player = RabbitRacingModel.Instance.Storage.PlayerList[i];
                player.LastShowScore = player.CurScore;
                player.LastShowRank = i;
            }
        }


        private void ScoreUpdate(BaseEvent e)
        {
            if (RabbitRacingModel.Instance.Storage.IsDone)
            {
                DoEndAnim();
            }
            else
            {
                List<StorageBalloonRacingPlayer> list = (List<StorageBalloonRacingPlayer>)e.datas[0];

                for (int i = 0; i < list.Count; i++)
                {
                    SpeedRacePlayerItem item = _playerItems.Find(p => p.Seat == list[i].Seat);
                    if (item != null)
                    {
                        item.DoRefreshAnim(list[i]);
                    }
                }
            }
        }


        private async void DoEndAnim()
        {
            float maxTime = 0;
            float doneIndex = 0;
            for (int i = 0; i < RabbitRacingModel.Instance.Storage.PlayerList.Count; i++)
            {
                StorageBalloonRacingPlayer player = RabbitRacingModel.Instance.Storage.PlayerList[i];
                SpeedRacePlayerItem item = _playerItems.Find(p => p.Seat == player.Seat);
                if (player.IsDone)
                {
                    maxTime = 0.5f + doneIndex * 0.2f;
                    item.DoRefreshAnim(player);
                    doneIndex++;
                }
                else
                {
                    item.DoRefreshAnim(player);
                }
            }

            await XUtility.WaitSeconds(maxTime + 1f);

            RabbitRacingModel.Instance.Storage.IsAward = true;
            int rankIndex = RabbitRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.IsMe == true);

            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRabbitRaceLeader,
                RabbitRacingModel.Instance.Storage.RunRounds.ToString(), (rankIndex + 1).ToString());

            bool isFaild = false;

            //输了
            if (rankIndex == -1 || rankIndex >= RabbitRacingModel.Instance.CurRacing.RewardRank)
            {
                isFaild = true;
                await OpenFailAsync();
            }
            else
            {
                List<ResData> listReward = new List<ResData>();

                string content = "";
                switch (rankIndex)
                {
                    case 0:
                        listReward = CommonUtils.FormatReward(RabbitRacingModel.Instance.CurRacing.RewardType1,
                            RabbitRacingModel.Instance.CurRacing.RewardNumber1);
                        content = LocalizationManager.Instance.GetLocalizedString("ui_race_complete_desc1");
                        break;
                    case 1:
                        listReward = CommonUtils.FormatReward(RabbitRacingModel.Instance.CurRacing.RewardType2,
                            RabbitRacingModel.Instance.CurRacing.RewardNumber2);
                        content = LocalizationManager.Instance.GetLocalizedString("ui_race_complete_desc2");

                        break;
                    case 2:
                        listReward = CommonUtils.FormatReward(RabbitRacingModel.Instance.CurRacing.RewardType3,
                            RabbitRacingModel.Instance.CurRacing.RewardNumber3);
                        content = LocalizationManager.Instance.GetLocalizedString("ui_race_complete_desc3");

                        break;
                }

                await XUtility.WaitSeconds(0.5f);
                UIRabbitRacingOpenBoxController.Open(listReward, rankIndex + 1,
                    () =>
                    {
                        EventDispatcher.Instance.DispatchEvent(EventEnum.RABBIT_RACING_AWARD);
                        RabbitRacingModel.Instance.TryOpenMain();
                    });
                //UIBalloonRacingRewardController.Open(listReward, rankIndex + 1);
            }

            if (isFaild)
            {
                await XUtility.WaitSeconds(0.1f);
                EventDispatcher.Instance.DispatchEvent(EventEnum.RABBIT_RACING_AWARD);

                AnimCloseWindow(delegate { RabbitRacingModel.Instance.TryOpenMain(); });
            }
        }


        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            if (objs != null && objs.Length > 0)
            {
                _isCanClose = (bool)objs[0];
            }

            _textTargetScore.SetText(RabbitRacingModel.Instance.CurRacing.Collect.ToString());
            _textRounds.SetText(RabbitRacingModel.Instance.Storage.RunRounds.ToString());
            InvokeRepeating(nameof(UpdateTime), 0, 1);
            InitPlayer();
            if (RabbitRacingModel.Instance.Storage.IsDone && !RabbitRacingModel.Instance.Storage.IsAward)
            {
                DoEndAnim();
            }

            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.RabbitRacingSelf, null);
        }

        private void InitPlayer()
        {
            GameObject obj = null;
            // var topTransform = transform.Find("Root/MiddleGruop/Top").GetComponent<RectTransform>();
            // var bottomTransform = transform.Find("Root/MiddleGruop/Bottom").GetComponent<RectTransform>();
            for (int i = 0; i < 5; i++)
            {
                obj = transform.Find("Root/MiddleGruop/" + i).gameObject;
                SpeedRacePlayerItem playerItem = obj.AddComponent<SpeedRacePlayerItem>();

                int rank = RabbitRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.Seat == i);
                playerItem.Init(rank, RabbitRacingModel.Instance.Storage.PlayerList[rank]);
                _playerItems.Add(playerItem);
            }
        }

        private void UpdateTime()
        {
            _textTime.SetText(RabbitRacingModel.Instance.GetActivityLeftTimeString());
            if (RabbitRacingModel.Instance.GetActivityLeftTime() <= 0 && !_isClose)
            {
                _isClose = true;
                if (_isCanClose)
                {
                    AnimCloseWindow();
                }
                else
                {
                    _textTime.transform.parent.gameObject.SetActive(false);
                }
            }
        }


        private async Task OpenFailAsync()
        {
            var task = new TaskCompletionSource<bool>();
            Action endCall = delegate { task.SetResult(true); };

            //GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRaceFail,  RabbitRacingModel.Instance.Storage.RunRounds.ToString());

            UIManager.Instance.OpenUI(UINameConst.UIPopupRabbitRacingFail, UIWindowType.Normal, endCall);

            await task.Task;
        }
    }
}