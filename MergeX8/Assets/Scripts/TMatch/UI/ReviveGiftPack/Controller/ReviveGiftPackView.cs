using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Network.API.Protocol;

namespace TMatch
{
    public class ReviveSystemViewParam : UIViewParam
    {
        public int reviveReason; // 复活原因 1超时，2超空间
    }

    public class ReviveGiftPackView : UIView
    {
        protected override bool IsChildView => true;
        [ComponentBinder("BuyButton")] private Button buyButton;
        [ComponentBinder("Text")] private Text buyText;
        [ComponentBinder("ItemsGroup")] private Transform itemsGroup;
        [ComponentBinder("Icon1")] private Transform item1;
        [ComponentBinder("Root/Icon")] private Image coinIcon;

        [ComponentBinder("Root/Icon/NumberText")]
        private LocalizeTextMeshProUGUI coinText;

        [ComponentBinder("Adorn")] private Image reviveTypeImage;
        [ComponentBinder("DesText")] private LocalizeTextMeshProUGUI descText;
        [ComponentBinder("Tag")] private Transform discountTag;
        [ComponentBinder("TagText1")] private LocalizeTextMeshProUGUI tagText;

        private List<GameObject> itemList = new List<GameObject>();
        private TMReviveGiftPack showPackCfg;
        private bool buySuccess;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            DragonPlus.GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmpackageRiseIn);
            if (gameObject.GetComponent<Canvas>() != null)
            {
                gameObject.GetComponent<Canvas>().sortingOrder = 550;   
            }
            InitUI();
            var viewParam = param as ReviveSystemViewParam;
            if (viewParam.reviveReason == 1)
            {
                descText.SetTerm("ui_tm_package_revival_desc");
                // reviveTypeImage.sprite =
                //     ResourcesManager.Instance.GetSpriteVariant("BoostAtlas", "ui_common_icon_time");
            }
            else
            {
                descText.SetTerm("ui_tm_package_revival_desc");
            }

            buyButton.onClick.AddListener(OnBuyButtonClicked);

            EventDispatcher.Instance.AddEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
        }

        public override Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
            if (!buySuccess)
            {
                ReviveGiftPackController.Instance.model.AddNotBuyTimes();
            }

            ReviveGiftPackController.Instance.isInPurchase = false;
            return base.OnViewClose();
        }

        private void InitUI()
        {
            ReviveGiftPackController.Instance.model.SetShowTime();
            showPackCfg = ReviveGiftPackController.Instance.model.GetCurrentShowPack();

            if (showPackCfg.Discount == 100)
            {
                discountTag.gameObject.SetActive(false);
            }
            else
            {
                tagText.SetText(showPackCfg.Discount + "%");
            }

            coinText.SetText(showPackCfg.ItemNums[0].ToString());
            item1.gameObject.SetActive(false);
            for (int i = 1; i < showPackCfg.ItemIds.Count; i++)
            {
                GameObject itemObj = GameObject.Instantiate(item1.gameObject, itemsGroup);
                itemList.Add(itemObj);
                itemObj.SetActive(true);
                ItemConfig cfg = TMatchShopConfigManager.Instance.GetItem(showPackCfg.ItemIds[i]);
                itemObj.transform.Find("Icon").GetComponent<Image>().sprite = ItemModel.Instance.GetItemSprite(cfg.id);
                itemObj.transform.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText(
                    cfg.infinity
                        ? CommonUtils.FormatPropItemTime((long) (cfg.infiniityTime * 1000))
                        : showPackCfg.ItemNums[i].ToString());
                itemObj.transform.Find("InfiniteTag").gameObject
                    .SetActive(cfg.type == (int) ItemType.TMEnergyInfinity);
            }

            buyText.text = StoreModel.Instance.GetPrice(showPackCfg.ShopId);
        }

        private void OnBuyButtonClicked()
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmBpclickLifeUseup,data1:"0");
            ReviveGiftPackController.Instance.isInPurchase = true;
            var purchaseId = showPackCfg.ShopId;
            // IAPController.Instance.SetIAPBiParaPlacement(BiEventHappyMatchCafe.Types.MonetizationIAPEventPlacement
                // .PlacementUnknown);
            StoreModel.Instance.Purchase(purchaseId, null);
        }

        private void OnIAPSuccessEvent(BaseEvent evt)
        {
            IAPSuccessEvent drivedEvt = evt as IAPSuccessEvent;
            if (drivedEvt.shop.id == showPackCfg.ShopId)
            {
                BuySuccess();

                // List<ItemData> itemDatas = new List<ItemData>();
                // for (int i = 0; i < showPackCfg.ItemIds.Count; i++)
                // {
                //     ItemData itemData = new ItemData(){ id = showPackCfg.ItemIds[i], cnt = showPackCfg.ItemNums[i] };
                //     itemDatas.Add(itemData);
                // }
                // CommonUtils.AddRewards(itemDatas, new BiUtil.ItemChangeReasonArgs(BiEventHappyMatchCafe.Types.ItemChangeReason.Iap){ data1 = drivedEvt.shop.Product_id });

                //coin
                {
                    int id = showPackCfg.ItemIds[0];
                    int cnt = showPackCfg.ItemNums[0];
                    FlySystem.Instance.FlyItem(id, cnt, coinIcon.transform.position,
                        FlySystem.Instance.GetShopTargetTransform(id).position, null);
                }
                //boost
                for (int i = 1; i < showPackCfg.ItemIds.Count; i++)
                {
                    int id = showPackCfg.ItemIds[i];
                    int cnt = showPackCfg.ItemNums[i];
                    var curIndex = i;
                    FlySystem.Instance.FlyItem(id, cnt, itemList[i - 1].transform.position,
                        FlySystem.Instance.GetShopTargetTransform(id).position,
                        () =>
                        {
                            if (curIndex == showPackCfg.ItemIds.Count - 1)
                            {
                                gameObject.SetActive(false);
                                // UIViewSystem.Instance.Close<ReviveGiftPackView>();
                            }
                        });
                }
            }
        }

        private void BuySuccess()
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmBpbuyLifeUseup,data1:"0");
            DragonPlus.GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmpackageRise,
                data1: (showPackCfg.Level-1).ToString());
            buySuccess = true;
            buyButton.interactable = false;
            ReviveGiftPackController.Instance.model.BuySuccess();
            EventDispatcher.Instance.DispatchEvent(EventEnum.BuyReviveGiftPackSuccess);
            
        }
    }
}