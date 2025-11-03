using System;
using DragonPlus.Config.TMatch;
using TMPro;
using UnityEngine.UI;
using DragonPlus;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus.Config.TMatchShop;
using UnityEngine;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;


namespace TMatch
{
    public class UitMatchSpaceOutViewParam : UIViewParam
    {
        public Action<bool> handleClick;
    }

    [AssetAddress("TMatch/Prefabs/UISettlementInsufficientSpace")]
    public class UITMatchSpaceOutController : UIPopup
    {
        public override Action EmptyCloseAction => null;
        private UitMatchSpaceOutViewParam _viewParam;
        private int closeOnClickTimes;
        private List<TMatchRewardItemData> loseItemDatas;
        private List<Transform> loseItems = new List<Transform>();
        private bool canFreeRevive;
        [ComponentBinder("Root/Root1/PlayButton")]
        private Button playButton;
        [ComponentBinder("PlayText")] private LocalizeTextMeshProUGUI playText;
        [ComponentBinder("Root/Root1/PlayButton/Coin")]
        private Transform buyCoinGroup;
        private TMatchReviveSystem showReviveType;
        [ComponentBinder("ReviveGift")] private Transform reviveGift;

        private void InitRewardUI()
        {
            Revive revive = TMatchConfigManager.Instance.GetRevive(2, TMatchPruchaseBoostSystem.Instance.RetainPurchaseCntInSingleLevel);
            transform.Find($"Root/Root1/PlayButton/Coin/NumberText").GetComponent<TextMeshProUGUI>().SetText(revive.reviveCost.ToString());
            if (revive.rewardID.Length == 1)
            {
                transform.Find($"Root/Root1/InsideGroup").gameObject.SetActive(true);
                transform.Find($"Root/Root1/InsideGroup1").gameObject.SetActive(false);
                transform.Find($"Root/Root1/InsideGroup2").gameObject.SetActive(false);
            }
            else if (revive.rewardID.Length == 2)
            {
                transform.Find($"Root/Root1/InsideGroup").gameObject.SetActive(false);
                transform.Find($"Root/Root1/InsideGroup1").gameObject.SetActive(true);
                transform.Find($"Root/Root1/InsideGroup2").gameObject.SetActive(false);

                for (int i = 0; i < revive.rewardID.Length; i++)
                {
                    var rewardId = revive.rewardID[i];
                    DragonPlus.Config.TMatchShop.ItemConfig item = TMatchShopConfigManager.Instance.GetItem(rewardId);
                    if (item.GetItemType() == ItemType.TMLighting)
                    {
                        transform.Find($"Root/Root1/InsideGroup1/PropGroup/Item2/Text").GetComponent<TextMeshProUGUI>().SetText($"X{revive.rewardCnt[i] * item.effectValue1}");
                    }
                }
            }
        }

        private void InitAdUI()
        {
            // if (!AdLogicManager.Instance.ShouldShowRV(eAdReward.TMatchReviveSpaceOut, true)) {
            //     transform.Find($"Root/Root2").gameObject.SetActive(false);
            // }
            transform.Find($"Root/Root2").gameObject.SetActive(false);
        }

        private void InitLoseUI()
        {
            AddLoseItem();
            RefreshLoseItem();
        }
        private void InitReviveGiftPackView()
        {
            AddChildView<ReviveGiftPackView>("TMatch/Prefabs/UIReviveGift", reviveGift, new ReviveSystemViewParam()
            {
                reviveReason = 1
            });
        }
        private void AddLoseItem()
        {
            loseItemDatas = new List<TMatchRewardItemData>();
            loseItemDatas.Add(new TMatchRewardItemData(TMatchAtlasName.Boost, "ui_common_icon_stars", false));
            // 暂时固定添加一个体力显示，后续如果有了无限体力，则这里不展示体力
            if (!EnergyModel.Instance.GetEnterGameIsUnlimitedState()) // 条件判断当前是否无限体力
                loseItemDatas.Add(new TMatchRewardItemData(TMatchAtlasName.Boost, "ui_common_icon_live", true, 1));
            // 收集物品
            if (UITMatchMainCollectItemView.collectItems.Count > 0)
            {
                foreach (var dictItem in UITMatchMainCollectItemView.collectItems)
                {
                    if (dictItem.Value > 0)
                    {
                        var matchCfg = TMatchConfigManager.Instance.GetItem(dictItem.Key);
                        var cnt = dictItem.Value;
                        var cfg = TMatchShopConfigManager.Instance.GetItem(matchCfg.boosterId);
                        loseItemDatas.Add(new TMatchRewardItemData(HospitalConst.TMatchAtlas, cfg.pic_res, true, cnt));
                    }
                }
            }

            // 帽子
            if (TMatchGoldenHatterSystem.Instance.GoldenHatterMarkValue > 0)
            {
                string iconName = "";
                if (TMatchGoldenHatterSystem.Instance.GoldenHatterMarkValue == 1)
                {
                    iconName = "ui_common_level_pull_1";
                }
                else if (TMatchGoldenHatterSystem.Instance.GoldenHatterMarkValue == 2)
                {
                    iconName = "ui_common_level_pull_2";
                }
                else
                {
                    iconName = "ui_common_level_pull_3";
                }

                loseItemDatas.Add(new TMatchRewardItemData(TMatchAtlasName.Common01, iconName, false));
            }
        }

