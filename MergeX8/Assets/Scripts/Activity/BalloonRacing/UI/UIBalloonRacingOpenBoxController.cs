using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BalloonRacing.UI
{
    public class UIBalloonRacingOpenBoxController : UIWindowController
    {
        //private SkeletonGraphic Spine;
        private Transform DefaultItem;
        private Button CloseButton;
        private RectTransform HeadIconRoot;
        private LocalizeTextMeshProUGUI _text;
        private Text _playerName;

        public override void PrivateAwake()
        {
            _text = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
            DefaultItem = transform.Find("Root/Reward/Item");
            HeadIconRoot = transform.Find("Root/Player/Head") as RectTransform;
            _playerName = GetItem<Text>("Root/Player/Text");

            DefaultItem.gameObject.SetActive(false);
            CloseButton = GetItem<Button>("Root/ButtonClose");
            CloseButton.onClick.AddListener(OnClickCloseButton);
            _animator = GetComponent<Animator>();
        }

        private List<ResData> Rewards;
        private List<RewardData> RewardData = new List<RewardData>();
        private Action Callback;
        private int _selfRank;
        private bool _isFristClick = true;

        private Animator _animator;

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            Rewards = objs[0] as List<ResData>;
            _selfRank = (int)objs[1];
            SetData();
            if (Rewards == null)
            {
                Debug.LogError("竞速宝箱，奖励为空");
                return;
            }

            var rank = (int)objs[1];
            if (objs.Length > 2)
            {
                Callback = objs[2] as Action;
            }

            RewardData.Clear();
            for (var i = 0; i < Rewards.Count; i++)
            {
                var reward = Rewards[i];
                var rewardItemObj = Instantiate(DefaultItem.gameObject, DefaultItem.parent);
                RewardData rdData = new RewardData();
                rdData.gameObject = rewardItemObj;
                rdData.image = GetItem<Image>("Icon", rewardItemObj);
                rdData.numText = GetItem<LocalizeTextMeshProUGUI>("Num", rewardItemObj);
                rdData.UpdateReward(reward);
                rewardItemObj.SetActive(true);
                RewardData.Add(rdData);
            }

            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.REWARD_POPUP);
            ClickEnable = false;
            _text.SetTermFormats(rank.ToString());
            var headIcon = HeadIconNode.BuildMyHeadIconNode(HeadIconRoot);

            _playerName.text = LocalizationManager.Instance.GetLocalizedString("UI_race_user");

            for (int i = 0; i < Rewards.Count; i++)
            {
                if (!UserData.Instance.IsResource(Rewards[i].id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonRaceGet,
                        isChange = false,
                        itemAId = Rewards[i].id
                    });
                }
            }

            //礼包开启瞬间音效
            UserData.Instance.AddRes(Rewards, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.RaceGet));
            XUtility.WaitSeconds(0.5f, () => ClickEnable = true);
        }

        private void SetData()
        {
            for (var i = 1; i <= 3; i++)
            {
                transform.Find("Root/BoxGroup/Position/" + i).gameObject.SetActive(i == _selfRank);
            }
        }


        private bool ClickEnable = true;

        public void OnClickCloseButton()
        {
            if (!ClickEnable)
                return;
            if (_isFristClick)
            {
                _animator.Play("OpenBox", 0, 0);
                AudioManager.Instance.PlaySound(40);
                XUtility.WaitSeconds(1f, () => ClickEnable = true);
                ClickEnable = false;
                _isFristClick = false;
                return;
            }
            else
            {
                ClickEnable = false;
            }

            _animator.Play("disappear", 0, 0);

            FlyGameObjectManager.Instance.FlyObject(RewardData, CurrencyGroupManager.Instance.currencyController, () =>
            {
                Callback?.Invoke();
                //EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                var main = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIBalloonRacingMain) as UISpeedRaceMainController;
                if (main != null)
                {
                    main.AnimCloseWindow();
                }

                CloseWindowWithinUIMgr(true);
                CommonRewardManager.Instance.PopupCacheReward();
                foreach (var resData in RewardData)
                {
                    GameObject.Destroy(resData.gameObject);
                }
            });
        }

        public static UIBalloonRacingOpenBoxController Open(List<ResData> Rewards, int rank,
            System.Action animEndCall = null)
        {
            var popup = UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingOpenBox, Rewards, rank, animEndCall) as UIBalloonRacingOpenBoxController;
            return popup;
        }
    }
}