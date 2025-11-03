using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BattlePass
{
    public enum BuyType
    {
        Copper,
        Golden,
        Ultimate
    }
    public class BattlePassBuyGroup
    {
        public GameObject _gameObject;
        private BuyType _buyType;
        private List<RewardData> _RewardDatas = new List<RewardData>();
        private Button _payButton;
        private Action<int> _payAction = null;
        private Text _payText;
        private int _shopId = 0;
        private LocalizeTextMeshProUGUI _vipText;
        public void Init(GameObject gameObject, BuyType type, Action<int> payAction)
        {
            _gameObject = gameObject;
            _buyType = type;
            _payAction = payAction;
            
            bool isOldUser = BattlePassModel.Instance.IsOldUser();
            _shopId = BattlePassModel.Instance.GetShopId(type);
            
            _payButton = gameObject.transform.Find("BuyButton").GetComponent<Button>();
            _payText = gameObject.transform.Find("BuyButton/Text1").GetComponent<Text>();
            _payText.text = StoreModel.Instance.GetPrice(_shopId);
            
            _vipText = gameObject.transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(_shopId));
            
            if(!isOldUser && type == BuyType.Copper)
                gameObject.SetActive(false);
            
            switch (type)
            {
                case BuyType.Copper:
                {
                    RewardData data = new RewardData();
                    _RewardDatas.Add(data);

                    data.gameObject = gameObject.transform.Find("Reward").gameObject;
                    data.image = gameObject.transform.Find("Reward/Image").GetComponent<Image>();
                    data.numText = gameObject.transform.Find("Reward/Text").GetComponent<LocalizeTextMeshProUGUI>();
                    
                    data.UpdateReward(BattlePassModel.Instance.BattlePassShopConfig.oldRewardType_1[0], BattlePassModel.Instance.BattlePassShopConfig.oldRewardNum_1[0]);
                    break;
                }
                case BuyType.Golden:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        RewardData data = new RewardData();
                        _RewardDatas.Add(data);
                        string path = "";
                        if (i == 0)
                        {
                            path = "Reward1";
                        }
                        else
                        {
                            path = "RewardGroup/Reward" + (i + 1);
                        }
                        data.gameObject = gameObject.transform.Find(path).gameObject;
                        data.gameObject.gameObject.SetActive(false);
                        data.image = gameObject.transform.Find($"{path}/Image").GetComponent<Image>();
                        data.numText = gameObject.transform.Find($"{path}/Text").GetComponent<LocalizeTextMeshProUGUI>();
                    }

                    int[] rewardType;
                    int[] rewardNum;
                    if (isOldUser)
                    {
                        rewardType = BattlePassModel.Instance.BattlePassShopConfig.oldRewardType_2;
                        rewardNum = BattlePassModel.Instance.BattlePassShopConfig.oldRewardNum_2;
                    }
                    else
                    {
                        rewardType = BattlePassModel.Instance.BattlePassShopConfig.newRewardType_1;
                        rewardNum = BattlePassModel.Instance.BattlePassShopConfig.newRewardNum_1;
                    }

                    UpdateRewardData(rewardType, rewardNum);
                    break;
                }
                case BuyType.Ultimate:
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        RewardData data = new RewardData();
                        _RewardDatas.Add(data);
                        string path = "";
                        if (i <= 2)
                        {
                            path = "RewardGroup/Reward" + i;
                        }
                        else
                        {
                            path = "Reward" + i;
                        }
                        data.gameObject = gameObject.transform.Find(path).gameObject;
                        data.gameObject.gameObject.SetActive(false);
                        data.image = gameObject.transform.Find($"{path}/Image").GetComponent<Image>();
                        data.numText = gameObject.transform.Find($"{path}/Text").GetComponent<LocalizeTextMeshProUGUI>();
                    }
                    
                    int[] rewardType;
                    int[] rewardNum;
                    if (isOldUser)
                    {
                        rewardType = BattlePassModel.Instance.BattlePassShopConfig.oldRewardType_3;
                        rewardNum = BattlePassModel.Instance.BattlePassShopConfig.oldRewardNum_3;
                    }
                    else
                    {
                        rewardType = BattlePassModel.Instance.BattlePassShopConfig.newRewardType_2;
                        rewardNum = BattlePassModel.Instance.BattlePassShopConfig.newRewardNum_2;
                    }

                    UpdateRewardData(rewardType, rewardNum);
                    break;
                }
            }
            
            _payButton.onClick.AddListener(() =>
            {
                _payAction?.Invoke(_shopId);
            });
        }

        private void UpdateRewardData(int[] rewardType, int[] rewardNum)
        {
            int addIndex = 0;
            for (var i = 0; i < rewardType.Length; i++)
            {
                int rdType = rewardType[i];
                int rdNum = rewardNum[i];
                
                switch (rdType)
                {
                    case 1: //经验
                    {
                        if (_buyType == BuyType.Ultimate && i <= 1)
                            addIndex = 1;
                        _RewardDatas[i+addIndex].numText.SetText(rdNum.ToString());
                        break;
                    }
                    case 2: //倍数
                    {
                        if (_buyType ==BuyType.Ultimate && i <= 1)
                            addIndex = 1;
                        
                        _RewardDatas[i+addIndex].numText.SetText("x"+((100f+rdNum)/100f).ToString("0.0"));
                        break;
                    }
                    default:
                    {
                        _RewardDatas[i].UpdateReward(rdType, rdNum);
                        break;
                    }
                }
                _RewardDatas[i+addIndex].gameObject.SetActive(true);
            }
        }
    }
    public class UIPopupBattlePassBuyNew : UIWindowController
    {
        public BattlePassBuyGroup _copperGroup = new BattlePassBuyGroup();
        public BattlePassBuyGroup _goldenGroup = new BattlePassBuyGroup();
        public BattlePassBuyGroup _ultimateGroup = new BattlePassBuyGroup();
        
        private Button _buttonClose;
        private ScrollRect _scrollView;
        public override void PrivateAwake()
        {
            _scrollView = transform.Find("Root/BuyGroup").GetComponent<ScrollRect>();
            
            _copperGroup.Init(transform.Find("Root/BuyGroup/Content/CopperPass").gameObject, BuyType.Copper, PayAction);
            _goldenGroup.Init(transform.Find("Root/BuyGroup/Content/GoldenPass").gameObject, BuyType.Golden, PayAction);
            _ultimateGroup.Init(transform.Find("Root/BuyGroup/Content/UltimateGoldenPass").gameObject, BuyType.Ultimate, PayAction);
            
            _buttonClose = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _buttonClose.onClick.AddListener(OnClose);
            InitUI();
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_PURCHASE, OnCloseUI);
        }
        
        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            int openSrc = 1;
            if (objs != null && objs.Length > 0)
                openSrc = (int)objs[0];
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpBuyBuy, BattlePassModel.Instance.storageBattlePass.PurchasePopCount.ToString());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpBuyPop, openSrc.ToString());
            BattlePassModel.Instance.storageBattlePass.PurchasePopCount++;

            if (BattlePassModel.Instance.IsOldUser())
                StartCoroutine(ScrollViewAnim());
        }

        IEnumerator ScrollViewAnim()
        {
            yield return new WaitForSeconds(0.5f);

            _scrollView.DOHorizontalNormalizedPos(1, 0.5f).SetEase(Ease.Linear);
        }
        
        public void InitUI()
        {
            var config = BattlePassModel.Instance.BattlePassActiveConfig;
        }
        
        private void OnClose()
        {
            AnimCloseWindow();
        }
        
        private void OnCloseUI(BaseEvent e)
        {
            OnClose();
        }
        
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_PURCHASE, OnCloseUI);

        }
        
        private void PayAction(int shpId)
        {
            StoreModel.Instance.Purchase(shpId, "battlePass");
        }
        
        public static bool CanShowUIWithOpenWindow()
        {
            if (!CanShowUIWithOutOpenWindow())
                return false;
            UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePassBuyNew1);
            return true;
        }

        public static bool CanShowUIWithOutOpenWindow()
        {
            if (!BattlePassModel.Instance.IsOpened())
                return false;
            if (BattlePassModel.Instance.IsPurchase())
                return false;
            return true;
        }
    }
}