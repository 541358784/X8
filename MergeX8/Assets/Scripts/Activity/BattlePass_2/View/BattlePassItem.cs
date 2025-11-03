using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using UnityEngine;
using UnityEngine.UI;
using Gameplay;

// using Unity.Services.Analytics;

namespace Activity.BattlePass_2
{
    public class ViewGroup
    {
        public Transform rewardItem;
        public GameObject completeGroup;
        public GameObject lockGroup;
        public Button claimBut;
        public Button button;
        public Animator lockAnim;
        public Transform unlockTips;
        public Transform receiveTips;
        public Transform noBuyTIps;
        public Transform item3;
    }

    public class BattlePassItem : MonoBehaviour
    {
        private ViewGroup normalGroup = new ViewGroup();
        private ViewGroup goldGroup = new ViewGroup();
        public LocalizeTextMeshProUGUI levelText;
        public LocalizeTextMeshProUGUI levelFinishText;

        private TableBattlePassReward rewardConfig = null;

        private int index;
        private bool isUpdata = false;

        private GameObject unLockGroup;
        private Button unLockButton;

        private bool isUnLock;
        private bool isPurchase;
        private bool isPrucheGeting;
        private bool isNormailGeting;

        private bool _isInit = false;

        private List<Transform> _normalItems = new List<Transform>();
        private List<Transform> _goldItems = new List<Transform>();
        public GameObject _finishEffect;

        public int Index => index;

        private Transform _currentTips;
        private UIShiny _shiny;

