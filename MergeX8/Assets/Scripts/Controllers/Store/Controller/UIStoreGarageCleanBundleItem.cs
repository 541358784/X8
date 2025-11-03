using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreGarageCleanBundleItem : MonoBehaviour
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

    private TableBundle _bundle;
    private Button _btnBuy;

    private Transform _rewardItem;
    private Transform _rewardParent;
    private Text _priceText;

    private LocalizeTextMeshProUGUI _contentText;

    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;
    private LocalizeTextMeshProUGUI _timeText;

    private GameGiftTitleItem _gameGiftTitleItem;

    protected void Awake()
    {
        _btnBuy = transform.Find("BuyButton").GetComponent<Button>();
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();
        _contentText = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();

        _tagGroup = transform.Find("TipGroup");
        _tagText = transform.Find("TipGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardParent = transform.Find("RewardGroup");
        _rewardItem = transform.Find("Item1 (1)");
        _rewardItem.gameObject.SetActive(false);
        BuyButton.onClick.AddListener(OnClickBuy);

        InvokeRepeating("UpdateTimeText", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.GARAGE_CLEANUP_PURCHASE_REFRESH, PurchseRefresh);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GARAGE_CLEANUP_PURCHASE_REFRESH, PurchseRefresh);
        CancelInvoke("UpdateTimeText");
    }

    public void UpdateTimeText()
    {
        if(GarageCleanupModel.Instance.GetActivityLeftTime()<=0)
            DeleteBundle();
        
        _timeText.SetText(GarageCleanupModel.Instance.GetActivityLeftTimeString());
    }

    private void PurchseRefresh(BaseEvent obj)
    {
        if (!GarageCleanupModel.Instance.CanShowPackInStore())
            DeleteBundle();
        Refresh();
    }

    public void Init(TableShop cfg, TableBundle bundle, GameGiftTitleItem gameGiftTitleItem,
        ShowArea area = ShowArea.none, string src = "")
    {
        _showArea = area;
        openSrc = src;
        if (cfg == null)
            return;
        _cfg = cfg;
        _bundle = bundle;
        _gameGiftTitleItem = gameGiftTitleItem;
        Refresh();
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishBoostsExpose);

    }

    public void DeleteBundle()
    {
        Destroy(gameObject);
        if (TaskAssistPackModel.Instance.GetCanBuyCount() > 0)
            CommonUtils.DelayedCall(0.1f, DeLayRefresh);
        else
        {
            _gameGiftTitleItem.gameObject.SetActive(false);
            UIStoreController.Instance?.ReBuild();
        }
    }

    private void DeLayRefresh()
    {
        _gameGiftTitleItem.ResetPageScrollView();
    }

    public void Refresh()
    {
        CommonUtils.DestroyAllChildren(_rewardParent);
        _priceText.text = (StoreModel.Instance.GetPrice(Cfg.id));

        transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(Cfg.id));
        
        _tagText.SetText((1 - Cfg.showDiscount) * 100 + "% \n" +
                         LocalizationManager.Instance.GetLocalizedString("ui_common_off"));
        string txt = LocalizationManager.Instance.GetLocalizedString("UI_info_text11");
        _tagText.transform.parent.gameObject.SetActive(false);
        _tagText.SetText(string.Format(txt, GarageCleanupModel.Instance.GetCleanupPackLeft()));
        _contentText.SetTerm(_bundle.name);
        List<int> itemIds = new List<int>(_bundle.bundleItemList);
        List<int> itemCounts = new List<int>(_bundle.bundleItemCountList);

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
                MergeInfoView.Instance.OpenMergeInfo(itemIds[index],_isShowProbability:true);
                UIPopupMergeInformationController controller =
                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                if (controller != null)
                    controller.SetResourcesActive(false);
            });
        }
    }

    protected void OnClickBuy()
    {
   
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishBoostsPurchase);

        int buyID = Cfg.id;
        StoreModel.Instance.nowItemPos = this.transform.position;
        StoreModel.Instance.Purchase(buyID, openSrc, _bundle.shopItemId);
    }
}