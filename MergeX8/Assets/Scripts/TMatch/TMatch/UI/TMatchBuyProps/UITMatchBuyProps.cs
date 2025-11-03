using System.Threading.Tasks;
using DragonPlus.Config.TMatch;
using TMPro;
using UnityEngine.UI;
using DragonU3DSDK.Asset;
using UnityEngine;
using DragonPlus;
using System;
using DragonU3DSDK.Network.API.Protocol;


namespace TMatch
{
    public class UITMatchBuyPropsData : UIViewParam
    {
        public DragonPlus.Config.TMatchShop.ItemConfig inItem = null;
        public Action closeAction;
    }

    [AssetAddress("TMatch/Prefabs/UIBuyProps")]
    public class UITMatchBuyProps : UIPopup
    {
        public override Action EmptyCloseAction => GiveupOnClick;
        private int closeOnClickTimes;
        private DragonPlus.Config.TMatchShop.ItemConfig item;
        private Action closeAction;

        [ComponentBinder("Root/BuyButton")] private Button buyButton;

        public void RefreshUI()
        {
            var globalConfig = TMatchConfigManager.Instance.GlobalList[0];
            // taskItem.obj.transform.Find("Icon").GetComponent<RawImage>().texture =
            //         ResourcesManager.Instance.LoadResource<Texture2D>($"Textures/TMatchItemIcon/{taskItem.itemCfg.PrefabName}");
            transform.Find($"Root/PropGroup/PropIcon").GetComponent<Image>().sprite =
                // ResourcesManager.Instance.LoadResource<Sprite>($"Textures/Boost/{item.Icon}");
                ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, item.pic_res);
            transform.Find($"Root/PropGroup/NumberText").GetComponent<TextMeshProUGUI>().SetText(globalConfig.BoosterBuyLimit.ToString());
            var totalSpend = globalConfig.BoosterBuyLimit * item.price;
            transform.Find($"Root/BuyButton/NumberText").GetComponent<TextMeshProUGUI>().SetText(totalSpend.ToString());

            var nameStr = LocalizationManager.Instance.GetLocalizedString($"&key.{item.name}");
            transform.Find($"Root/TitleGroup/TitleText").GetComponent<LocalizeTextMeshProUGUI>().SetText(nameStr);
            var tipStr = LocalizationManager.Instance.GetLocalizedString($"&key.{item.sourceDesc}");
            transform.Find($"Root/TipsText").GetComponent<LocalizeTextMeshProUGUI>().SetText(tipStr);
        }

        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);

            TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchModel.Instance.GetMainLevel());
            CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);

            buyButton.onClick.AddListener(BuyOnClick);
            transform.Find($"Root/CloseButton").GetComponent<Button>().onClick.AddListener(GiveupOnClick);
            transform.Find($"Root/NOButton").GetComponent<Button>().onClick.AddListener(GiveupOnClick);

            UITMatchBuyPropsData windowData = data as UITMatchBuyPropsData;
            item = windowData.inItem;
            closeAction = windowData.closeAction;
            if (item != null)
            {
                RefreshUI();
            }
        }

        public override async Task OnViewClose()
        {
            await base.OnViewClose();
            closeAction?.Invoke();
        }

        private async void BuyOnClick()
        {
            buyButton.interactable = false;
            var globalConfig = TMatchConfigManager.Instance.GlobalList[0];
            var totalSpend = item.price * globalConfig.BoosterBuyLimit;
            // var userCoins = CurrencyModel.Instance.GetRes(ResourceId.Coin);
            var userCoins = ItemModel.Instance.GetNum((int)ResourceId.TMCoin);
            if (userCoins > totalSpend)
            {
                // var args = new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventMatchFrenzy.Types.ItemChangeReason.BuyItem);
                // args.data1 = item.ItemId.ToString();
                // args.data2 = globalConfig.BoosterBuyLimit.ToString();
                // if (MyMain.myGame.IsInMatch())
                // {
                //     args.data3 = TMatchModel.Instance.GetMainLevel().ToString();
                // }
                // CurrencyModel.Instance.CostRes(ResourceId.Coin, totalSpend, args);
                ItemModel.Instance.Cost((int)ResourceId.TMCoin, totalSpend, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug
                });
                EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(ResourceId.TMCoin));

                var addItemNum = globalConfig.BoosterBuyLimit;
                ItemModel.Instance.Add(item.id, addItemNum, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug
                },addType:1);
                // CurrencyModel.Instance.AddRes(item.GetResouceId(), addItemNum, new DragonPlus.GameBIManager.ItemChangeReasonArgs{
                //     reason = BiEventMatchFrenzy.Types.ItemChangeReason.BuyItem,
                //     data1 = totalSpend.ToString()
                // });

                EventDispatcher.Instance.DispatchEvent(new GameItemChangeEvent(item.id));
                UIViewSystem.Instance.Close<UITMatchBuyProps>();
            }
            else
            {
                // IAPController.Instance.SetIAPBiParaPlacement(BiEventMatchFrenzy.Types.MonetizationIAPEventPlacement.PlacementCoinNotEnough, 
                //     item.ItemId.ToString());
                UIViewSystem.Instance.Open<ShopPartPopup>();
                await Task.Delay(1000);
                buyButton.interactable = true;
            }
        }

        private void GiveupOnClick()
        {
            UIViewSystem.Instance.Close<UITMatchBuyProps>();
            // if (closeOnClickTimes++ >= 1)
            // {
            //     UIManager.Instance.CloseUI<UITMatchBuyProps>();
            //     UIManager.Instance.OpenWindow<UITMatchFailController>(GlobalPrefabPath.UITMatchFail);
            // }
            // else
            // {
            //     transform.Find($"Root/{rootName}/Root1/InsideGroup").gameObject.SetActive(false);
            //     transform.Find($"Root/{rootName}/Root1/InsideGroup1").gameObject.SetActive(true);
            // }
        }
    }
}