using System;
using System.Collections;
using System.Collections.Generic;
using Activity.BattlePass;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using System.Threading.Tasks;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace Activity.BattlePass_2
{
    public partial class UIBattlePassMainController : UIWindowController
    {
        private Button _btnClose;
        private Button _btnPurchase;
        private Button _btnPurchaseVip;
        private LocalizeTextMeshProUGUI _countDownText;

        private RectTransform _contentRect;

        private Slider _totalSlider;
        private Transform _totalSliderEff;

        private Slider _bottomSlider;
        private Transform _bottomSliderEff;
        private Animator _bottomSliderEffAni;
        private LocalizeTextMeshProUGUI _bottomSliderText;
        private LocalizeTextMeshProUGUI _bottomSliderLv;
        private Transform _bottomSliderLvBoard;
        private Transform _bottomSliderBox;
        private Animator animator = null;
        private LocalizeTextMeshProUGUI _upgradesText;
        private bool closeing = false;
        private bool isAppear = true;

        // private BattlePassRewardBox _rewardBox;

        private string[] animName =
        {
            "appear",
            "disappear",
        };

        private Image _rewardIcon;
        private float scrollDis = 226;
        private float boxHeight = 384;
        private float scaleY = 1f;
        private GameObject _vipIcon;
        private GameObject _svipIcon;
        private GameObject _vvipIcon;
        private GameObject _vipLevelUp;
        private GameObject _vvipLevelUp;


        private ScrollView _loopScrollView;
        private List<BattlePassItem> _battlePassItems = new List<BattlePassItem>();

        public override void PrivateAwake()
        {
            CommonUtils.NotchAdapte(transform.Find("Root"));
            scrollDis = GetItem<RectTransform>("Root/MiddleGroup/Scroll View/Viewport/Content/Reward").rect.height;
            scaleY = GetItem<RectTransform>("Root/MiddleGroup/Scroll View/Viewport/Content").localScale.y;
            boxHeight = GetItem<RectTransform>("Root/MiddleGroup/Scroll View/Viewport/Content/Box").rect.height;

            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, UpdateUI);
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_STORE_REFRESH, UpdateStore);
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_COLLECT_LOOP, UpdateUI);
            InitUI();
            UpdateUI();
            ContentSnap();
            UpdateSlider();
            InvokeRepeating("UpdateActivityTime", 0.01f, 1.0f);

            PlayAnim(animName[0], Anim_Appear);
            InitTaskView();
        }


        protected override void OnOpenWindow(params object[] objs)
        {
            int openSrc = 1;
            if (objs != null && objs.Length > 0)
                openSrc = (int)objs[0];


            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpMainPop, openSrc.ToString());

            OnOpenView();

            StartCoroutine(GuideLogic());
        }

        private IEnumerator GuideLogic()
        {
            yield return new WaitForEndOfFrame();

            Action guideTrigger = () =>
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Bp_Reward, null);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Bp_Task, null);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Bp_Pay, null);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Bp_PayButton, null);


                StartCoroutine(BuyNewUltimate());
            };

            var dlg = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupBattlePass2Refresh);
            if (dlg == null)
            {
                guideTrigger();
                yield break;
            }

            while (true)
            {
                if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupBattlePass2Refresh) == null)
                    break;

                yield return new WaitForEndOfFrame();
            }

            while (true)
            {
                if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIBattlePass2Reward) == null)
                    break;

                yield return new WaitForEndOfFrame();
            }

            guideTrigger();
        }

        private IEnumerator BuyNewUltimate()
        {
            yield return new WaitForSeconds(0.5f);

            if (!GuideSubSystem.Instance.IsShowingGuide())
                UIPopupBattlePassBuyNewUltimate.CanShow();
        }

        private void InitUI()
        {
            animator = GetItem<Animator>(gameObject);
            animator.enabled = false;

            _btnClose = GetItem<Button>("Root/TitleGroup/ButtonClose");
            _btnClose.onClick.AddListener(CloseUI);

            _btnPurchase = GetItem<Button>("Root/TitleGroup/ButtonGroup/ButtonVip");
            _btnPurchase.onClick.AddListener(OnPurchase);

            _btnPurchaseVip = GetItem<Button>("Root/TitleGroup/ButtonGroup/ButtonVVip");
            _btnPurchaseVip.onClick.AddListener(OnPurchaseVip);

            _upgradesText = GetItem<LocalizeTextMeshProUGUI>("Root/TitleGroup/Attribute/LvUp/UpgradeText");


            _vipIcon = GetItem("Root/TitleGroup/VipIcon/Vip");
            _svipIcon = GetItem("Root/TitleGroup/VipIcon/SVip");
            _vvipIcon = GetItem("Root/TitleGroup/VipIcon/VVip");
            _vipLevelUp = GetItem("Root/TitleGroup/Attribute/EnergyUp");
            _vvipLevelUp = GetItem("Root/TitleGroup/Attribute/LvUp");

            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_btnPurchase.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Bp_PayButton,
                _btnPurchase.transform as RectTransform, topLayer: topLayer);

            _countDownText = GetItem<LocalizeTextMeshProUGUI>("Root/TitleGroup/TimeGroup/TimeText");

            _contentRect = GetItem<RectTransform>("Root/MiddleGroup/Scroll View/Viewport/Content");

            _totalSlider = GetItem<Slider>("Root/MiddleGroup/Scroll View/Viewport/Content/Slider");
            _totalSliderEff = _totalSlider.transform.Find("Fill Area/Fill/VFX_Slider");
            _totalSliderEff.gameObject.SetActive(true);
            _bottomSlider = GetItem<Slider>("Root/BottomGroup/Slider");
            _bottomSliderEff = _bottomSlider.transform.Find("Fill Area/Fill/VFX_Slider");
            _bottomSliderEffAni = _bottomSliderEff.GetComponent<Animator>();
            _bottomSliderText = GetItem<LocalizeTextMeshProUGUI>("Root/BottomGroup/Slider/Text");
            _bottomSliderLv = GetItem<LocalizeTextMeshProUGUI>("Root/BottomGroup/Slider/Lv");
            _bottomSliderLvBoard = GetItem<Transform>("Root/BottomGroup/Slider/Image2");
            _bottomSliderBox = GetItem<Transform>("Root/BottomGroup/Slider/Box");

            _rewardIcon = transform.Find("Root/BottomGroup/Slider/Image1").GetComponent<Image>();

            // _rewardBox = transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content/Box").gameObject
            //     .AddComponent<BattlePassRewardBox>();
            // _rewardBox.transform.SetAsLastSibling();

            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Bp_Pay,
                _btnPurchase.transform as RectTransform, moveToTarget: null);

            _loopScrollView = GetItem<ScrollView>("Root/MiddleGroup/Scroll View");

            _loopScrollView.onItemRender.AddListener(OnItemRender);
            _loopScrollView.numItems = (uint)BattlePassModel.Instance.BattlePassRewardConfig.Count;
            // if (transform.Find("Root/TitleGroup/Vip").TryGetComponent<UIShiny>(out var uiShiny))
            //     uiShiny.enabled = false;
            // transform.Find("Root/TitleGroup/Vip/VFX_Shiny").gameObject.SetActive(false);
            InitLoopRewardBox();
        }

        private void OnItemRender(int index, Transform child)
        {
            BattlePassItem script = child.gameObject.GetComponent<BattlePassItem>();
            if (script == null)
                script = child.gameObject.AddComponent<BattlePassItem>();

            if (index == 1)
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Bp_Reward, script.transform as RectTransform, moveToTarget: null);

            TableBattlePassReward config = BattlePassModel.Instance.BattlePassRewardConfig[index];
            script.InitData(config, index);

            if (!_battlePassItems.Contains(script))
                _battlePassItems.Add(script);

            _battlePassItems.Sort((a, b) => a.Index - b.Index);
            _battlePassItems.ForEach(a => a.transform.SetAsLastSibling());
        }

        private void UpdateUI(BaseEvent baseEvent = null)
        {
            if (baseEvent != null)
            {
                if (baseEvent.datas != null && baseEvent.datas.Length >= 1)
                {
                    bool isPurchase = (bool)(baseEvent.datas[0]);
                    if (isPurchase)
                    {
                    }
                }
            }

            _svipIcon.gameObject.SetActive(false);
            _vipIcon.gameObject.SetActive(false);
            _vvipIcon.gameObject.SetActive(false);
            _vipLevelUp.gameObject.SetActive(false);
            _vvipLevelUp.gameObject.SetActive(false);
            _btnPurchase.transform.gameObject.SetActive(false);
            _btnPurchaseVip.transform.gameObject.SetActive(false);

            bool isOldUser = BattlePassModel.Instance.IsOldUser();
            if (isOldUser)
            {
                _upgradesText.SetText("x" +
                                      ((100f + BattlePassModel.Instance.BattlePassActiveConfig.goldScoreMultiple) /
                                       100f).ToString("0.0"));
                switch (BattlePassModel.Instance.storageBattlePass.BuyType)
                {
                    case (int)BuyType.Copper:
                    {
                        if (!BattlePassModel.Instance.IsPurchase())
                        {
                            _svipIcon.gameObject.SetActive(true);
                            _vipLevelUp.gameObject.SetActive(true);
                            _vvipLevelUp.gameObject.SetActive(true);
                        }
                        else
                        {
                            _vipIcon.gameObject.SetActive(true);
                            _vipLevelUp.gameObject.SetActive(true);
                        }

                        break;
                    }
                    case (int)BuyType.Golden:
                    {
                        _svipIcon.gameObject.SetActive(true);
                        _vipLevelUp.gameObject.SetActive(true);
                        _vvipLevelUp.gameObject.SetActive(true);
                        break;
                    }
                    case (int)BuyType.Ultimate:
                    {
                        _vvipIcon.gameObject.SetActive(true);
                        _vipLevelUp.gameObject.SetActive(true);
                        _vvipLevelUp.gameObject.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                _upgradesText.SetText("x" +
                                      ((100f + BattlePassModel.Instance.BattlePassActiveConfig.goldScoreMultiple) /
                                       100f).ToString("0.0"));
                switch (BattlePassModel.Instance.storageBattlePass.BuyType)
                {
                    case (int)BuyType.Copper:
                    {
                        if (!BattlePassModel.Instance.IsPurchase())
                        {
                            _svipIcon.gameObject.SetActive(true);
                            _vipLevelUp.gameObject.SetActive(true);
                        }
                        else
                        {
                            _svipIcon.gameObject.SetActive(true);
                            _vipLevelUp.gameObject.SetActive(true);
                        }

                        break;
                    }
                    case (int)BuyType.Golden:
                    {
                        _svipIcon.gameObject.SetActive(true);
                        _vipLevelUp.gameObject.SetActive(true);
                        break;
                    }
                    case (int)BuyType.Ultimate:
                    {
                        _vvipIcon.gameObject.SetActive(true);
                        _vipLevelUp.gameObject.SetActive(true);
                        _vvipLevelUp.gameObject.SetActive(true);
                        break;
                    }
                }
            }

            _btnPurchase.gameObject.SetActive(!BattlePassModel.Instance.IsPurchase());

            bool isShowUltimate = BattlePassModel.Instance.CanShowUltimatePurchase();
            _btnPurchaseVip.gameObject.SetActive(isShowUltimate);

            if (isShowUltimate || BattlePassModel.Instance.storageBattlePass.IsUltimatePurchase)
            {
                _svipIcon.gameObject.SetActive(false);
                _vipLevelUp.gameObject.SetActive(false);
                _vvipIcon.gameObject.SetActive(true);
                _vipLevelUp.gameObject.SetActive(true);
                _vvipLevelUp.gameObject.SetActive(true);

                _upgradesText.SetText("x" +
                                      ((100f + BattlePassModel.Instance.BattlePassShopConfig.ultimateScoreMultiple) /
                                       100f).ToString("0.0"));
            }

            ContentSnap();
            UpdateSlider();
        }

        private void CloseUI()
        {
            if (!isAppear)
                return;

            if (closeing)
                return;

            closeing = true;

            PlayAnim(animName[1], Anim_Disappear);
        }

        private void OnPurchase()
        {
            if (!isAppear)
                return;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpMainBuy);

            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Bp_PayButton);
            UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePass2BuyNew1);
        }

        private void OnPurchaseVip()
        {
            if (!isAppear)
                return;

            UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePass2BuyNew2);
        }

        void UpdateActivityTime()
        {
            _countDownText.SetText(BattlePassModel.Instance.GetActivityLeftTimeString());

            UpdateTaskActivityTime();
        }


        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, UpdateUI);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_STORE_REFRESH, UpdateStore);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_COLLECT_LOOP, UpdateUI);
            CancelInvoke("UpdateActivityTime");
        }

        private void PlayAnim(string name, Action endCall)
        {
            animator.enabled = true;
            StartCoroutine(CommonUtils.PlayAnimation(animator, name, "", endCall));
        }

        private void Anim_Appear()
        {
            isAppear = true;
        }

        private void Anim_Disappear()
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpMainClose);

            CloseWindowWithinUIMgr(true);
        }

        private void UpdateStore(BaseEvent obj)
        {
            int store = (int)obj.datas[0];
            UpdateStore(store);
        }

        private async Task UpdateStore(int store)
        {
            closeing = true;
            int count = store > 10 ? 10 : store;
            var srcPos = Vector3.zero;
            var position = _rewardIcon.transform.position;
            float delayTime = 0.1f;
            for (int i = 0; i < count; i++)
            {
                int index = i;
                FlyGameObjectManager.Instance.FlyObject(_rewardIcon.gameObject, srcPos, position, true, 0.3f,
                    delayTime * i, async () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                        ShakeManager.Instance.ShakeLight();
                        if (index == count - 1)
                        {
                            await UpdateStoreSlider(store);
                        }
                    });
            }
        }

        private async Task UpdateStoreSlider(int store)
        {
            closeing = true;
            await UpdateSlider(store);
            await ContentSnap(store);
            closeing = false;
        }

        private async Task UpdateSlider(int store = 0)
        {
            int frontScore = 0;
            TableBattlePassReward frontRewardConfig = BattlePassModel.Instance.GetFrontRewardConfig();
            if (frontRewardConfig != null)
                frontScore = frontRewardConfig.exchangeItemNum;

            TableBattlePassReward rewardConfig =
                BattlePassModel.Instance.GetRewardConfig(BattlePassModel.Instance.CurScoreRatio, out var isEnd);

            if (store > 0)
            {
                _bottomSliderEff.gameObject.SetActive(false);
                _bottomSliderEff.gameObject.SetActive(true);
                TableBattlePassReward currentConfig =
                    BattlePassModel.Instance.GetRewardConfig(BattlePassModel.Instance.CurScoreRatio - store,
                        out var isEnd1);
                if (!isEnd1 && !isEnd)
                {
                    if (rewardConfig == null)
                        rewardConfig =
                            BattlePassModel.Instance.BattlePassRewardConfig[
                                BattlePassModel.Instance.BattlePassRewardConfig.Count - 1];
                    if (currentConfig == null)
                        return;
                    int lvCount = rewardConfig.id - currentConfig.id;
                    if (lvCount > 0)
                    {
                        int frontScore2 = 0;
                        TableBattlePassReward frontRewardConfig2 =
                            BattlePassModel.Instance.GetFrontRewardConfig(
                                BattlePassModel.Instance.CurScoreRatio - store);
                        if (frontRewardConfig2 != null)
                            frontScore2 = frontRewardConfig2.exchangeItemNum;
                        int start = BattlePassModel.Instance.CurScoreRatio - store - frontScore2;
                        DOTween.To(() => start, x => start = x, currentConfig.exchangeItemNum - frontScore2, 1f)
                            .OnUpdate(() =>
                            {
                                _bottomSliderText.SetText(start + "/" +
                                                          (currentConfig.exchangeItemNum - frontScore2));
                            });
                        _bottomSlider?.DOValue(1, 1f);
                        await Task.Delay(1000);
                        if (_bottomSlider == null)
                            return;
                        _bottomSlider.value = 0;
                        _bottomSliderLv?.SetText(rewardConfig.id.ToString());
                        await Task.Delay(400);
                        if (_bottomSlider == null)
                            return;
                        float ratio = 1.0f * (BattlePassModel.Instance.CurScoreRatio - frontScore) /
                                      (rewardConfig.exchangeItemNum - frontScore);
                        _bottomSlider.DOValue(ratio, 1f).onComplete += () => { _bottomSliderEffAni.Play("disappear"); };

                        int startValue = 0;
                        DOTween.To(() => startValue, x => startValue = x,
                            BattlePassModel.Instance.CurScoreRatio - frontScore, 1f).OnUpdate(() =>
                        {
                            _bottomSliderText.SetText(
                                startValue + "/" + (rewardConfig.exchangeItemNum - frontScore));
                        });
                        await Task.Delay(1000);
                    }
                    else
                    {
                        float ratio = 1.0f * (BattlePassModel.Instance.CurScoreRatio - frontScore) /
                                      (rewardConfig.exchangeItemNum - frontScore);
                        _bottomSlider.DOValue(ratio, 1f).onComplete += () => { _bottomSliderEffAni.Play("disappear"); };
                        int startValue = BattlePassModel.Instance.CurScoreRatio - store - frontScore;
                        DOTween.To(() => startValue, x => startValue = x,
                            BattlePassModel.Instance.CurScoreRatio - frontScore, 1f).OnUpdate(() =>
                        {
                            _bottomSliderText.SetText(
                                startValue + "/" + (rewardConfig.exchangeItemNum - frontScore));
                        });
                        await Task.Delay(1000);
                    }
                }
                else if (!isEnd1 && isEnd)
                {
                    if (rewardConfig == null)
                        rewardConfig =
                            BattlePassModel.Instance.BattlePassRewardConfig[
                                BattlePassModel.Instance.BattlePassRewardConfig.Count - 1];
                    if (currentConfig == null)
                        return;
                    int frontScore2 = 0;
                    TableBattlePassReward frontRewardConfig2 =
                        BattlePassModel.Instance.GetFrontRewardConfig(
                            BattlePassModel.Instance.CurScoreRatio - store);
                    if (frontRewardConfig2 != null)
                        frontScore2 = frontRewardConfig2.exchangeItemNum;
                    int start = BattlePassModel.Instance.CurScoreRatio - store - frontScore2;
                    DOTween.To(() => start, x => start = x, currentConfig.exchangeItemNum - frontScore2, 1f)
                        .OnUpdate(() =>
                        {
                            _bottomSliderText.SetText(start + "/" +
                                                      (currentConfig.exchangeItemNum - frontScore2));
                        });
                    _bottomSlider?.DOValue(1, 1f);
                    await Task.Delay(1000);
                    if (_bottomSlider == null)
                        return;
                    if (BattlePassModel.Instance.storageBattlePass.LoopRewardIsOpened())
                    {
                        _bottomSlider.value = 0;
                        _bottomSliderBox.gameObject.SetActive(true);
                        _bottomSliderLv.gameObject.SetActive(false);
                        _bottomSliderLvBoard.gameObject.SetActive(false);
                        await Task.Delay(400);
                        if (_bottomSlider == null)
                            return;
                        float ratio = (1.0f * (BattlePassModel.Instance.CurScoreRatio - BattlePassModel.Instance.GetFrontLoopRewardScore())) /
                                      (BattlePassModel.Instance.GetNextLoopRewardScore() - BattlePassModel.Instance.GetFrontLoopRewardScore());
                        _bottomSlider.DOValue(ratio, 1f).onComplete += () => { _bottomSliderEffAni.Play("disappear"); };

                        int startValue = 0;
                        DOTween.To(() => startValue, x => startValue = x,
                            BattlePassModel.Instance.CurScoreRatio - BattlePassModel.Instance.GetFrontLoopRewardScore(), 1f).OnUpdate(() =>
                        {
                            _bottomSliderText.SetText(
                                startValue + "/" + (BattlePassModel.Instance.GetNextLoopRewardScore() - BattlePassModel.Instance.GetFrontLoopRewardScore()));
                        });
                        await Task.Delay(1000);
                    }
                    else
                    {
                        _bottomSliderText.SetTerm("UI_max");
                        _bottomSlider.value = 1;
                    }
                }
                else if (isEnd1 && isEnd)
                {
                    if (BattlePassModel.Instance.storageBattlePass.LoopRewardIsOpened())
                    {
                        float ratio = 1.0f * (BattlePassModel.Instance.CurScoreRatio - BattlePassModel.Instance.GetFrontLoopRewardScore()) /
                                      (BattlePassModel.Instance.GetNextLoopRewardScore() - BattlePassModel.Instance.GetFrontLoopRewardScore());
                        _bottomSlider.DOValue(ratio, 1f).onComplete += () => { _bottomSliderEffAni.Play("disappear"); };
                        int startValue = BattlePassModel.Instance.CurScoreRatio - store - BattlePassModel.Instance.GetFrontLoopRewardScore();
                        DOTween.To(() => startValue, x => startValue = x,
                            BattlePassModel.Instance.CurScoreRatio - BattlePassModel.Instance.GetFrontLoopRewardScore(), 1f).OnUpdate(() =>
                        {
                            _bottomSliderText.SetText(
                                startValue + "/" + (BattlePassModel.Instance.GetNextLoopRewardScore() - BattlePassModel.Instance.GetFrontLoopRewardScore()));
                        });
                        await Task.Delay(1000);
                    }
                    else
                    {
                        _bottomSliderText.SetTerm("UI_max");
                        _bottomSlider.value = 1;
                    }
                }
            }

            if (_bottomSlider == null)
                return;
            if (isEnd)
            {
                if (BattlePassModel.Instance.storageBattlePass.LoopRewardIsOpened())
                {
                    _bottomSliderBox.gameObject.SetActive(true);
                    _bottomSliderLv.gameObject.SetActive(false);
                    _bottomSliderLvBoard.gameObject.SetActive(false);
                    // _bottomSliderText.SetTerm("UI_max");
                    // _bottomSlider.value = 1;
                    _bottomSlider.gameObject.SetActive(true);

                    float ratio = 1.0f * (BattlePassModel.Instance.CurScoreRatio - BattlePassModel.Instance.GetFrontLoopRewardScore()) /
                                  (BattlePassModel.Instance.GetNextLoopRewardScore() - BattlePassModel.Instance.GetFrontLoopRewardScore());

                    _bottomSlider.value = ratio;
                    _bottomSliderText.SetText((BattlePassModel.Instance.CurScoreRatio - BattlePassModel.Instance.GetFrontLoopRewardScore()) + "/" +
                                              (BattlePassModel.Instance.GetNextLoopRewardScore() - BattlePassModel.Instance.GetFrontLoopRewardScore()));
                }
                else
                {
                    _bottomSliderBox.gameObject.SetActive(false);
                    _bottomSliderLv.gameObject.SetActive(true);
                    _bottomSliderLvBoard.gameObject.SetActive(true);
                    _bottomSlider.gameObject.SetActive(true);
                    _bottomSliderText.SetTerm("UI_max");
                    _bottomSlider.value = 1;
                }
            }
            else
            {
                _bottomSliderBox.gameObject.SetActive(false);
                _bottomSliderLv.gameObject.SetActive(true);
                _bottomSliderLvBoard.gameObject.SetActive(true);
                _bottomSlider.gameObject.SetActive(true);

                float ratio = 1.0f * (BattlePassModel.Instance.CurScoreRatio - frontScore) /
                              (rewardConfig.exchangeItemNum - frontScore);

                _bottomSlider.value = ratio;
                _bottomSliderText.SetText((BattlePassModel.Instance.CurScoreRatio - frontScore) + "/" +
                                          (rewardConfig.exchangeItemNum - frontScore));
            }

            _bottomSliderLv.SetText(BattlePassModel.Instance.GetNowRewardConfig().id.ToString());
        }

        private float minY = 0;

        private async Task ContentSnap(int store = 0)
        {
            float firstStep = 0.02f;
            _totalSlider.minValue = 1;
            _totalSlider.maxValue = BattlePassModel.Instance.BattlePassRewardConfig.Count;
            int curIndex = 0;
            TableBattlePassReward rewardConfig =
                BattlePassModel.Instance.GetRewardConfig(BattlePassModel.Instance.CurScoreRatio, out var isEnd);
            if (isEnd)
            {
                // moveDisCount = (BattlePassModel.Instance.BattlePassRewardConfig.Count - 3);
                curIndex = BattlePassModel.Instance.BattlePassRewardConfig.Count;
            }
            else
            {
                // if (rewardConfig.id <= 3)
                // {
                //     moveDisCount = 0;
                // }
                // else
                // {
                //     moveDisCount = (rewardConfig.id - 3) >= (BattlePassModel.Instance.BattlePassRewardConfig.Count - 3)
                //         ? (BattlePassModel.Instance.BattlePassRewardConfig.Count - 3)
                //         : (rewardConfig.id - 3);
                // }


                curIndex = rewardConfig.id - 1;
            }

            if (store == 0)
            {
                int index = Math.Max(0, curIndex - 2);
                if (CommonUtils.IsLE_16_10() && isEnd)
                    index = BattlePassModel.Instance.BattlePassRewardConfig.Count-1;
                if(index >= 2)
                    _loopScrollView.ScrollTo(index, 0);
                _totalSlider.value = curIndex;
            }
            else
            {
                TableBattlePassReward currentConfig =
                    BattlePassModel.Instance.GetRewardConfig(BattlePassModel.Instance.CurScoreRatio - store,
                        out var isEnd1);
                if (isEnd1)
                    return;
                if (rewardConfig == null)
                    rewardConfig =
                        BattlePassModel.Instance.BattlePassRewardConfig[
                            BattlePassModel.Instance.BattlePassRewardConfig.Count - 1];
                int lvCount = rewardConfig.id - currentConfig.id;
                float time = 0.5f * lvCount;

                int _previousValue = Mathf.FloorToInt(_totalSlider.value);
                _totalSlider.DOValue(curIndex, time).OnUpdate(() =>
                {
                    int currentValue = Mathf.FloorToInt(_totalSlider.value);
                    if (currentValue != _previousValue)
                    {
                        var item = _battlePassItems.Find(a => a.Index == (currentValue - 1));
                        if(item != null)
                            item.UpdateUI(BattlePassModel.Instance.CurScoreRatio, true);
                        
                        _previousValue = currentValue;
                    }
                });
                int index = Math.Max(0, curIndex - 2);
                if (CommonUtils.IsLE_16_10() && isEnd)
                    index = BattlePassModel.Instance.BattlePassRewardConfig.Count-1;
                _loopScrollView.ScrollTo(index, time + 0.5f);
            }
        }
    }
}