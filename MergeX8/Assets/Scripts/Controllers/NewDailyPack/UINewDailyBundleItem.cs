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

public class UINewDailyBundleItem : MonoBehaviour
{
    private TableShop _shop;
    private string source = null;
    private Button _buyButton;
    private Text _priceText;

    private LocalizeTextMeshProUGUI _contentText;

    private List<Transform> _lstBoosters = new List<Transform>();
    private List<Transform> _lstBoosters2 = new List<Transform>();
    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;
    private bool isHot = false;
    private StorageNewDailyPackItem _pack;
    protected  void Awake()
    {
        _buyButton = transform.Find("ButtonBuy").GetComponent<Button>();
        _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
        _contentText = transform.Find("BundleText").GetComponent<LocalizeTextMeshProUGUI>();
        for (int i = 1; i <= 3; i++)
        {
            var item = transform.Find("RewardGroup/Item" + i);
            item.gameObject.SetActive(false);
            _lstBoosters.Add(item);
        }      
        for (int i = 1; i <= 3; i++)
        {
            var item = transform.Find("RewardGroup/Item4/" + i);
            item.gameObject.SetActive(false);
            _lstBoosters2.Add(item);
        }
        _tagGroup = transform.Find("TagGroup");
        _tagText = transform.Find("TagGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEventListener(EventEnum.NewDaily_Pack_REFRESH, UpDateUI);
        CommonUtils.SetShieldButUnEnable(_buyButton.gameObject);
    }
    

    private void UpDateUI(BaseEvent obj)
    {
        if (_shop != null)
            _buyButton.interactable = NewDailyPackModel.Instance.IsCanBuyItem(_shop.id);
        else
        {
            _buyButton.interactable = NewDailyPackModel.Instance.IsCanBuyItem(_pack.Shopid);
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
    
    public void Init(StorageNewDailyPackItem pack, TableNewDailyPackPackageConfig packInfo, string source)
    {
        
        this.source = source;
        _pack = pack;
        _shop = GlobalConfigManager.Instance.GetTableShopByID(pack.Shopid);

        if (_shop != null)
        {
            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(OnPurchaseClick);
            transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(_shop.price));   
            _priceText.text = StoreModel.Instance.GetPrice(_shop.id);
            transform.Find("Vip").gameObject.SetActive(true);
        }
        else
        {
            _buyButton.onClick.RemoveAllListeners();
            if (NewDailyPackModel.Instance.IsCanBuyItem(_pack.Shopid))
            {
                UIAdRewardButton.Create(ADConstDefine.R_NEW_DAILY_PACK, UIAdRewardButton.ButtonStyle.Disable, _buyButton.gameObject,
                    (s) =>
                    {
                        if (s)
                        {
                            NewDailyPackModel.Instance.RvSuccess(_pack.PackInfoId,"shop");
                        }
                    }, false, null, () =>
                    {
                    });   
            }
            _priceText.text = LocalizationManager.Instance.GetLocalizedString("UI_RvReward_text");
            transform.Find("Vip").gameObject.SetActive(false);
        }

        if (pack.Id.Count <= 3)
        {
            for (int i = 0; i < pack.Id.Count; i++)
            {
                InitItem(_lstBoosters[i], pack.Id[i], pack.Count[i]);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                InitItem(_lstBoosters[i], pack.Id[i], pack.Count[i]);
            }
            _lstBoosters[2].gameObject.SetActive(false);
            _lstBoosters2[0].parent.gameObject.SetActive(true);
            for (int i = 0; i <pack.Id.Count-2; i++)
            {
                InitItem(_lstBoosters2[i], pack.Id[i+2], pack.Count[i+2]);
            }
        }
        _tagGroup.gameObject.SetActive(packInfo.best_deal==2);
        _tagText.SetText( LocalizationManager.Instance.GetLocalizedString("UI_shop_tag_bestvlaue"));
        _contentText.SetTerm(packInfo.name);
        UpDateUI(null);
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