        private void RefreshLoseItem()
        {
            if (loseItems.Count > 0)
            {
                for (var i = 0; i < loseItems.Count; i++)
                {
                    GameObject.Destroy(loseItems[i]);
                }

                loseItems.Clear();
            }

            transform.Find($"Root/Root1/InsideGroup2/Items1").gameObject.SetActive(loseItemDatas.Count > 4);
            if (loseItemDatas.Count > 4)
            {
                var root1 = transform.Find($"Root/Root1").GetComponent<RectTransform>();
                var size = root1.sizeDelta;
                size.y = 805;
                root1.sizeDelta = size;
                var root2 = transform.Find("Root/Root2");
                var pos = root2.localPosition;
                pos.y = -337;
                root2.localPosition = pos;
            }

            var transItem = transform.Find($"Root/Root1/InsideGroup2/Items/Items");
            transItem.gameObject.SetActive(false);
            for (var i = 0; i < loseItemDatas.Count; i++)
            {
                var parent = transItem.parent;
                if (i > 3)
                {
                    parent = transform.Find($"Root/Root1/InsideGroup2/Items1");
                }

                var trans = GameObject.Instantiate(transItem, parent);
                trans.gameObject.SetActive(true);
                var icon = trans.Find("Icon").GetComponent<Image>();
                var data = loseItemDatas[i];
                icon.sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, data.iconName);
                var numText = trans.Find("NumberText").GetComponent<TextMeshProUGUI>();
                if (data.isShowNum)
                {
                    numText.text = data.num.ToString();
                }
                else
                {
                    numText.text = "";
                }

                loseItems.Add(trans);
            }
        }

        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);

            TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchSystem.LevelController.LevelData.level);
            CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);

            transform.Find($"Root/Root1/PlayButton").GetComponent<Button>().onClick.AddListener(ContinueOnClick);
            transform.Find($"Root/Root1/CloseButton").GetComponent<Button>().onClick.AddListener(GiveupOnClick);
            transform.Find($"Root/Root2/ADButton").GetComponent<Button>().onClick.AddListener(AdButtonOnClick);
            showReviveType = ReviveGiftPackController.Instance.model.GetShowRevivePackType();

            InitRewardUI();
            InitAdUI();
            InitLoseUI();
            InitBottomShow();
            _viewParam = data as UitMatchSpaceOutViewParam;
            EventDispatcher.Instance.AddEventListener(EventEnum.BuyReviveGiftPackSuccess, OnBuyReviveGiftPackSuccessEvent);

        }
        private void InitBottomShow()
        {

            TMatchReviveSystem nowShowtype = TMatchModel.Instance.GetReviveBottonShowType();
            showReviveType = nowShowtype;
            if (showReviveType == TMatchReviveSystem.NoSystem)
            {
                reviveGift.gameObject.SetActive(false);
                return;
            }
            switch (nowShowtype)
            {
                case TMatchReviveSystem.ReviveGiftPack:
                    InitReviveGiftPackView();
                    break;
                case TMatchReviveSystem.GoldenPass:
                    InitBp();
                    break;
            }
        
            StorageManager.Instance.GetStorage<StorageTMatch>().ReviveShowTag = (int)nowShowtype;
            // if (nowShowtype != TMatchReviveSystem.NoSystem)
            // {
            //     goTime.SetActive(true);
            //     goClean.SetActive(false);
            //     txtDes.SetTerm("UI_finding_revive_pack_1");
            // }
        }
        private void InitBp()
        {
            // GameObject obj = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/TMatch/TMBP/TM_BPBuyItem");
            // GameObject bpObj = GameObject.Instantiate(obj);
            // bpObj.transform.SetParent(transform.Find("Bottom"));
            // bpObj.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
            // bpObj.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            // bpObj.SetActive(true);
            // TM_BPOtherBuyEntrance buyInfo = bpObj.AddComponent<TM_BPOtherBuyEntrance>();
            var buyInfo = AddChildView<TM_BPOtherBuyEntrance>("Prefabs/Activity/TMatch/TMBP/TM_BPBuyItem", reviveGift, new ReviveSystemViewParam()
            {
                reviveReason = 1
            });
            buyInfo.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
            buyInfo.Init((shopLv) => { OnBuyBp();});
        }
        private void OnBuyBp()
        {
            OnBuy();
        }
    
        private void OnBuy()
        {
            UIViewSystem.Instance.Close<UITMatchSpaceOutController>();
            var revive = TMatchConfigManager.Instance.GetRevive(2, TMatchPruchaseBoostSystem.Instance.RetainPurchaseCntInSingleLevel);
            foreach (var t in revive.rewardID)
            {
                EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(t));
            }
            TMatchPruchaseBoostSystem.Instance.RetainPurchaseCntInSingleLevel++;
            _viewParam?.handleClick?.Invoke(true);
        }
        
        private async void ContinueOnClick()
        {
            playButton.interactable = false;
            Revive revive = TMatchConfigManager.Instance.GetRevive(2, TMatchPruchaseBoostSystem.Instance.RetainPurchaseCntInSingleLevel);
            if (!canFreeRevive && !ItemModel.Instance.IsEnough((int)ResourceId.TMCoin, revive.reviveCost))
            {
                // IAPController.Instance.SetIAPBiParaPlacement(BiEventMatchFrenzy.Types.MonetizationIAPEventPlacement.PlacementCoinNotEnough, 
                //     "spaceout");
                UIViewSystem.Instance.Open<ShopPartPopup>();
                await Task.Delay(1000);
                playButton.interactable = true;
                return;
            }

            if (!canFreeRevive)
            {
                ItemModel.Instance.Cost((int)ResourceId.TMCoin, revive.reviveCost, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SpaceReliveTm
                });
                EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(ResourceId.TMCoin));
                TMatchPruchaseBoostSystem.Instance.RetainPurchaseCntInSingleLevel++;

            }
            
            // CurrencyModel.Instance.CostRes(ResourceId.Coin, revive.reviveCost, 
            //     new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventMatchFrenzy.Types.ItemChangeReason.SpaceRelive));
            UIViewSystem.Instance.Close<UITMatchSpaceOutController>();
            // GameBIManager.Instance.LevelInfo.SpaceRespawnCount++;
            for (int i = 0; i < revive.rewardID.Length; i++)
            {
                EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(revive.rewardID[i]));
            }

            CrocodileActivityModel.Instance.GameRevive();
            _viewParam?.handleClick?.Invoke(true);
        }

        private void GiveupOnClick()
        {
            if (closeOnClickTimes++ >= 1)
            {
                UIViewSystem.Instance.Close<UITMatchSpaceOutController>();
                _viewParam?.handleClick?.Invoke(false);
            }
            else
            {
                AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
                transform.Find($"Root/Root1/InsideGroup").gameObject.SetActive(false);
                transform.Find($"Root/Root1/InsideGroup1").gameObject.SetActive(false);
                transform.Find($"Root/Root1/InsideGroup2").gameObject.SetActive(true);
            }
        }

        private void AdButtonOnClick()
        {
            // if (AdLogicManager.Instance.ShouldShowRV(eAdReward.TMatchReviveSpaceOut, false))
            // {
            //     AdLogicManager.Instance.TryShowRewardedVideo(eAdReward.TMatchReviveSpaceOut, (success, str) => {
            //         if (success) {
            //             // var addTimes = 15;
            //             Revive revive = TMatchConfigManager.Instance.GetRevive(2, TMatchPruchaseBoostSystem.Instance.RetainPurchaseCntInSingleLevel);
            //             TMatchPruchaseBoostSystem.Instance.RetainPurchaseCntInSingleLevel++;
            //             UIViewSystem.Instance.Close<UITMatchSpaceOutController>();
            //             GameBIManager.Instance.LevelInfo.SpaceRespawnAdCount++;
            //             for (int i = 0; i < revive.AdRewardID.Length; i++)
            //             {
            //                 EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(revive.AdRewardID[i]));
            //             }
            //         
            //             _viewParam?.handleClick?.Invoke(true);
            //         }
            //     });
            // }
            // else
            // {
            //     CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            //     {
            //         DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_loading_ADS"),
            //         TitleString = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_box_tittle"),
            //     });
            // }
        }

        public override Task OnViewClose()
        {
            SetAllButtonInteractable(false);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BuyReviveGiftPackSuccess, OnBuyReviveGiftPackSuccessEvent);

            return base.OnViewClose();
        }

        private void SetAllButtonInteractable(bool interactable)
        {
            transform.Find($"Root/Root1/PlayButton").GetComponent<Button>().interactable = interactable;
            transform.Find($"Root/Root1/CloseButton").GetComponent<Button>().interactable = interactable;
            transform.Find($"Root/Root2/ADButton").GetComponent<Button>().interactable = interactable;
        }
        private void OnBuyReviveGiftPackSuccessEvent(BaseEvent evt)
        {
            canFreeRevive = true;
            playText.SetTerm("ui_tm_clearup");
            playText.transform.localPosition = new Vector3(0, 13.2f, 0);
            buyCoinGroup.gameObject.SetActive(false);
        }
    }
}