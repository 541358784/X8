using System;
using System.Collections.Generic;
using Activity.BalloonRacing;
using Activity.RabbitRacing.Dynamic;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIStoreFlashItem : UIStoreBaseItem
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

    private Text _priceText;

    private ShopCostType _costType;
    private LocalizeTextMeshProUGUI _contentText;
    private LocalizeTextMeshProUGUI _numberText;
    private LocalizeTextMeshProUGUI _numberTextRare;
    private LocalizeTextMeshProUGUI _farmContentText;
    private LocalizeTextMeshProUGUI _farmNumberText;
    private Button _rvGO;
    private LocalizeTextMeshProUGUI _rvText;
    private Transform _coinGO, _diamonGO;
    private float clickTime;
    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;
    private LocalizeTextMeshProUGUI _rareText;
    Dictionary<string, string> extras = new Dictionary<string, string>();

    private Transform _rareBG;
    private Transform _farmBg;

    private Transform _balloonRacing;
    private Transform _rabbitRacing;
    protected override void PrivateAwake()
    {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _btnBuy = transform.Find("ButtonBuy").GetComponent<Button>();
        _btnHelp = transform.Find("HelpButton").GetComponent<Button>();
        _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
        _contentText = transform.Find("ContentText").GetComponent<LocalizeTextMeshProUGUI>();
        _numberText = transform.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>();
        _numberTextRare = transform.Find("NumberTextRare").GetComponent<LocalizeTextMeshProUGUI>();
        _rareText = transform.Find("RareText").GetComponent<LocalizeTextMeshProUGUI>();
        _rvGO = transform.Find("ButtonRv").GetComponent<Button>();
        _rvText = transform.Find("ButtonRv/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _coinGO = transform.Find("ButtonBuy/Coin");
        _diamonGO = transform.Find("ButtonBuy/Diamond");
        _rareBG = transform.Find("RareBG");
        _farmBg = transform.Find("FarmBG");
        _balloonRacing = transform.Find("BalloonRacing");
        _rabbitRacing = transform.Find("RabbitRacing");
        
        _farmContentText = transform.Find("ContentTextFarm").GetComponent<LocalizeTextMeshProUGUI>();
        _farmNumberText = transform.Find("NumberTextFarm").GetComponent<LocalizeTextMeshProUGUI>();
            
        _btnBuy.onClick.AddListener(OnClickBuy);
        _btnHelp.onClick.AddListener(OnClickHelp);
        _tagGroup = transform.Find("TagGroup");
        _tagText = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEventListener(EventEnum.FLASH_SALE_REFRESH,OnRefresh);
        
        InvokeRepeating("InvokeUpdate", 0, 1); 
    }

    private void OnRefresh(BaseEvent obj)
    {
        Refresh();
    }

    private void OnClickHelp()
    {
        MergeInfoView.Instance.OpenMergeInfo(_itemConfig,_isShowProbability:true);
        UIPopupMergeInformationController controller =
            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
        if (controller != null)
            controller.SetResourcesActive(false);
    }

    protected override void PrivateStart()
    {
        CommonUtils.SetShieldButUnEnable(_btnBuy.gameObject);
    }

    public void SetItemId(StorageStoreItem storeItem, int index, bool isInit = false)
    {
        this._storeItem = storeItem;
        this._index = index;

        _itemConfig = GameConfigManager.Instance.GetItemConfig(storeItem.ItemId);
    }

    public override void Refresh()
    {
        _btnHelp.gameObject.SetActive(true);
        _farmBg.gameObject.SetActive(false);
        _farmContentText.gameObject.SetActive(false);
        _farmNumberText.gameObject.SetActive(false);
        
        if (UserData.Instance.IsFarmRes(_storeItem.ItemId))
        {
            _btnHelp.gameObject.SetActive(false);
            var config = FarmConfigManager.Instance.TableFarmPropList.Find(a => a.Id == _storeItem.ItemId);
            if(config == null)
                return;

            _icon.sprite = FarmModel.Instance.GetFarmIcon(config.Image);
            bool canBuy = StoreModel.Instance.IsCanBuyFlashSaleItem(_index);
            if (canBuy)
                SetPrice();
            else
            {
                _tagGroup.gameObject.SetActive(false);
                _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
            }
            
            var leftCount = StoreModel.Instance.GetBuyFlashSaleCount(_index);
            string text = LocalizationManager.Instance.GetLocalizedString("UI_info_text11");
            _numberText.SetText(string.Format(text, leftCount));
            _numberTextRare.SetText(string.Format(text, leftCount));
            _farmNumberText.SetText(string.Format(text, leftCount));
            
            _contentText.SetTerm(config.NameKey);
            _farmContentText.SetTerm(config.NameKey);
            
            _btnBuy.interactable = canBuy;
            _coinGO.gameObject.SetActive(canBuy && _costType == ShopCostType.Coin);
            _diamonGO.gameObject.SetActive(canBuy && _costType == ShopCostType.Diamonds);
            if (leftCount > 0)
            {
                _rvGO.gameObject.SetActive(_costType == ShopCostType.Rv);
                _btnBuy.gameObject.SetActive(_costType != ShopCostType.Rv);
            }
            else
            {
                _rvGO.gameObject.SetActive(false);
                _btnBuy.gameObject.SetActive(true);
            }

            if(config.SpeedCoef == 0)
            {
                _numberText.gameObject.SetActive(config.SpeedCoef > 0);
                _numberTextRare.gameObject.SetActive(config.SpeedCoef == 0);
                _rareBG.gameObject.SetActive(config.SpeedCoef == 0);
                _rareText.gameObject.SetActive(config.SpeedCoef == 0);
            }
            else
            {
                _rareBG.gameObject.SetActive(false);
                _numberText.gameObject.SetActive(false);
                _numberTextRare.gameObject.SetActive(false);
                
                _farmBg.gameObject.SetActive(true);
                //_farmContentText.gameObject.SetActive(true);
                _farmNumberText.gameObject.SetActive(true);
            }
            
            return;
        }
        
        if (_itemConfig == null)
            return;

        _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(_itemConfig.image);
        bool isCanBuy = StoreModel.Instance.IsCanBuyFlashSaleItem(_index);
        if (isCanBuy)
            SetPrice();
        else
        {
            _tagGroup.gameObject.SetActive(false);
            _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
        }

        var countLeft = StoreModel.Instance.GetBuyFlashSaleCount(_index);
        string txt = LocalizationManager.Instance.GetLocalizedString("UI_info_text11");
        _numberText.SetText(string.Format(txt, countLeft));
        _numberTextRare.SetText(string.Format(txt, countLeft));
 
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
        _numberText.gameObject.SetActive(!_itemConfig.sold_confirm);
        _numberTextRare.gameObject.SetActive(_itemConfig.sold_confirm);
        _rareBG.gameObject.SetActive(_itemConfig.sold_confirm);
        _rareText.gameObject.SetActive(_itemConfig.sold_confirm);
        
    }

    private void SetPrice()
    {
        if (_storeItem == null)
        {
             Debug.LogError("flash sale 的id 出错:" + _storeItem.ItemId);
            return;
        }

        var price = StoreModel.Instance.GetFlashSaleItemPrice(_index);
        var disCount = _storeItem.Price > 0 ? Mathf.FloorToInt((1 - (float) price / _storeItem.Price) * 100) : 0;
        _tagText.SetText(disCount.ToString());
        _tagGroup.gameObject.SetActive(disCount > 0);
        if (_storeItem.PriceType == 3) //RV
        {
            var countLeft = StoreModel.Instance.GetBuyFlashSaleCount(_index);

            if (price > 0)
            {
                string rvtxt = countLeft <= 0
                    ? LocalizationManager.Instance.GetLocalizedString("button_soldout")
                    : StoreModel.Instance.GetWatchRvCount(_index) + "/" + price;
                _rvText.SetText(rvtxt);
                _costType = ShopCostType.Rv;
            }
            else
            {
                _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_free");
                _costType = ShopCostType.Free;
            }
        }
        else
        {
            string txt1 = price + "";
            if (price <= 0)
                txt1 = LocalizationManager.Instance.GetLocalizedString("button_free");
            _priceText.text = txt1;
            _costType = _storeItem.PriceType == 2 ? ShopCostType.Coin : ShopCostType.Diamonds;
        }
    }

    private new void OnClickBuy()
    {
        // if (Time.time - clickTime < 1.5f)
        //     return;
        // clickTime = Time.time;

        var price = StoreModel.Instance.GetFlashSaleItemPrice(_index);
        switch (_costType)
        {
            case ShopCostType.Coin:
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopFlashsaleBuy,
                    _storeItem.ItemId.ToString(),"1");
                int needCount=price;
                if (UserData.Instance.CanAford(UserData.ResourceId.Coin, price))
                {
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, price, new GameBIManager.ItemChangeReasonArgs
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopFlashSale,
                    });
                    AddRewardItem("coin");
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopFlashsaleGet,
                        _storeItem.ItemId.ToString(),"1");
                }
                else
                {
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Coin, "",
                        _storeItem.ItemId.ToString(), "coin_lack_flash",true,needCount);
                }

                break;
            case ShopCostType.Diamonds:
                extras.Clear();
                extras.Add("type", "1");
                extras.Add("amount", price.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopFlashsaleBuy,
                    _storeItem.ItemId.ToString(),"1");
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
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopFlashsaleGet,
                        _storeItem.ItemId.ToString(),"1");

                    BalloonRacingModel.Instance.AddFlashBuyScore(price, _btnBuy.transform.position);
                    RabbitRacingModel.Instance.AddFlashBuyScore(price, _btnBuy.transform.position);
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
        if (type == "rv")
        {
            StoreModel.Instance.AddWatchRvCount(_index);
            if (StoreModel.Instance.CanGetRvReward(_index))
            {
                StoreModel.Instance.ClearWatchRvCount(_index);
            }
            else
            {
                Refresh();
                return;
            }
        }

        StoreModel.Instance.BuyFlashSaleItem(_index);
        Refresh();

        if (UserData.Instance.IsFarmRes(_storeItem.ItemId))
        {
            FarmModel.Instance.AddProductItem(_storeItem.ItemId, 1);
        }
        else
        {
            var mergeItem = MergeManager.Instance.GetEmptyItem();

            mergeItem.Id = _storeItem.ItemId;
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
        }
        
        Vector3 endPos = Vector3.zero;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            endPos = MergeMainController.Instance.rewardBtnPos;
        }
        else
        {
            endPos = UIHomeMainController.mainController.MainPlayTransform.position;
        }

        if (UserData.Instance.IsFarmRes(_storeItem.ItemId))
        {
            if(FarmModel.Instance.IsFarmModel())
                endPos = FarmModel.Instance.WarehouseTransform.position;
            else
                endPos = UIHomeMainController.mainController.FarmTransform.position;
        }
        
        FlyGameObjectManager.Instance.FlyObject(_storeItem.ItemId, transform.position, endPos, 1.2f, 2.0f, 1f,
            () => { EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH); });
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FLASH_SALE_REFRESH,OnRefresh);
    }

    private void InvokeUpdate()
    {
        var leftCount = StoreModel.Instance.GetBuyFlashSaleCount(_index);
        if (leftCount <= 0)
        {
            _balloonRacing.gameObject.SetActive(false);
            _rabbitRacing.gameObject.SetActive(false);
            return;
        }
        _balloonRacing.gameObject.SetActive(BalloonRacingModel.Instance.IsShowReward());
        _rabbitRacing.gameObject.SetActive(RabbitRacingModel.Instance.IsShowReward());
    }
}