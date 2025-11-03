
using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BattlePass_2
{
    public class UIPopupBattlePassEndBuyController : UIWindowController
    {
        private Button _buttonClose;
        private Button _buttonBuy;
        private Text _priceText;
        private Button _receiveBtn;
        private Transform rewardItem;
        private TableShop _shop;
        private LocalizeTextMeshProUGUI _vipText;

        private LocalizeTextMeshProUGUI _descText;
        private LocalizeTextMeshProUGUI _titleText;

        public enum EndBuyOpenType
        {
            EndBuy,
            Receive
        }

        public override void PrivateAwake()
        {
            _buttonBuy = GetItem<Button>("Root/BuyButton");
            _buttonBuy.onClick.AddListener(OnBtnBuy);
            _priceText = GetItem<Text>("Root/BuyButton/Text1");

            _buttonClose = GetItem<Button>("Root/ButtonClose");
            _buttonClose.onClick.AddListener(OnClose);

            _receiveBtn = GetItem<Button>("Root/ReceiveButton");
            _receiveBtn.onClick.AddListener(OnReceive);
            rewardItem = transform.Find("Root/Scroll View/Viewport/Content/Item");
            rewardItem.gameObject.SetActive(false);
            _descText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
            _titleText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/TextTitle");
            _vipText = gameObject.transform.Find("Root/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, OnCloseUI);

        }

        private StorageBattlePass _storageBattlePass;
        private EndBuyOpenType _openType;

        protected override void OnOpenWindow(params object[] objs)
        {
            if (objs != null && objs.Length > 0)
            {
                _storageBattlePass = (StorageBattlePass)objs[0];
                _openType = (EndBuyOpenType)objs[1];
            }

            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpEndBuyPop);

            InitUI();
        }


        private void OnClose()
        {
            if (_openType == EndBuyOpenType.EndBuy)
            {
                AnimCloseWindow(() =>
                {
                    if (BattlePassModel.Instance.IsCanGetAllReward(_storageBattlePass))
                        UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePass2EndBuy, _storageBattlePass, UIPopupBattlePassEndBuyController.EndBuyOpenType.Receive);
                    else
                    {
                        _storageBattlePass.IsGetAllReward = true;
                    }
                });
            }
            else
            {
                AnimCloseWindow(() => { BattlePassModel.Instance.PopCommonReward(_storageBattlePass, _shop); });
            }
        }

        private void OnCloseUI(BaseEvent e)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpEndBuyBuySuccess);
            AnimCloseWindow();
        }


        public void InitUI()
        {
            string buyKey = "battlepass2end_buy";
            if (!BattlePassModel.Instance.IsOldUser())
                buyKey = "battlepass2end_buy_new";
            
            _shop = GlobalConfigManager.Instance.GetTableShopByID(GlobalConfigManager.Instance.GetNumValue(buyKey));
            _priceText.text = StoreModel.Instance.GetPrice(_shop.id);
            List<ResData> resDatas;
            //_titleText.SetTerm("ui_battlepass_title");
            if (_openType == EndBuyOpenType.EndBuy)
            {
                _descText.SetTerm("ui_battlepass_end_desc");
                _receiveBtn.gameObject.SetActive(false);
                resDatas = GetRewardList(_storageBattlePass, true);
            }
            else
            {
                resDatas = GetRewardList(_storageBattlePass, false);
                _buttonBuy.gameObject.SetActive(false);
                _descText.SetTerm("ui_battlepass_end_reward");

            }

            foreach (var res in resDatas)
            {
                var item = Instantiate(rewardItem, rewardItem.parent);
                item.gameObject.SetActive(true);
                InitRewardItem(item, res.id, res.count);
            }
            _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(_shop.price));
        }

        private void OnReceive()
        {
            AnimCloseWindow(() => { BattlePassModel.Instance.PopCommonReward(_storageBattlePass, _shop); });
        }

        private void OnBtnBuy()
        {
            BattlePassModel.Instance.isActivityEndBuy = true;
            StoreModel.Instance.Purchase(_shop.id, "battlePass");
        }

        public List<ResData> GetRewardList(StorageBattlePass storageBattlePass, bool isEndBuy)
        {
            var ret = new List<ResData>();
            foreach (var kv in storageBattlePass.Reward)
            {
                if (storageBattlePass.ActivityScore < kv.UnlockScore)
                    continue;

                if (!kv.IsNormalGet)
                {
                    for (int i = 0; i < kv.NormalRewardIds.Count; i++)
                    {
                        ret.Add(new ResData(kv.NormalRewardIds[i], kv.NormalRewardCounts[i]));
                    }
                }

                if ((storageBattlePass.IsPurchase || isEndBuy) && !kv.IsPurchaseGet)
                {
                    for (int i = 0; i < kv.PurchaseRewardIds.Count; i++)
                    {
                        ret.Add(new ResData(kv.PurchaseRewardIds[i], kv.PurchaseRewardCounts[i]));
                    }
                }
            }

            return ret;
        }

        private void InitRewardItem(Transform rewardItem, int rewardId, int rewardCount)
        {
            var rewardImage = rewardItem.Find("Icon").GetComponent<Image>();
            if (rewardImage == null)
                return;

            var tipsBtn = rewardItem.Find("TipsBtn")?.GetComponent<Button>();
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
                    tipsBtn.onClick.AddListener(() => { MergeInfoView.Instance.OpenMergeInfo(itemConfig, null, false, true); });
                }

            }

            var text = rewardItem.Find("Num")?.GetComponent<LocalizeTextMeshProUGUI>();
            if (text != null)
                text.SetText("x" + rewardCount);

        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, OnCloseUI);

        }

    }
}
