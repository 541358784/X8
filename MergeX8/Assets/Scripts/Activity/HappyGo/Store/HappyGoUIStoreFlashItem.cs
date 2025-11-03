using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class HappyGoUIStoreFlashItem : UIStoreBaseItem
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
    private Button _rvGO;
    private LocalizeTextMeshProUGUI _rvText;
    private Transform _coinGO, _diamonGO;
    private float clickTime;
    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;
    private LocalizeTextMeshProUGUI _rareText;
    Dictionary<string, string> extras = new Dictionary<string, string>();

    private Transform _rareBG;
    private HappyGoGameGiftTitleItem _titleItem;
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
        _btnBuy.onClick.AddListener(OnClickBuy);
        _btnHelp.onClick.AddListener(OnClickHelp);
        _tagGroup = transform.Find("TagGroup");
        _tagText = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
    }

    private void OnClickHelp()
    {
        MergeInfoView.Instance.OpenMergeInfo(_itemConfig,_isShowProbability:true,isShowGetResource:false);
        UIPopupMergeInformationController controller =
            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
        if (controller != null)
            controller.SetResourcesActive(false);
    }

    protected override void PrivateStart()
    {
        CommonUtils.SetShieldButUnEnable(_btnBuy.gameObject);
    }

    public void SetItemId(StorageStoreItem storeItem, int index, bool isInit = false,HappyGoGameGiftTitleItem titleItem=null)
    {
        this._storeItem = storeItem;
        this._index = index;
        if (titleItem != null)
            _titleItem = titleItem;

        _itemConfig = GameConfigManager.Instance.GetItemConfig(storeItem.ItemId);
    }

    public override void Refresh()
    {
        if (_itemConfig == null)
            return;

        _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(_itemConfig.image);
        bool isCanBuy = HappyGoStoreModel.Instance.IsCanBuyFlashSaleItem(_index);
        if (isCanBuy)
            SetPrice();
        else
        {
            _tagGroup.gameObject.SetActive(false);
            _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
        }

        var countLeft = HappyGoStoreModel.Instance.GetBuyFlashSaleCount(_index);
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
       
        _titleItem?.UpdateRefreshStatus();
        
    }

    private void SetPrice()
    {
        if (_itemConfig == null || _storeItem == null)
        {
            if (_storeItem != null) Debug.LogError("flash sale 的id 出错:" + _storeItem.ItemId);
            return;
        }

        var price = HappyGoStoreModel.Instance.GetFlashSaleItemPrice(_index);
        var disCount = _storeItem.Price > 0 ? Mathf.FloorToInt((1 - (float) price / _storeItem.Price) * 100) : 0;
        _tagText.SetText(disCount.ToString());
        _tagGroup.gameObject.SetActive(disCount > 0);
        if (_storeItem.PriceType == 3) //RV
        {
            var countLeft = HappyGoStoreModel.Instance.GetBuyFlashSaleCount(_index);

            if (price > 0)
            {
                string rvtxt = countLeft <= 0
                    ? LocalizationManager.Instance.GetLocalizedString("button_soldout")
                    : HappyGoStoreModel.Instance.GetWatchRvCount(_index) + "/" + price;
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

        var price = HappyGoStoreModel.Instance.GetFlashSaleItemPrice(_index);
        switch (_costType)
        {
            case ShopCostType.Coin:
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopFlashsaleBuy,
                    "coin");
                int needCount = price;
                if (UserData.Instance.CanAford(UserData.ResourceId.Coin, price))
                {
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, price, new GameBIManager.ItemChangeReasonArgs
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopFlashSale,
                    });
                    AddRewardItem("coin");
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopFlashsaleGet,
                        "coin",_storeItem.ItemId.ToString());
                }
                else
                {
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Coin, "",
                        _storeItem.ItemId.ToString(), "coin_lack_flash",true,needCount);
                }

                break;
            case ShopCostType.Diamonds:
               
                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
                {
                    var reason = new GameBIManager.ItemChangeReasonArgs
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.HgVdShopFlashBuy,
                    };
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, price, reason);
                    AddRewardItem("gem");
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdShopFlashBuy,_itemConfig.id.ToString(),price.ToString());

                }
                else
                {
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                        "" ,"openSr",needCount:price);
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
            HappyGoStoreModel.Instance.AddWatchRvCount(_index);
            if (HappyGoStoreModel.Instance.CanGetRvReward(_index))
            {
                HappyGoStoreModel.Instance.ClearWatchRvCount(_index);
            }
            else
            {
                Refresh();
                return;
            }
        }

        HappyGoStoreModel.Instance.BuyFlashSaleItem(_index);
        Refresh();
        var mergeItem = MergeManager.Instance.GetEmptyItem();

        mergeItem.Id = _storeItem.ItemId;
        mergeItem.State = 1;
        MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.HappyGo);

        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeReasonHgVdShopFlashGet,
            itemAId = mergeItem.Id,
            ItemALevel = _itemConfig.level,
            isChange = true,
            extras = new Dictionary<string, string>
            {
                {"type", type},
            }
        });
        Vector3 endPos = Vector3.zero;
        if (HappyGoMainController.Instance != null && HappyGoMainController.Instance.gameObject.activeSelf)
        {
            endPos = HappyGoMainController.Instance.rewardBtnPos;
        }
        else
        {
            endPos = UIHomeMainController.mainController.MainPlayTransform.position;
        }

        FlyGameObjectManager.Instance.FlyObject(mergeItem.Id, transform.position, endPos, 1.2f, 2.0f, 1f,
            () => { EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH); });
    }
}