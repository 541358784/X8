using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Activity.BalloonRacing;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BalloonRacing.UI
{
    public class UISpeedRaceMainController : UIWindowController
    {
        private Button _buttonClose;
        private Button _buttonHelp;

        private GameObject _objHelp;

        private RectTransform _rectHelpBg;

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
            _objHelp = GetItem<Transform>("Root/HelpButton/Help").gameObject;
            _rectHelpBg = GetItem<RectTransform>("Root/HelpButton/Help/Image");

            _buttonClose.onClick.AddListener(delegate { AnimCloseWindow(); });
            _buttonHelp.onClick.AddListener(() => { UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingStart, UIWindowType.Normal, true); });

            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);

            var middleTrans = transform.Find("Root/MiddleGruop/0").GetComponent<RectTransform>();
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BalloonRacingSelf, middleTrans.transform, topLayer: middleTrans.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BalloonRacingEnd, this.transform);
        }


        void Update()
        {
            if (!_objHelp.activeSelf)
            {
                return;
            }

            // 检测点击任意位置关闭
            if (Input.GetMouseButtonDown(0))
            {
                //范围内
                if (UIRoot.Instance.IsPointInArea(Input.mousePosition, _rectHelpBg))
                    return;
                _objHelp.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);

            for (int i = 0; i < BalloonRacingModel.Instance.Storage.PlayerList.Count; i++)
            {
                StorageBalloonRacingPlayer player = BalloonRacingModel.Instance.Storage.PlayerList[i];
                player.LastShowScore = player.CurScore;
                player.LastShowRank = i;
            }
        }


        private void ScoreUpdate(BaseEvent e)
        {
            if (BalloonRacingModel.Instance.Storage.IsDone)
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
            for (int i = 0; i < BalloonRacingModel.Instance.Storage.PlayerList.Count; i++)
            {
                StorageBalloonRacingPlayer player = BalloonRacingModel.Instance.Storage.PlayerList[i];
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

            BalloonRacingModel.Instance.Storage.IsAward = true;
            int rankIndex = BalloonRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.IsMe == true);

            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRabbitRaceLeader,
                BalloonRacingModel.Instance.Storage.RunRounds.ToString(), (rankIndex + 1).ToString());

            bool isFaild = false;

            //输了
            if (rankIndex == -1 || rankIndex >= BalloonRacingModel.Instance.CurRacing.RewardRank)
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
                        listReward = CommonUtils.FormatReward(BalloonRacingModel.Instance.CurRacing.RewardType1,
                            BalloonRacingModel.Instance.CurRacing.RewardNumber1);
                        content = LocalizationManager.Instance.GetLocalizedString("ui_race_complete_desc1");
                        break;
                    case 1:
                        listReward = CommonUtils.FormatReward(BalloonRacingModel.Instance.CurRacing.RewardType2,
                            BalloonRacingModel.Instance.CurRacing.RewardNumber2);
                        content = LocalizationManager.Instance.GetLocalizedString("ui_race_complete_desc2");

                        break;
                    case 2:
                        listReward = CommonUtils.FormatReward(BalloonRacingModel.Instance.CurRacing.RewardType3,
                            BalloonRacingModel.Instance.CurRacing.RewardNumber3);
                        content = LocalizationManager.Instance.GetLocalizedString("ui_race_complete_desc3");

                        break;
                }

                await XUtility.WaitSeconds(0.5f);
                UIBalloonRacingOpenBoxController.Open(listReward, rankIndex + 1,
                    () =>
                    {
                        EventDispatcher.Instance.DispatchEvent(EventEnum.RABBIT_RACING_AWARD);
                        BalloonRacingModel.Instance.TryOpenMain();
                    });
                //UIBalloonRacingRewardController.Open(listReward, rankIndex + 1);
            }

            if (isFaild)
            {
                await XUtility.WaitSeconds(0.1f);
                EventDispatcher.Instance.DispatchEvent(EventEnum.RABBIT_RACING_AWARD);

                AnimCloseWindow(delegate { BalloonRacingModel.Instance.TryOpenMain(); });
            }
        }


        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            if (objs != null && objs.Length > 0)
            {
                _isCanClose = (bool)objs[0];
            }

            _textTargetScore.SetText(BalloonRacingModel.Instance.CurRacing.Collect.ToString());
            _textRounds.SetText(BalloonRacingModel.Instance.Storage.RunRounds.ToString());
            InvokeRepeating(nameof(UpdateTime), 0, 1);
            InitPlayer();
            if (BalloonRacingModel.Instance.Storage.IsDone && !BalloonRacingModel.Instance.Storage.IsAward)
            {
                DoEndAnim();
            }

            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BalloonRacingSelf, null);
        }

        private void InitPlayer()
        {
            GameObject obj = null;
            var topTransform = transform.Find("Root/MiddleGruop/Top").GetComponent<RectTransform>();
            var bottomTransform = transform.Find("Root/MiddleGruop/Bottom").GetComponent<RectTransform>();
            for (int i = 0; i < 5; i++)
            {
                obj = transform.Find("Root/MiddleGruop/" + i).gameObject;
                SpeedRacePlayerItem playerItem = obj.AddComponent<SpeedRacePlayerItem>();

                int rank = BalloonRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.Seat == i);
                playerItem.Init(rank, BalloonRacingModel.Instance.Storage.PlayerList[rank], topTransform, bottomTransform);
                _playerItems.Add(playerItem);
            }
        }

        private void UpdateTime()
        {
            _textTime.SetText(BalloonRacingModel.Instance.GetActivityLeftTimeString());
            if (BalloonRacingModel.Instance.GetActivityLeftTime() <= 0 && !_isClose)
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

            //GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRaceFail,  BalloonRacingModel.Instance.Storage.RunRounds.ToString());

            UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingFail, UIWindowType.Normal, endCall);

            await task.Task;
        }
    }
}
