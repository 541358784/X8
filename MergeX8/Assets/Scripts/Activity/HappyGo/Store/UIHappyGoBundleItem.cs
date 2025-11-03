using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.HappyGo;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIHappyGoBundleItem : MonoBehaviour
{
    public UIStoreItemType ItemType { get; } = UIStoreItemType.Bundle;

    protected Button BuyButton
    {
        get { return _btnBuy; }
    }

    private TableShop _cfg;

    public TableShop Cfg
    {
        get { return _cfg; }
    }

    private ShowArea _showArea;

    public ShowArea showArea
    {
        get { return _showArea; }
    }

    private string _openSrc;

    public string OpenSrc
    {
        get { return openSrc; }
    }

    public string openSrc;

    private HGVDBundle _bundleItem;
    private Button _btnBuy;

    private Transform _rewardItem;
    private Transform _rewardParent;
    private Text _priceText;

    private LocalizeTextMeshProUGUI _contentText;

    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;
    private LocalizeTextMeshProUGUI _tipText;
    private LocalizeTextMeshProUGUI _timeText;
    private StorageHGBundleItem _packItem;

    private HappyGoGameGiftTitleItem _gameGiftTitleItem;
    private int _type;

    protected void Awake()
    {
        _btnBuy = transform.Find("BuyButton").GetComponent<Button>();
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();
        _contentText = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();

        _tagGroup = transform.Find("TipGroup");
        _tipText = transform.Find("TipGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _tagText = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardParent = transform.Find("RewardGroup");
        _rewardItem = transform.Find("Item1 (1)");
        _rewardItem.gameObject.SetActive(false);
        BuyButton.onClick.AddListener(OnClickBuy);

        InvokeRepeating("UpdateTimeText", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.HAPPYGOBUNDLE_PURCHASE_REFRESH, PurchseRefresh);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HAPPYGOBUNDLE_PURCHASE_REFRESH, PurchseRefresh);
        CancelInvoke("UpdateTimeText");
    }

    public void UpdateTimeText()
    {
        if (_packItem == null)
            return;

        _timeText.SetText(HappyGoPackModel.Instance.GetPackLeftTimeStr());
    }

    private void PurchseRefresh(BaseEvent obj)
    {
        var pack = HappyGoPackModel.Instance.GetBundleByType((PackEnum) _type);
        if (pack != null)
        {
            _packItem = pack;
            _bundleItem = HappyGoModel.Instance.GetTableHgBundle(_packItem.ShopId);
            _cfg = GlobalConfigManager.Instance.GetTableShopByID(_packItem.ShopId);
            Refresh();
        }else{
            DeleteBundle();
        }
    }

    public void Init(TableShop cfg, StorageHGBundleItem bundleItem,int type, HappyGoGameGiftTitleItem gameGiftTitleItem,
        ShowArea area = ShowArea.none, string src = "")
    {
        _type=type;
        _showArea = area;
        openSrc = src;
        if (cfg == null)
            return;
        _cfg = cfg;
        _bundleItem = HappyGoModel.Instance.GetTableHgBundle(bundleItem.ShopId);
        _gameGiftTitleItem = gameGiftTitleItem;
        _packItem = bundleItem;
        Refresh();
    }

    public void DeleteBundle()
    {
        Destroy(gameObject);        
        _gameGiftTitleItem.ResetPageScrollView();

        if (HappyGoPackModel.Instance.GetCanBuyCount() > 0)
            CommonUtils.DelayedCall(0.5f, DeLayRefresh);
        else
        {
            _gameGiftTitleItem.gameObject.SetActive(false);
            HappyGoUIStoreGameController.Instance?.ReBuild();
        }
    }

    private void DeLayRefresh()
    {
        HappyGoUIStoreGameController.Instance?.ReBuild();
        _gameGiftTitleItem.ResetPageScrollView();
    }

    public void Refresh()
    {
        CommonUtils.DestroyAllChildren(_rewardParent);
        _priceText.text = (StoreModel.Instance.GetPrice(Cfg.id));

        _tagText.SetText(_bundleItem.labelNum + "%");
        string txt = LocalizationManager.Instance.GetLocalizedString("UI_info_text11");
        _tipText.SetText(string.Format(txt, _packItem.LeftBuyCount));
        _contentText.SetTerm(_bundleItem.name);
        List<int> itemIds = new List<int>(_bundleItem.bundleItemList);
        List<int> itemCounts = new List<int>(_bundleItem.bundleItemCountList);

        for (int i = 0; i < itemIds.Count; i++)
        {
            var item = Instantiate(_rewardItem, _rewardParent);
            item.gameObject.SetActive(true);
            if (UserData.Instance.IsResource(itemIds[i]))
            {
                item.transform.Find("Icon").GetComponent<Image>().sprite =
                    UserData.GetResourceIcon(itemIds[i], UserData.ResourceSubType.Reward);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(itemIds[i]);
                item.transform.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }

            item.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(itemCounts[i].ToString());
            var _helpButon = item.transform.Find("HelpButton").GetComponent<Button>();
            int index = i;
            _helpButon.gameObject.SetActive(!UserData.Instance.IsResource(itemIds[index]));
            _helpButon.onClick.AddListener(() =>
            {
                MergeInfoView.Instance.OpenMergeInfo(itemIds[index],_isShowProbability:true,isShowGetResource:false);
                UIPopupMergeInformationController controller =
                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                if (controller != null)
                    controller.SetResourcesActive(false);
            });
        }
    }


    protected void OnClickBuy()
    {
        int buyID = Cfg.id;
        
        AppConst.tempButRemoveAdsType = "store";
        AppConst.tempBuyStarPackType = "store";
        AppConst.tempBuySpecialPackType = "store";
        AppConst.tempBuyWeeklyCardType = "store";
        StoreModel.Instance.nowItemPos = this.transform.position;
        StoreModel.Instance.Purchase(buyID, openSrc, _bundleItem.shopItemId);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdBuypackageShop,_bundleItem.id.ToString());

    }
}