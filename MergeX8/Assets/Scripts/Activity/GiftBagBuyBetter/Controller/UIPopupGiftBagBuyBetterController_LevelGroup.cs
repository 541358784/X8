using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGiftBagBuyBetterController : UIWindowController
{
    public enum LevelState
    {
        Complete,
        Normal,
        Lock,
    }
    public class LevelGroup : MonoBehaviour
    {
        public int Index;
        private GiftBagBuyBetterResource Config;
        private Button BuyBtn;
        private Button LockBtn;
        private Transform CompleteFlag;
        private Transform DefaultRewardItem;
        private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
        private Transform TagGroup;
        private LocalizeTextMeshProUGUI TagText;
        private LevelState State;
        private UIPopupGiftBagBuyBetterController MainUI;
        private Text PriceText;
        private Text PriceTextLock;
        
        private void Awake()
        {
            BuyBtn = transform.Find("Button").GetComponent<Button>();
            BuyBtn.onClick.AddListener(OnClickBuyBtn);
            LockBtn = transform.Find("ButtonLock").GetComponent<Button>();
            CompleteFlag = transform.Find("ButtonFinish");
            DefaultRewardItem = transform.Find("ItemGroup/Item");
            DefaultRewardItem.gameObject.SetActive(false);
            TagGroup = transform.Find("TagGroup");
            if (TagGroup)
            {
                TagText = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
            }
            PriceText = transform.Find("Button/Text").GetComponent<Text>();
            PriceTextLock = transform.Find("ButtonLock/Text").GetComponent<Text>();
        }
        public void SetConfig(GiftBagBuyBetterResource config,int index,UIPopupGiftBagBuyBetterController mainUI)
        {
            MainUI = mainUI;
            Config = config;
            Index = index;
            if (TagGroup)
            {
                TagGroup.gameObject.SetActive(!Config.Label.IsEmptyString());
                TagText.SetText(Config.Label);
            }

            var rewards = CommonUtils.FormatReward(Config.RewardID, Config.Amount);
            for (var i = 0; i < rewards.Count; i++)
            {
                var rewardItem = Instantiate(DefaultRewardItem, DefaultRewardItem.parent).gameObject
                    .AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(rewards[i]);
                RewardItems.Add(rewardItem);
                rewardItem.transform.SetAsLastSibling();
            }
            if (Config.ConsumeType == 2)
            {
                TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(Config.ConsumeAmount);
                if (PriceText)
                    PriceText.text = StoreModel.Instance.GetPrice(tableShop.id);
                if (PriceTextLock)
                    PriceTextLock.text = StoreModel.Instance.GetPrice(tableShop.id);
                
                transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(Config.ConsumeAmount));
            }
            else
            {
                transform.Find("Vip").gameObject.SetActive(false); 
            }
            UpdateState();
            
        }

        public void UpdateState()
        {
            var curIndex = GiftBagBuyBetterModel.Instance.GetCurIndex();
            if (Index == curIndex)
                State = LevelState.Normal;
            else if (Index > curIndex)
                State = LevelState.Lock;
            else if (Index < curIndex)
                State = LevelState.Complete;   
        }
        public void UpdateUI()
        {
            BuyBtn.gameObject.SetActive(State == LevelState.Normal);
            LockBtn.gameObject.SetActive(State == LevelState.Lock);
            if (CompleteFlag)
                CompleteFlag.gameObject.SetActive(State == LevelState.Complete);
        }

        public void OnClickBuyBtn()
        {
            MainUI.GetLevelReward(this);
        }

        public void PerformComplete()
        {
            UpdateState();
            UpdateUI();
        }

        public void PerformUnlock()
        {
            UpdateState();
            UpdateUI();
        }
    }
}