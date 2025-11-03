using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using TMatchShopConfigManager = TMatch.TMatchShopConfigManager;
using ActivityWeeklyChallengeModel = WeeklyChallengeModel;


namespace TMatch
{
    public class WeeklyChallengeGateView : UIView
    {
        protected override bool IsChildView => true;

        public enum StateType
        {
            Lock,
            UnOpen,
            Play,
            Finish,
        }

        [ComponentBinder("Lock")] private Transform lockTra;
        [ComponentBinder("UnOpen")] private Transform UnOpenTra;
        [ComponentBinder("Play")] private Transform playTra;
        [ComponentBinder("Finish")] private Transform finishTra;
        [ComponentBinder("Offline")] private Transform offlineTra;
        [ComponentBinder("GateButton")] private Button gateButton;

        private StateType stateType = StateType.Lock;
        public static StateType GateStateType = StateType.Lock;
        private LocalizeTextMeshProUGUI countDownText;

        private Color grayColor = new Color(0.5f, 0.5f, 0.5f);
        private Color lockTraTextColor;
        private Color unOpenTraColor;
        private Color finishTextColor;

        private WeeklyChallengeProgressView playProgressView;

        public override void OnViewOpen(UIViewParam param)
        {
            InvokeRepeating("UpdateActivity", 0, 1);
            
            bool isOpen = ActivityWeeklyChallengeModel.Instance.IsOpened();
            gameObject.SetActive(isOpen);
            if(!isOpen)
                return;
            
            base.OnViewOpen(param);
            StateReset(null);
            gateButton.onClick.AddListener(GeteOnClick);
            EventDispatcher.Instance.AddEventListener(EventEnum.LobbyMainShowState, Show);
            EventDispatcher.Instance.AddEventListener(EventEnum.WeeklyChallengeStateReset, StateReset);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecuteEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.WeeklyChellengeAddCollectCnt, OnWeeklyChellengeAddCollectCntEvt);
            
        }

        private void UpdateActivity()
        {
            bool isOpen = ActivityWeeklyChallengeModel.Instance.IsOpened();
            gameObject.SetActive(isOpen);
        }
        
