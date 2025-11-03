using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UINewDailNewyBundleItem : MonoBehaviour
{
    private TableShop _shop;
    private string source = null;
    private Button _buyButton;
    private Text _priceText;
    private Button _rvButton;

    private LocalizeTextMeshProUGUI _contentText;

    private List<Transform> _lstBoosters = new List<Transform>();
    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;
    private bool isHot = false;
    private StorageNewDailyPackItem _pack;
    private int MaxItemCount = 5;
    private LocalizeTextMeshProUGUI _vipText;
    protected  void Awake()
    {
        _buyButton = transform.Find("ButtonBuy").GetComponent<Button>();
        _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
        _rvButton = transform.Find("ButtonRv").GetComponent<Button>();
        _contentText = transform.Find("BundleText").GetComponent<LocalizeTextMeshProUGUI>();
        for (int i = 1; i <= MaxItemCount; i++)
        {
            var item = transform.Find("RewardGroup/Item" + i);
            item.gameObject.SetActive(false);
            _lstBoosters.Add(item);
        }
        _tagGroup = transform.Find("TagGroup");
        _tagText = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEventListener(EventEnum.NewDaily_Pack_REFRESH, UpDateUI);
        CommonUtils.SetShieldButUnEnable(_buyButton.gameObject);
        
        _vipText = transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    private void UpDateUI(BaseEvent obj)
    {
        if (_shop != null)
        {
            _buyButton.gameObject.SetActive(true);
            _buyButton.interactable = NewDailyPackModel.Instance.IsCanBuyItem(_shop.id);
            _rvButton.gameObject.SetActive(false);
        }
        else
        {
            _rvButton.gameObject.SetActive(true);
            if (!NewDailyPackModel.Instance.IsCanBuyItem(_pack.Shopid))
                _rvButton.interactable = false;
            // _rvButton.interactable = NewDailyPackModel.Instance.IsCanBuyItem(_pack.Shopid);
            _buyButton.gameObject.SetActive(false);
        }
        
        if (obj == null)
            return;

        if (obj.datas == null || obj.datas.Length == 0)
            return;

        int shopId = (int) obj.datas[0];
        if (_shop == null || _shop.id != shopId)
            return;

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPackBuySuccess, source,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString(),"Daily");
    }

    private List<NewDailyPackageExtraRewardLabel> ExtraRewardLabel = new List<NewDailyPackageExtraRewardLabel>();
    public void Init(StorageNewDailyPackItem pack, TableNewDailyPackPackageConfig packInfo, string source)
    {
        
        this.source = source;
        _pack = pack;
        _shop = GlobalConfigManager.Instance.GetTableShopByID(pack.Shopid);
        
        
        if (_shop != null)
        {
            _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(_shop.price));
            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(OnPurchaseClick);
            _priceText.text = StoreModel.Instance.GetPrice(_shop.id);
            transform.Find("Vip").gameObject.SetActive(true);
        }
        else
        {
            _rvButton.onClick.RemoveAllListeners();
            if (NewDailyPackModel.Instance.IsCanBuyItem(_pack.Shopid))
            {
                UIAdRewardButton.Create(ADConstDefine.R_NEW_DAILY_PACK, UIAdRewardButton.ButtonStyle.Disable,
                    _rvButton.gameObject,
                    (s) =>
                    {
                        if (s)
                        {
                            NewDailyPackModel.Instance.RvSuccess(_pack.PackInfoId, "shop");
                        }
                    }, false, null, () => { });
            }
            transform.Find("Vip").gameObject.SetActive(false);
        }
        
        
        
        var maxCount = Math.Min(MaxItemCount, pack.Id.Count);
        for (int i = 0; i < maxCount; i++)
        {
            InitItem(_lstBoosters[i], pack.Id[i], pack.Count[i]);
        }
        _tagGroup.gameObject.SetActive(packInfo.best_deal==2);
        // _tagText.SetText( LocalizationManager.Instance.GetLocalizedString("UI_shop_tag_bestvlaue"));
        _tagText.SetText(packInfo.labelNum);
        _contentText.SetTerm(packInfo.name);
        UpDateUI(null);
        for (var i = 1; i <= 2; i++)
        {
            var label = transform.Find("ExtraReward"+i).gameObject.AddComponent<NewDailyPackageExtraRewardLabel>();
            label.Init(pack.PackInfoId,i);
            if (label.gameObject.activeSelf)
                _tagGroup.gameObject.SetActive(false);
            ExtraRewardLabel.Add(label);
        }
    }

    private void InitItem(Transform item,int itemID,int ItemCount)
    {
        if (UserData.Instance.IsResource(itemID))
        {
            item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID,UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
            if (itemConfig != null)
            {
                item.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                ;
            }
            else
            {
                Debug.LogError("Get MergeItemConfig---null " + itemID);
            }
        }

        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
        bool isRes = UserData.Instance.IsResource(itemID);
        var infoBtn = item.transform.Find("HelpButton");
        if (infoBtn != null)
        {
            infoBtn.gameObject.SetActive(!isRes);
            if (!isRes)
            {
                var itemButton = CommonUtils.GetComponent<Button>(infoBtn.gameObject);
                itemButton.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemID, null,_isShowProbability:true);
                    UIPopupMergeInformationController controller =
                        UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                    if (controller != null)
                        controller.SetResourcesActive(false);
                });
            }
        }
    }
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NewDaily_Pack_REFRESH, UpDateUI);
    }
    
    private void OnPurchaseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StoreModel.Instance.Purchase(_shop.id,source);
      
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPackBuy, source);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventNewDailyDealsPurchase,data1:source,data2:_pack.PackInfoId.ToString());
    }
}