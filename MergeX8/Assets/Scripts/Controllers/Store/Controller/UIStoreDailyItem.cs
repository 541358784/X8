using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using ExtraEnergy;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIStoreDailyItem : UIStoreBaseItem
{
    public override UIStoreItemType ItemType { get; } = UIStoreItemType.Daily;

    protected override Button BuyButton
    {
        get { return _btnBuy; }
    }

    private Button _btnBuy;
    private Button _helpButton;
    private Button rv_go;
    private Transform coin_go, diamon_go;

    private Image _icon;

    private Text _priceText;
    private float clickTime;
    private LocalizeTextMeshProUGUI _contentText;

    private DailyShop _dailyShop;
    private TableMergeItem _itemConfig;
    private bool isRvItem;
    private Transform tagGroup;
    private LocalizeTextMeshProUGUI tagText;
    private LocalizeTextMeshProUGUI _addText;

    protected override void PrivateAwake()
    {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _btnBuy = transform.Find("ButtonBuy").GetComponent<Button>();
        _btnBuy.onClick.AddListener(OnClickBuy);
        _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
        _contentText = transform.Find("ContentText").GetComponent<LocalizeTextMeshProUGUI>();

        _helpButton = transform.Find("HelpButton").GetComponent<Button>();
        _helpButton.gameObject.SetActive(true);
        _helpButton.onClick.AddListener(() => { MergeInfoView.Instance.OpenMergeInfo(_itemConfig,_isShowProbability:true); });

        rv_go = transform.Find("ButtonRv").GetComponent<Button>();

        coin_go = transform.Find("ButtonBuy/Coin");
        diamon_go = transform.Find("ButtonBuy/Diamond");
        tagGroup = transform.Find("TagGroup");
        tagText = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
        _addText = transform.Find("TagGroupEnergy/Text1").GetComponent<LocalizeTextMeshProUGUI>();
    }

    protected override void PrivateStart()
    {
        InvokeRepeating("RefreshTime",0,1);

    }
    public void RefreshTime()
    {
        if (ExtraEnergyModel.Instance.IsOpened() && _dailyShop!=null && _dailyShop.ItemId==(int)UserData.ResourceId.Energy)
        {
            var storageItem = StoreModel.Instance.GetStorageItem(ID);
            _contentText.SetTerm(Cfg.amount+"+"+ ExtraEnergyModel.Instance.GetExtraEnergyActivityConfigByCount(storageItem.BuyCount).ExtraEnergy);
            _addText.transform.parent.gameObject.SetActive(true);
            _addText.SetText(ExtraEnergyModel.Instance.GetExtraEnergyActivityConfigByCount(storageItem.BuyCount).ExtraEnergy+"%");
        }
        else
        {
            _addText.transform.parent.gameObject.SetActive(false);
            _contentText.SetTerm(Cfg.amount.ToString());
        }
    }
    public override void Refresh()
    {
        _dailyShop = AdConfigHandle.Instance.GetDailyShops().Find(x => x.ShopItemId == ID);
        if (_dailyShop == null)
        {
            Debug.LogError("--_dailyShop 配置错误 ID---" + ID);
            return;
        }

        gameObject.SetActive(_dailyShop.Sold == 1 && ExperenceModel.Instance.GetLevel() >= _dailyShop.Unlock_id);
        if (UserData.Instance.IsResource(_dailyShop.ItemId))
        {
         
            _icon.sprite = UserData.GetResourceIcon(_dailyShop.ItemId, UserData.ResourceSubType.Reward);// ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, Cfg.image);
            _helpButton.gameObject.SetActive(false);
        }
        else
        {
            _itemConfig = GameConfigManager.Instance.GetItemConfig(_dailyShop.ItemId);
            if (_itemConfig == null)
            {
                DebugUtil.LogError("商品表配置错误---ItemID " + _dailyShop.ItemId);
                return;
            }

            _contentText.SetTerm(_itemConfig.name_key);
            Sprite s = MergeConfigManager.Instance.mergeIcon.GetSprite(_itemConfig.image);
            _icon.sprite = s;
        }

        SetPrice();
    }

    private bool SetPrice()
    {
        bool canBuy = false;
        var storageItem = StoreModel.Instance.GetStorageItem(ID);
        if (_dailyShop == null)
            return canBuy;
        isRvItem = _dailyShop.Price[0] == 3;
        var price = _dailyShop.Price[1] + storageItem.PriceAdd;
        var price_original = _dailyShop.Price_original != null && _dailyShop.Price_original.Count >= 2
            ? _dailyShop.Price_original[1]
            : 0;
        var disCount = price_original > 0 ? Mathf.FloorToInt((1 - (float) price / price_original) * 100) : 0;
        tagText.SetText(disCount.ToString());
        tagGroup.gameObject.SetActive(disCount > 0);
        switch (_dailyShop.Price[0])
        {
            case 1: //钻石
                _priceText.text = string.Format("{0}", price);
                canBuy = storageItem.BuyCount < _dailyShop.Amount;
                if (!canBuy)
                    _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
                rv_go.gameObject.SetActive(false);
                _btnBuy.gameObject.SetActive(true);
                _btnBuy.interactable = canBuy;
                coin_go.gameObject.SetActive(false);
                diamon_go.gameObject.SetActive(canBuy);
                break;
            case 2: // 金币
                _priceText.text = string.Format("{0}", price);
                canBuy = storageItem.BuyCount < _dailyShop.Amount;
                if (!canBuy)
                    _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
                rv_go.gameObject.SetActive(false);
                _btnBuy.interactable = canBuy;
                _btnBuy.gameObject.SetActive(true);
                coin_go.gameObject.SetActive(canBuy);
                diamon_go.gameObject.SetActive(false);
                break;
            case 3: //广告
                if (storageItem.BuyCount >= _dailyShop.Amount)
                    _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
                rv_go.interactable = canBuy;
                rv_go.gameObject.SetActive(true);
                _btnBuy.gameObject.SetActive(false);
                break;
            case 4: //免费
                canBuy = storageItem.BuyCount < 1;
                _btnBuy.interactable = canBuy;
                _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_free");
                if (!canBuy)
                    _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
                coin_go.gameObject.SetActive(false);
                diamon_go.gameObject.SetActive(false);
                rv_go.gameObject.SetActive(false);
                break;
        }

        return canBuy;
    }

    private void OnClickBuy()
    {
        if (Time.time - clickTime < 1.5f)
            return;
        var storageItem = StoreModel.Instance.GetStorageItem(ID);
        int price = _dailyShop.Price[1] + storageItem.PriceAdd;
        var extras = new Dictionary<string, string>();

        switch (_dailyShop.Price[0])
        {
            case 1: //钻石
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardClick,
                    "gem");
                extras.Clear();
                extras.Add("type", "1");
                extras.Add("amount", price.ToString());
                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
                {
                    var reson = new GameBIManager.ItemChangeReasonArgs(
                        BiEventAdventureIslandMerge.Types.ItemChangeReason.DailyReward)
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DailyReward
                    };
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, price, reson);
                    AddRewardItem("gem");
                    StoreModel.Instance.AddItemCount(ID, _dailyShop);
                    SetPrice();
                }
                else
                {
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_info_text14"),
                        HasCloseButton = true,
                    });
                }

                break;
            case 2: // 金币
     
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardClick,
                    "coin");
                if (UserData.Instance.CanAford(UserData.ResourceId.Coin, price))
                {
                    var reson = new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DailyReward
                    };
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, price, reson);
                    StoreModel.Instance.AddItemCount(ID, _dailyShop);
                    AddRewardItem("coin");
                    SetPrice();
                }
               

                break;
            case 4: //免费
      
                StoreModel.Instance.AddItemCount(ID, null);
                AddRewardItem("get");
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventShopDailyRewardClick);
                SetPrice();
                break;
        }

        clickTime = Time.time;
    }

    private void AddRewardItem(string type)
    {
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Shop,AdLocalOperate.Operate);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardGet,type);
        if (UserData.Instance.IsResource(_dailyShop.ItemId))
        {
            GameBIManager.ItemChangeReasonArgs reasonArgs =
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.DailyReward);
            int gemGetCount = Cfg.amount;
            if (ExtraEnergyModel.Instance.IsOpened() && _dailyShop.ItemId==(int)UserData.ResourceId.Energy)
            {
                var storageItem = StoreModel.Instance.GetStorageItem(ID);
                gemGetCount += ExtraEnergyModel.Instance.GetExtraEnergyActivityConfigByCount(storageItem.BuyCount).ExtraEnergy;
            }
            UserData.Instance.AddRes((int) _dailyShop.ItemId, gemGetCount, reasonArgs, false);
            FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                (UserData.ResourceId) _dailyShop.ItemId, gemGetCount,
                transform.position, 0.8f, true, true, 0.15f);
        }
        else
        {
            var mergeItem = MergeManager.Instance.GetEmptyItem();
            mergeItem.Id = _dailyShop.ItemId;
            mergeItem.State = 1;
            MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);
            var itemConfig = GameConfigManager.Instance.GetItemConfig(_dailyShop.ItemId);
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeShopDailyDeals,
                itemAId = itemConfig.id,
                ItemALevel = itemConfig.level,
                isChange = true,
                extras = new Dictionary<string, string>
                {
                    {"type", type},
                }
            });
            Vector3 endPos = Vector3.zero;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                endPos = MergeMainController.Instance.rewardBtnPos;
            }
            else
            {
                endPos = UIHomeMainController.mainController.MainPlayTransform.position;
            }

            FlyGameObjectManager.Instance.FlyObject(mergeItem.Id, transform.position, endPos, 1f, 2.0f, 1f, () =>
            {
                EventDispatcher.Instance.DispatchEvent(MergeEvent.DAILYDEALS_PURCHASE_SUCCESS);
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            });
        }
    }
}