        public override void OnViewDestroy()
        {
            GateStateType = StateType.Lock;
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LobbyMainShowState, Show);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.WeeklyChallengeStateReset, StateReset);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecuteEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.WeeklyChellengeAddCollectCnt, OnWeeklyChellengeAddCollectCntEvt);
            base.OnViewDestroy();
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            countDownText?.SetText(WeeklyChallengeController.Instance.model.GetCurWeekLeftTimeString());
        }

        private void StateReset(BaseEvent evt)
        {
            lockTraTextColor = lockTra.Find("Text").GetComponent<TextMeshProUGUI>().color;
            unOpenTraColor = UnOpenTra.Find("Text").GetComponent<TextMeshProUGUI>().color;
            finishTextColor = finishTra.Find("Text").GetComponent<TextMeshProUGUI>().color;

            lockTra.gameObject.SetActive(false);
            UnOpenTra.gameObject.SetActive(false);
            playTra.gameObject.SetActive(false);
            finishTra.gameObject.SetActive(false);
            countDownText = null;

            //Offline
            offlineTra.gameObject.SetActive(!WeeklyChallengeController.Instance.alreadySyncWithServer);
            transform.Find("BG").GetComponent<Image>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? Color.white : grayColor;
            GateStateType = stateType;

            //Lock
            if (!WeeklyChallengeController.Instance.model.IsUnlock(false))
            {
                stateType = StateType.Lock;
                lockTra.gameObject.SetActive(true);
                lockTra.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(
                    LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_weekchallenge_2", TMatchConfigManager.Instance.GlobalList[0].WeeklyChallengeUnlcok.ToString()));

                //颜色
                lockTra.Find("L").GetComponent<Image>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? Color.white : grayColor;
                lockTra.Find("R").GetComponent<Image>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? Color.white : grayColor;
                lockTra.Find("Text").GetComponent<TextMeshProUGUI>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? lockTraTextColor : lockTraTextColor * new Color(0.5f, 0.5f, 0.5f);
                return;
            }

            //UnOpen
            if (WeeklyChallengeController.Instance.model.stoage.CurWeekId == 0 || !WeeklyChallengeController.Instance.IsOpen())
            {
                stateType = StateType.UnOpen;
                UnOpenTra.gameObject.SetActive(true);

                //颜色
                UnOpenTra.Find("L").GetComponent<Image>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? Color.white : grayColor;
                UnOpenTra.Find("R").GetComponent<Image>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? Color.white : grayColor;
                UnOpenTra.Find("Text").GetComponent<TextMeshProUGUI>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? unOpenTraColor : unOpenTraColor * new Color(0.5f, 0.5f, 0.5f);
                return;
            }

            //Play
            if (!WeeklyChallengeController.Instance.model.IsClaimedAll())
            {
                stateType = StateType.Play;
                playTra.gameObject.SetActive(true);
                countDownText = playTra.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
                if (playProgressView == null) playProgressView = AddChildView<WeeklyChallengeProgressView>(playTra.Find("TaskSlider").gameObject);
                else playProgressView.Refresh();

                //颜色
                playProgressView.SetColor(!WeeklyChallengeController.Instance.alreadySyncWithServer);
                return;
            }

            //Finish
            stateType = StateType.Finish;
            finishTra.gameObject.SetActive(true);
            countDownText = finishTra.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();

            //颜色
            finishTra.Find("L").GetComponent<Image>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? Color.white : grayColor;
            finishTra.Find("R").GetComponent<Image>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? Color.white : grayColor;
            finishTra.Find("Text").GetComponent<TextMeshProUGUI>().color = WeeklyChallengeController.Instance.alreadySyncWithServer ? finishTextColor : finishTextColor * new Color(0.5f, 0.5f, 0.5f);

        }

        private void OnTMatchResultExecuteEvt(BaseEvent evt)
        {
            TMatchResultExecuteEvent realEvt = evt as TMatchResultExecuteEvent;
            if (realEvt.ExecuteType != TMatchResultExecuteType.WeeklyChallenge) return;
            if (!realEvt.LevelData.win)
            {
                LobbyTaskSystem.Instance.FinishCurrentTask();
                return;
            }

            if (stateType != StateType.Play || !WeeklyChallengeController.Instance.alreadySyncWithServer)
            {
                LobbyTaskSystem.Instance.FinishCurrentTask();
                return;
            }

            int cnt = CollectCnt(realEvt.LevelData.tripleItems);
            if (cnt == 0)
            {
                LobbyTaskSystem.Instance.FinishCurrentTask();
                return;
            }

            AddCollectCnt(cnt);
        }

        public static int CollectCnt(Dictionary<int, int> tripleItems)
        {
            if (!WeeklyChallengeController.Instance.model.IsUnlock() || WeeklyChallengeController.Instance.model.IsClaimedAll()) return 0;
            WeeklyChallenge weeklyChallengeCfg = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
            if (weeklyChallengeCfg == null) return 0;
            var cfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallengeCfg.collectItemId);
            int cnt = 0;
            foreach (var p in tripleItems)
            {
                if (p.Key == cfg.subId)
                {
                    cnt += p.Value;
                }
            }

            return cnt;
        }

        private void OnWeeklyChellengeAddCollectCntEvt(BaseEvent evt)
        {
            WeeklyChellengeAddCollectCntEvent derivedEvt = evt as WeeklyChellengeAddCollectCntEvent;
            AddCollectCnt(derivedEvt.cnt);
        }

        private void AddCollectCnt(int cnt)
        {
            WeeklyChallenge weeklyChallengeCfg = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
            List<WeeklyChallengeReward> rewards = WeeklyChallengeController.Instance.model.GetCurWeekRewards();
            int CurCollectItemNumOld = WeeklyChallengeController.Instance.model.GetCurLevelCollectNum();
            WeeklyChallengeReward reward = WeeklyChallengeController.Instance.model.GetCurLevelReward();
            WeeklyChallengeController.Instance.model.stoage.CurCollectItemNum += cnt;
            Action endAction = () =>
            {
                DOTween.To(() => CurCollectItemNumOld,
                    value => { playProgressView.planText.GetComponent<LocalizeTextMeshProUGUI>().SetText($"{value}/{reward.collectNum}"); },
                    WeeklyChallengeController.Instance.model.GetCurLevelCollectNum(), 0.2f);
                playProgressView.slider.DOValue(WeeklyChallengeController.Instance.model.GetCurLevelProgress(), 0.2f).OnComplete(() =>
                {
                    if (WeeklyChallengeController.Instance.model.GetCurLevelProgress() >= 1.0f)
                    {
                        WeeklyChallengeReward curLevelReward = WeeklyChallengeController.Instance.model.GetCurLevelReward();
                        playProgressView.rewardView.gameObject.SetActive(false);
                        WeeklyChallengeGetRewardViewParam viewParam = new WeeklyChallengeGetRewardViewParam();
                        //cur
                        {
                            WeeklyChallenge weeklyChallenge = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
                            WeeklyChallengeReward reward = WeeklyChallengeController.Instance.model.GetCurLevelReward();
                            var collectItemCfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallenge.collectItemId);
                            var rewardItemCfg = TMatchShopConfigManager.Instance.GetItem(reward.rewardId);
                            viewParam.curItem = new WeeklyChallengeItemViewParam();
                            viewParam.curItem.data = new ItemData();
                            viewParam.curItem.data.id = rewardItemCfg.GetItemType() == ItemType.TMWeeklyChallengeBuff ? collectItemCfg.id : rewardItemCfg.id;
                            viewParam.curItem.data.cnt = reward.rewardCnt;
                            if (rewardItemCfg.GetItemType() == ItemType.TMWeeklyChallengeBuff)
                            {
                                viewParam.curItem.buffData = new ItemData();
                                viewParam.curItem.buffData.id = rewardItemCfg.id;
                            }
                        }
                        //next
                        {
                            WeeklyChallenge weeklyChallenge = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
                            List<WeeklyChallengeReward> rewards = WeeklyChallengeController.Instance.model.GetCurWeekRewards();
                            if (WeeklyChallengeController.Instance.model.stoage.CurLevel < rewards.Count)
                            {
                                WeeklyChallengeReward reward = rewards[WeeklyChallengeController.Instance.model.stoage.CurLevel];
                                var collectItemCfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallenge.collectItemId);
                                var rewardItemCfg = TMatchShopConfigManager.Instance.GetItem(reward.rewardId);
                                viewParam.nextItem = new WeeklyChallengeItemViewParam();
                                viewParam.nextItem.data = new ItemData();
                                viewParam.nextItem.data.id = rewardItemCfg.GetItemType() == ItemType.TMWeeklyChallengeBuff ? collectItemCfg.id : rewardItemCfg.id;
                                viewParam.nextItem.data.cnt = reward.rewardCnt;
                                if (rewardItemCfg.GetItemType() == ItemType.TMWeeklyChallengeBuff)
                                {
                                    viewParam.nextItem.buffData = new ItemData();
                                    viewParam.nextItem.buffData.id = rewardItemCfg.id;
                                }
                            }
                        }
                        viewParam.src = playProgressView.rewardView.transform;
                        viewParam.claim = () =>
                        {
                            List<ItemData> rewards = new List<ItemData>();
                            ItemData rewardItem = new ItemData();
                            rewardItem.id = curLevelReward.rewardId;
                            rewardItem.cnt = curLevelReward.rewardCnt;
                            rewards.Add(rewardItem);
                            ItemModel.Instance.Add(rewardItem.id, rewardItem.cnt, new DragonPlus.GameBIManager.ItemChangeReasonArgs()
                            {
                                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ChallengeActiviteTm,
                                data1 = StorageManager.Instance.GetStorage<StorageTMatch>().WeeklyChallenge.CurLevel.ToString(),
                            },addType:3);
                            // CommonUtils.AddRewards(rewards, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                            // {
                            //     reason = BiEventMatchFrenzy.Types.ItemChangeReason.ChallengeActivite,
                            //     data1 = StorageManager.Instance.GetStorage<StorageTMatch>().WeeklyChallenge.CurLevel.ToString()
                            // });
                            var cfg = TMatchShopConfigManager.Instance.GetItem(curLevelReward.rewardId);
                            if (cfg.id != 0)
                            {
                                EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)cfg.id));
                            }
                        };
                        viewParam.finish = () =>
                        {
                            LobbyTaskSystem.Instance.FinishCurrentTask();
                            if (!WeeklyChallengeController.Instance.model.IsClaimedAll())
                            {
                                playProgressView.rewardView.gameObject.SetActive(true);
                                playProgressView.rewardView.Refresh(viewParam.nextItem);
                                StateReset(null);
                                if (WeeklyChallengeController.Instance.model.GetCurLevelProgress() > 0.0f)
                                {
                                    if (WeeklyChallengeController.Instance.model.GetCurLevelProgress() >= 1.0f)
                                    {
                                        AddCollectCnt(0);
                                    }
                                    else
                                    {
                                        playProgressView.slider.value = 0;
                                        playProgressView.planText.GetComponent<LocalizeTextMeshProUGUI>().SetText($"{0}/{reward.collectNum}");
                                        reward = WeeklyChallengeController.Instance.model.GetCurLevelReward();
                                        DOTween.To(() => 0,
                                            value => { playProgressView.planText.GetComponent<LocalizeTextMeshProUGUI>().SetText($"{value}/{reward.collectNum}"); },
                                            WeeklyChallengeController.Instance.model.GetCurLevelCollectNum(), 0.2f);
                                        playProgressView.slider.DOValue(WeeklyChallengeController.Instance.model.GetCurLevelProgress(), 0.2f);
                                    }
                                }
                            }
                            else
                            {
                                StateReset(null);
                            }
                        };
                        UIViewSystem.Instance.Open<WeeklyChallengeGetRewardView>(viewParam);

                        WeeklyChallengeController.Instance.model.stoage.CurLevel++;
                        WeeklyChallengeController.Instance.model.stoage.CurClaimCnt++;
                    }
                    else
                    {
                        LobbyTaskSystem.Instance.FinishCurrentTask();
                    }
                });
            };
            if (cnt > 0)
            {
                FlySystem.Instance.FlyItem(weeklyChallengeCfg.collectItemId, cnt,
                    UILobbyMainViewLevelButton.GetTopView().position, playProgressView.targetIcon.transform.position, endAction);
            }
            else
            {
                endAction.Invoke();
            }
        }

        private void GeteOnClick()
        {
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventChallengeChestClickTm);
            if (stateType == StateType.Lock)
            {
                int expectWeekId = TMatchConfigManager.Instance.GetSuitableWeekId((long)APIManager.Instance.GetServerTime());
                UIViewSystem.Instance.Open<WeeklyChallengeLockView>(new WeeklyChallengeViewParam() { expectWeekId = expectWeekId });
            }
            else if (stateType == StateType.Play)
            {
                UIViewSystem.Instance.Open<WeeklyChallengeView>();
            }
            else if (stateType == StateType.Finish)
            {
                int nextWeekId = TMatchConfigManager.Instance.GetSuitableWeekId((long)APIManager.Instance.GetServerTime() + 7 * 24 * 60 * 60 * 1000);
                UIViewSystem.Instance.Open<WeeklyChallengeLockView>(new WeeklyChallengeViewParam() { expectWeekId = nextWeekId, disableUnlockText = true });
            }
        }

        public void Show(BaseEvent evt)
        {
            LobbyMainShowStateEvent realEvt = evt as LobbyMainShowStateEvent;
            if (realEvt.enable)
            {
                DOTween.Kill(transform.GetComponent<RectTransform>());
                transform.GetComponent<RectTransform>().DOAnchorPosY(-171.0f, 0.3f);
            }
            else
            {
                DOTween.Kill(transform.GetComponent<RectTransform>());
                transform.GetComponent<RectTransform>().DOAnchorPosY(171.0f, 0.3f);
            }
        }
    }
}