        private void Awake()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, UnLockEvent);
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_PURCHASE_UNLOCK, PurchaseUnLockEvent);
        }

        private void Start()
        {
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, UnLockEvent);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_PURCHASE_UNLOCK, PurchaseUnLockEvent);
        }

        public void InitData(TableBattlePassReward config, int index)
        {
            rewardConfig = config;
            this.index = index;

            InitUI();
            UpdateUIView();
            UpdateUI(BattlePassModel.Instance.CurScoreRatio, false);
        }

        private void UpdateUIView()
        {
            if (rewardConfig == null)
                return;

            levelText.SetText(rewardConfig.id.ToString());
            levelFinishText.SetText(rewardConfig.id.ToString());

            foreach (Transform child in normalGroup.item3)
            {
                child.gameObject.SetActive(false);
            }

            _normalItems.ForEach(a => a.gameObject.SetActive(false));

            if (rewardConfig.normalRewardId.Length > 2)
            {
                normalGroup.item3.gameObject.SetActive(true);

                for (int i = 0; i < rewardConfig.normalRewardId.Length; i++)
                {
                    var item = normalGroup.item3.Find("Icon" + (i + 1));
                    InitRewardItem(item, rewardConfig.normalRewardId[i], rewardConfig.normalRewardNum[i]);
                }
            }
            else
            {
                for (int i = 0; i < rewardConfig.normalRewardId.Length; i++)
                {
                    Transform item = null;
                    if (i + 1 <= _normalItems.Count)
                    {
                        item = _normalItems[i];
                    }
                    else
                    {
                        item = Instantiate(normalGroup.rewardItem, normalGroup.rewardItem.parent);
                        _normalItems.Add(item);
                    }

                    item.gameObject.SetActive(true);
                    InitRewardItem(item, rewardConfig.normalRewardId[i], rewardConfig.normalRewardNum[i]);
                }
            }


            foreach (Transform child in goldGroup.item3)
            {
                child.gameObject.SetActive(false);
            }

            _goldItems.ForEach(a => a.gameObject.SetActive(false));

            if (rewardConfig.keyRewardId.Length > 2)
            {
                goldGroup.item3.gameObject.SetActive(true);
                for (int i = 0; i < rewardConfig.keyRewardId.Length; i++)
                {
                    var item = goldGroup.item3.Find("Icon" + (i + 1));
                    InitRewardItem(item, rewardConfig.keyRewardId[i], rewardConfig.keyRewardNum[i]);
                }
            }
            else
            {
                for (int i = 0; i < rewardConfig.keyRewardId.Length; i++)
                {
                    Transform item = null;
                    if (i + 1 <= _goldItems.Count)
                    {
                        item = _goldItems[i];
                    }
                    else
                    {
                        item = Instantiate(goldGroup.rewardItem, goldGroup.rewardItem.parent);
                        _goldItems.Add(item);
                    }

                    item.gameObject.SetActive(true);
                    InitRewardItem(item, rewardConfig.keyRewardId[i], rewardConfig.keyRewardNum[i]);
                }
            }
        }

        private void InitUI()
        {
            if (rewardConfig == null)
                return;

            if (_isInit)
                return;

            _isInit = true;

            normalGroup.rewardItem = transform.Find("Free/IconGroup/Icon");
            normalGroup.item3 = transform.Find("Free/Item3");
            normalGroup.rewardItem.gameObject.SetActive(false);
            normalGroup.completeGroup = transform.Find("Free/Finish").gameObject;
            normalGroup.unlockTips = transform.Find("Free/UnlockedTips");
            normalGroup.unlockTips.gameObject.SetActive(false);
            normalGroup.receiveTips = transform.Find("Free/ReceiveTips");
            normalGroup.receiveTips.gameObject.SetActive(false);
            normalGroup.claimBut = transform.Find("Free/Button").GetComponent<Button>();
            normalGroup.claimBut.onClick.AddListener(OnButton_NormalGetting);
            normalGroup.button = transform.Find("Free").GetComponent<Button>();
            normalGroup.button.onClick.AddListener(OnButton_NormalClick);

            goldGroup.rewardItem = transform.Find("Vip/IconGroup/Icon");
            goldGroup.item3 = transform.Find("Vip/Item3");
            goldGroup.rewardItem.gameObject.SetActive(false);
            goldGroup.completeGroup = transform.Find("Vip/Finish").gameObject;
            goldGroup.lockGroup = transform.Find("Vip/Lock").gameObject;
            goldGroup.unlockTips = transform.Find("Vip/UnlockedTips");
            goldGroup.unlockTips.gameObject.SetActive(false);
            goldGroup.receiveTips = transform.Find("Vip/ReceiveTips");
            goldGroup.receiveTips.gameObject.SetActive(false);
            goldGroup.noBuyTIps = transform.Find("Vip/NotBuyTips");
            goldGroup.noBuyTIps.gameObject.SetActive(false);
            goldGroup.button = transform.Find("Vip").GetComponent<Button>();
            goldGroup.button.onClick.AddListener(OnButton_VipClick);
            // goldGroup.lockAnim = goldGroup.lockGroup.gameObject.GetComponent<Animator>();
            // goldGroup.lockAnim.enabled = false;
            goldGroup.claimBut = transform.Find("Vip/Button").GetComponent<Button>();
            goldGroup.claimBut.onClick.AddListener(OnButton_PurchaseGetting);
            //_shiny = transform.Find("Vip/BG/vfx_Shiny").GetComponent<UIShiny>();

            levelText = transform.Find("LV/Text").GetComponent<LocalizeTextMeshProUGUI>();
            levelFinishText = transform.Find("LV_Finish/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _finishEffect = transform.Find("LV_Finish/VFX_hint_1").gameObject;
            _finishEffect.gameObject.SetActive(false);
        }

        private void OnButton_VipClick()
        {
            ShowTips(true);
        }

        private void OnButton_NormalClick()
        {
            ShowTips(false);
        }

        private void ShowTips(bool isVip)
        {
            if (_currentTips != null)
                return;
            if (isVip)
            {
                if (!isPurchase)
                {
                    goldGroup.noBuyTIps.gameObject.SetActive(true);
                    _currentTips = goldGroup.noBuyTIps;
                }
                else
                {
                    if (isPrucheGeting)
                    {
                        goldGroup.receiveTips.gameObject.SetActive(true);
                        _currentTips = goldGroup.receiveTips;
                    }
                    else
                    {
                        if (!isUnLock)
                        {
                            goldGroup.unlockTips.gameObject.SetActive(true);
                            _currentTips = goldGroup.unlockTips;
                        }
                    }
                }
            }
            else
            {
                if (isNormailGeting)
                {
                    normalGroup.receiveTips.gameObject.SetActive(true);
                    _currentTips = normalGroup.receiveTips;
                }
                else
                {
                    if (!isUnLock)
                    {
                        normalGroup.unlockTips.gameObject.SetActive(true);
                        _currentTips = normalGroup.unlockTips;
                    }
                }
            }

            if (_currentTips != null)
            {
                StartCoroutine(CommonUtils.DelayWork(0.8f, () =>
                {
                    _currentTips.gameObject.SetActive(false);
                    _currentTips = null;
                }));
            }
        }

        public void UpdateUI(int score, bool isAnim)
        {
            if (rewardConfig == null)
                return;

            isUnLock = IsUnLock(score);
            isPurchase = IsPurchase();
            isPrucheGeting = IsPrucheGeting();
            isNormailGeting = IsNormalGetting();

            if (!isPurchase)
            {
                goldGroup.lockGroup.SetActive(true);
                goldGroup.claimBut.gameObject.SetActive(false);
                goldGroup.completeGroup.gameObject.SetActive(false);
            }
            else
            {
                goldGroup.lockGroup.SetActive(false);
                goldGroup.completeGroup.SetActive(isPrucheGeting);
                goldGroup.claimBut.gameObject.SetActive(!isPrucheGeting && isUnLock);
                //_shiny.enabled = !isPrucheGeting;
            }

            normalGroup.completeGroup.SetActive(isNormailGeting);
            normalGroup.claimBut.gameObject.SetActive(!isNormailGeting && isUnLock);
            levelFinishText.transform.parent.gameObject.SetActive(isUnLock);
            _finishEffect.gameObject.SetActive(isAnim && isUnLock);
        }

        private bool IsUnLock(int score)
        {
            if (rewardConfig == null)
                return false;

            return score >= rewardConfig.exchangeItemNum;
        }

        private bool IsWillUnLock()
        {
            TableBattlePassReward config = BattlePassModel.Instance.GetRewardConfig(BattlePassModel.Instance.CurScoreRatio, out var isEnd);
            if (config == null)
                return false;

            if (config != rewardConfig)
                return false;

            return true;
        }

        private bool IsPurchase()
        {
            return BattlePassModel.Instance.IsPurchase();
        }

        private bool IsPrucheGeting()
        {
            if (rewardConfig == null)
                return false;

            return BattlePassModel.Instance.IsGetReward(rewardConfig.id, false);
        }

        private bool IsNormalGetting()
        {
            if (rewardConfig == null)
                return false;

            return BattlePassModel.Instance.IsGetReward(rewardConfig.id, true);
        }

        private void OnButton_PurchaseGetting()
        {
            GuideSubSystem.Instance.FinishCurrent();

            if (rewardConfig == null)
                return;

            bool isPurchase = IsPurchase();
            if (!isPurchase)
            {
                //UITipsManager.Instance.ShowTips(LocalizationManager.Instance.GetLocalizedString("&key.UI_summer_challenge_help_7"));
                return;
            }

            bool isUnLock = IsUnLock(BattlePassModel.Instance.CurScoreRatio);
            if (!isUnLock)
            {
                //UITipsManager.Instance.ShowTips(LocalizationManager.Instance.GetLocalizedString("&key.UI_summer_challenge_help_6"));
                return;
            }

            bool isPrucheGeting = IsPrucheGeting();
            if (isPrucheGeting)
                return;

            var ret = new List<ResData>();
            for (int i = 0; i < rewardConfig.keyRewardId.Length; i++)
            {
                ret.Add(new ResData(rewardConfig.keyRewardId[i], rewardConfig.keyRewardNum[i]));
                if (!UserData.Instance.IsResource(rewardConfig.keyRewardId[i]))
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(rewardConfig.keyRewardId[i]);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonBp2,
                            itemAId = itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }

            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Bp2);
            reasonArgs.data1 = rewardConfig.id.ToString();
            reasonArgs.data2 = rewardConfig.keyRewardId.ToString();

            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true, reasonArgs, () => { });

            BattlePassModel.Instance.GetReward(rewardConfig.id, false);
            UnLockEvent();
        }

        private void OnButton_NormalGetting()
        {
            if (rewardConfig == null)
                return;

            bool isUnLock = IsUnLock(BattlePassModel.Instance.CurScoreRatio);
            if (!isUnLock)
            {
                //UITipsManager.Instance.ShowTips(LocalizationManager.Instance.GetLocalizedString("&key.UI_summer_challenge_help_6"));
                return;
            }

            bool isNormailGeting = IsNormalGetting();
            if (isNormailGeting)
                return;

            var ret = new List<ResData>();
            for (int i = 0; i < rewardConfig.normalRewardId.Length; i++)
            {
                ret.Add(new ResData(rewardConfig.normalRewardId[i], rewardConfig.normalRewardNum[i]));
                if (!UserData.Instance.IsResource(rewardConfig.normalRewardId[i]))
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(rewardConfig.normalRewardId[i]);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonBp1,
                            itemAId = itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }

            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Bp1);
            reasonArgs.data1 = rewardConfig.id.ToString();
            reasonArgs.data2 = rewardConfig.normalRewardId.ToString();

            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true, reasonArgs, () => { });

            BattlePassModel.Instance.GetReward(rewardConfig.id, true);
            UnLockEvent();
        }

        private void UnLockEvent(BaseEvent baseEvent = null)
        {
            UpdateUI(BattlePassModel.Instance.CurScoreRatio, false);
            // if (baseEvent != null && baseEvent.datas != null && baseEvent.datas.Length >= 1)
            // {
            //     bool isPurchase = (bool)baseEvent.datas[0];
            //     if(isPurchase)
            //         return;
            // }
            // else
            // {
            //     UpdateUI(BattlePassModel.Instance.CurScoreRatio);
            // }
        }

        private void PurchaseUnLockEvent(BaseEvent baseEvent)
        {
            // goldGroup.lockAnim.enabled = true;
            // StartCoroutine(CommonUtils.PlayAnimation(goldGroup.lockAnim, "appear", "", () =>
            // {
            //     UpdateUI(BattlePassModel.Instance.CurScoreRatio);
            // }));
        }

        private void InitRewardItem(Transform rewardItem, int rewardId, int rewardCount)
        {
            var rewardImage = rewardItem.GetComponent<Image>();
            if (rewardImage == null)
                return;

            rewardItem.gameObject.SetActive(true);
            var tipsBtn = rewardItem.Find("TipsBtn")?.GetComponent<Button>();
            if(tipsBtn != null)
                tipsBtn.gameObject.SetActive(false);
            
            if (UserData.Instance.IsResource(rewardId))
            {
                rewardImage.sprite = UserData.GetResourceIcon(rewardId, UserData.ResourceSubType.Reward);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(rewardId);
                if (itemConfig != null)
                    rewardImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                if (tipsBtn != null)
                {
                    tipsBtn.gameObject.SetActive(true);
                    tipsBtn.onClick.RemoveAllListeners();
                    tipsBtn.onClick.AddListener(() => { MergeInfoView.Instance.OpenMergeInfo(itemConfig, null, false, true); });
                }
            }

            var text = rewardItem.Find("Text")?.GetComponent<LocalizeTextMeshProUGUI>();
            if (text != null)
                text.SetText("x" + rewardCount);
        }
    }
}