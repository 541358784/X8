using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.ABTest;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;
using ABTestManager = ABTest.ABTestManager;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;


public class UIStorePigSaleItem : UIStoreBaseItem
{
    public enum ShopCostType
    {
        Diamonds,
        Coin,
        Rv,
        Free,
    }

    public override UIStoreItemType ItemType { get; } = UIStoreItemType.Flash;

    protected override Button BuyButton
    {
        get { return _btnBuy; }
    }

    private StorageStoreItem _storeItem;
    private int _index;
    private TableMergeItem _itemConfig;
    private Button _btnBuy;
    private Button _btnHelp;

    private Image _icon;
    private Image _iconBg;

    private Text _priceText;

    private ShopCostType _costType;
    private LocalizeTextMeshProUGUI _contentText;
    private LocalizeTextMeshProUGUI _numberText;
    private Button _rvGO;
    private LocalizeTextMeshProUGUI _rvText;
    private Transform _coinGO, _diamonGO;
    private float clickTime;
    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;
    Dictionary<string, string> extras = new Dictionary<string, string>();
    private LocalizeTextMeshProUGUI _rareText;
    private Transform _rareBG;
    
    protected override void PrivateAwake()
    {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _iconBg = transform.Find("BG").GetComponent<Image>();
        _btnBuy = transform.Find("ButtonBuy").GetComponent<Button>();
        _btnHelp = transform.Find("HelpButton").GetComponent<Button>();
        _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
        _contentText = transform.Find("ContentText").GetComponent<LocalizeTextMeshProUGUI>();
        _numberText = transform.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>();

        _rvGO = transform.Find("ButtonRv").GetComponent<Button>();
        ;
        _rvText = transform.Find("ButtonRv/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _coinGO = transform.Find("ButtonBuy/Coin");
        _diamonGO = transform.Find("ButtonBuy/Diamond");
        _btnBuy.onClick.AddListener(OnClickBuy);
        _btnHelp.onClick.AddListener(OnClickHelp);
        _tagGroup = transform.Find("TagGroup");
        _tagText = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
        _rareText = transform.Find("RareText").GetComponent<LocalizeTextMeshProUGUI>();
        _rareText.gameObject.SetActive(false);
        _rareBG = transform.Find("RareBG");
        _rareBG.gameObject.SetActive(false);
        
        transform.Find("BalloonRacing").gameObject.SetActive(false);
    }

    private void OnClickHelp()
    {
        MergeInfoView.Instance.OpenMergeInfo(_itemConfig,_isShowProbability:true);
    }

    protected override void PrivateStart()
    {
        CommonUtils.SetShieldButUnEnable(_btnBuy.gameObject);
    }

    public void SetItemId(StorageStoreItem storeItem, int index, bool isInit = false,ShowArea firstArea=ShowArea.none)
    {
        this._storeItem = storeItem;
        this._index = index;
        _rareBG.gameObject.SetActive(false);
        _rareText.gameObject.SetActive(false);
        _itemConfig = GameConfigManager.Instance.GetItemConfig(storeItem.ItemId);
        if (index == 0 && isInit)
        {   
            UIAdRewardButton.Create(ADConstDefine.Rv_FLASH_BUY, UIAdRewardButton.ButtonStyle.Disable, _rvGO.gameObject,
                (s) =>
                {
                    if (s)
                    {
                        extras.Clear();
                        extras.Add("type", "3");
                        AddRewardItem("rv");
                    }
                }, true, null, () =>
                {
                    extras.Clear();
                    extras.Add("type", "3");
               
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardClick,
                        "rv");
                });
        }
        else if (index == 1 && isInit)
        {
            // gameObject.SetActive(!ABTestManager.Instance.IsOpenADTest());
            UIAdRewardButton.Create(ADConstDefine.Rv_FLASH_BUY2, UIAdRewardButton.ButtonStyle.Disable, _rvGO.gameObject,
                (s) =>
                {
                    if (s)
                    {
                        AddRewardItem("rv");
                    }
                }, true, null, () =>
                {
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardClick,
                        "rv");
                });
        }
    }

    public override void Refresh()
    {
        if (_itemConfig == null)
            return;
        //
        // if (_iconBg != null)
        //     _iconBg.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, "UI_Common_BG_Shop_6");
        //

        _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(_itemConfig.image);
        bool isCanBuy = StoreModel.Instance.IsCanBuyPigSaleItem(_index);
        if (isCanBuy)
            SetPrice();
        else
        {
            _tagGroup.gameObject.SetActive(false);
            _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
        }

        var countLeft = StoreModel.Instance.GetBuyPigSaleCount(_index);
        string txt = LocalizationManager.Instance.GetLocalizedString("UI_info_text11");
        _numberText.SetText(string.Format(txt, countLeft));
        _numberText.gameObject.SetActive(true);
        _contentText.SetTerm(_itemConfig.name_key);
        _btnBuy.interactable = isCanBuy;
        _coinGO.gameObject.SetActive(isCanBuy && _costType == ShopCostType.Coin);
        _diamonGO.gameObject.SetActive(isCanBuy && _costType == ShopCostType.Diamonds);
        if (countLeft > 0)
        {
            _rvGO.gameObject.SetActive(_costType == ShopCostType.Rv);
            _btnBuy.gameObject.SetActive(_costType != ShopCostType.Rv);
        }
        else
        {
            _rvGO.gameObject.SetActive(false);
            _btnBuy.gameObject.SetActive(true);
        }

        if (_index == 0)
        {
            if (VipStoreModel.Instance.VipLevel() <= 3)
            {
                _btnBuy.interactable = false;
                _priceText.text = string.Format(LocalizationManager.Instance.GetLocalizedString("ui_vipsystem_VipUnlock"), 4);
            }
        }
    }

    private void SetPrice()
    {
        if (_itemConfig == null || _storeItem == null)
        {
            Debug.LogError("pig sale 的id 出错:" + _storeItem.ItemId);
            return;
        }

        var price = StoreModel.Instance.GetPigSaleItemPrice(_index);
        var disCount = _storeItem.Price > 0 ? Mathf.FloorToInt((1 - (float) price / _storeItem.Price) * 100) : 0;
        _tagText.SetText(disCount.ToString());
        _tagGroup.gameObject.SetActive(disCount > 0);
        int buyCount = StoreModel.Instance.GetPigSaleItemBuyCount(_index);
        if (_storeItem.PriceType == 3) //RV
        {
            var countLeft = StoreModel.Instance.GetBuyPigSaleCount(_index);

            if (price > 0)
            {
                string rvtxt = countLeft <= 0
                    ? LocalizationManager.Instance.GetLocalizedString("button_soldout")
                    : StoreModel.Instance.GetPigWatchRvCount(_index) + "/" + price;
                _rvText.SetText(rvtxt);
                _costType = ShopCostType.Rv;
            }
            else
            {
                _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_free");
                ;
                _costType = ShopCostType.Free;
            }
        }
        else
        {
            string txt1 = price + "";
            if (price <= 0)
            {
                txt1 = LocalizationManager.Instance.GetLocalizedString("button_free");
                _costType = ShopCostType.Free;
            }
            else
            {
                _costType = _storeItem.PriceType == 2 ? ShopCostType.Coin : ShopCostType.Diamonds;
            }

            _priceText.text = txt1;
        }
    }

    private new void OnClickBuy()
    {
        // if (Time.time - clickTime < 1.5f)
        //     return;
        // clickTime = Time.time;

        var price = StoreModel.Instance.GetPigSaleItemPrice(_index);
        switch (_costType)
        {
            case ShopCostType.Coin:
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardClick,
                    "coin");
                int needCount=price;
                if (UserData.Instance.CanAford(UserData.ResourceId.Coin, price))
                {
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, price, new GameBIManager.ItemChangeReasonArgs
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopFlashSale,
                    });
                    AddRewardItem("coin");
            
                }
                else
                {
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Coin, "",
                        _storeItem.ItemId.ToString(), "coin_lack_flash",true,needCount);
                }

                break;
            case ShopCostType.Diamonds:
          
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardClick,
                    "gem");
                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
                {
                    var reason = new GameBIManager.ItemChangeReasonArgs
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopFlashSale,
                        data1 = _storeItem.ItemId.ToString(),
                        data2 = "1"
                    };
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, price, reason);
                    AddRewardItem("gem");
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
            case ShopCostType.Free:
                AddRewardItem("free");
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardClick,
                    "free");
                break;
        }
    }

    private int GetRandomReward()
    {
        var configs = AdConfigHandle.Instance.GetFlashSaleBoxs();
        var starNum = UserData.Instance.GetTotalDecoCoin();
        var config = configs.Find(a => { return a.StarMin <= starNum && starNum <= a.StarMax; });
        var index = CommonUtils.RandomIndexByWeight(new List<int>(config.Weight));

        return config.Reward[index];
    }

    private void AddRewardItem(string type)
    {
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopDailyRewardGet,
            "gem",_storeItem.ItemId.ToString());

        if (type == "rv")
        {
            StoreModel.Instance.AddPigWatchRvCount(_index);
            if (StoreModel.Instance.CanGetPigRvReward(_index))
            {
                StoreModel.Instance.ClearPigWatchRvCount(_index);
            }
            else
            {
                Refresh();
                return;
            }
        }

        StoreModel.Instance.BuyPigSaleItem(_index);
        Refresh();
        var mergeItem = MergeManager.Instance.GetEmptyItem();
        if (_storeItem.ItemId == 60000)
        {
            mergeItem.Id = GetRandomReward();
        }
        else
        {
            mergeItem.Id = _storeItem.ItemId;
        }

        mergeItem.State = 1;
        MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);

        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeShopFlashSale,
            itemAId = mergeItem.Id,
            ItemALevel = _itemConfig.level,
            isChange = true,
            extras = new Dictionary<string, string>
            {
                {"type", type},
            }
        });
        Vector3 endPos;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            endPos = MergeMainController.Instance.rewardBtnPos;
        }
        else
        {
            endPos = UIHomeMainController.mainController.MainPlayTransform.position;
        }

        FlyGameObjectManager.Instance.FlyObject(mergeItem.Id, transform.position, endPos, 1.2f, 2.0f, 1f,
            () => { EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH); });
    }
}