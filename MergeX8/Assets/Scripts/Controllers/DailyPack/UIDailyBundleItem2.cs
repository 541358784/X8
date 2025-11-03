using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyBundleItem2 : MonoBehaviour
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
    private StorageDailyPackItem _pack;
    private int MaxItemCount = 5;
    protected  void Awake()
    {
        _buyButton = transform.Find("ButtonBuy").GetComponent<Button>();
        _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
        _contentText = transform.Find("BundleText").GetComponent<LocalizeTextMeshProUGUI>();
        for (int i = 1; i <= MaxItemCount; i++)
        {
            var item = transform.Find("RewardGroup/Item" + i);
            item.gameObject.SetActive(false);
            _lstBoosters.Add(item);
        }
        // for (int i = 1; i <= 3; i++)
        // {
        //     var item = transform.Find("RewardGroup/Item" + i);
        //     item.gameObject.SetActive(false);
        //     _lstBoosters.Add(item);
        // }      
        // for (int i = 1; i <= 3; i++)
        // {
        //     var item = transform.Find("RewardGroup/Item4/" + i);
        //     item.gameObject.SetActive(false);
        //     _lstBoosters2.Add(item);
        // }
        _tagGroup = transform.Find("TagGroup");
        _tagText = transform.Find("TagGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEventListener(EventEnum.Daily_Pack_REFRESH, UpDateUI);
        CommonUtils.SetShieldButUnEnable(_buyButton.gameObject);
    }

    private void Start()
    {
        _buyButton.onClick.AddListener(OnPurchaseClick);
    }

    private void UpDateUI(BaseEvent obj)
    {
        if (_shop != null)
            _buyButton.interactable = DailyPackModel.Instance.IsCanBuyItem(_shop.id);

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
    
    public void Init(StorageDailyPackItem pack, DailyPackInfo packInfo, string source)
    {
        
        this.source = source;
        _pack = pack;
        _shop = GlobalConfigManager.Instance.GetTableShopByID(pack.Shopid);
        var maxCount = Math.Min(MaxItemCount, pack.Id.Count);
        for (int i = 0; i < maxCount; i++)
        {
            InitItem(_lstBoosters[i], pack.Id[i], pack.Count[i]);
        }

        _tagGroup.gameObject.SetActive(packInfo.Best_deal==2);
        _priceText.text = StoreModel.Instance.GetPrice(_shop.id);
        _tagText.SetText( LocalizationManager.Instance.GetLocalizedString("UI_shop_tag_bestvlaue"));
        _contentText.SetTerm(packInfo.Name);
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
        EventDispatcher.Instance.RemoveEventListener(EventEnum.Daily_Pack_REFRESH, UpDateUI);
    }
    
    private void OnPurchaseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StoreModel.Instance.Purchase(_shop.id,source);
      
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPackBuy, source);
    }
}