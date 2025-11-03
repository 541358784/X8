using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIIcebreakingPackItem : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _titleText;

    // private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _tagText;
    private Transform _tagGroup;
    private Text _priceText;
    private IceBreakPack _pack;
    private TableShop _shop;
    private Button _buyButton;
    private string source = null;
    private List<Transform> _rewardItems;
    
    private void Awake()
    {
        _titleText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        // _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _tagGroup = transform.Find("TagGroup");
        _tagText = transform.Find("TagGroup/Image/Text").GetComponent<LocalizeTextMeshProUGUI>();

        // _tagGroup.gameObject.SetActive(false);
        _buyButton = transform.Find("BuyButton").GetComponent<Button>();
        _buyButton.onClick.AddListener(OnPurchaseClick);
        
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();
        _rewardItems = new List<Transform>();
        for (int i = 1; i < 5; i++)
        {
            _rewardItems.Add(transform.Find("RewardGroup/Item"+i));
        }

        EventDispatcher.Instance.AddEventListener(EventEnum.IceBreak_Pack_REFRESH, UpDateUI);
    }

    protected void Start()
    {
        CommonUtils.SetShieldButUnEnable(_buyButton.gameObject);

    }

    private void UpDateUI(BaseEvent obj)
    {
        if (_pack != null)
            _buyButton.interactable = IcebreakingPackModel.Instance.IsCanBuyItem(_pack.ShopItem);

        if (obj == null)
            return;

        if (obj.datas == null || obj.datas.Length == 0)
            return;

        int shopId = (int) obj.datas[0];
        if (_shop == null || _shop.id != shopId)
            return;

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPackBuySuccess, source,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "IceBreak");
    }

    public void Init(IceBreakPack pack, string source)
    {
        this.source = source;
        _pack = pack;
        _shop = GlobalConfigManager.Instance.GetTableShopByID(pack.ShopItem);
        _priceText.text = StoreModel.Instance.GetPrice(_shop.id);
        _tagText.SetText(pack.Discount + "% \n" + LocalizationManager.Instance.GetLocalizedString("ui_common_off"));
        _titleText.SetTerm(_pack.Description);
        for (int i = 0; i < _rewardItems.Count; i++)
        {
            var item =_rewardItems[i];
            if (i < pack.Content.Count)
            {
              
                item.gameObject.SetActive(true);
                if (UserData.Instance.IsResource(pack.Content[i]))
                {
                    item.transform.Find("Icon").GetComponent<Image>().sprite =
                        UserData.GetResourceIcon(pack.Content[i], UserData.ResourceSubType.Reward);
                    
                    Button _infoButton = item.transform.Find("TipsBtn").GetComponent<Button>();
                    _infoButton.gameObject.SetActive(false);
                }
                else
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(pack.Content[i]);
                    item.transform.Find("Icon").GetComponent<Image>().sprite =
                        MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                    
                    Button _infoButton = item.transform.Find("TipsBtn").GetComponent<Button>();
                    _infoButton.gameObject.SetActive(true);
                    _infoButton.onClick.AddListener(() =>
                    {
                        MergeInfoView.Instance.OpenMergeInfo(itemConfig, isShowGetResource:false,_isShowProbability:true);
                    });
                }

                item.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(pack.Count[i].ToString());
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
        UpDateUI(null);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.IceBreak_Pack_REFRESH, UpDateUI);
    }

    private void OnPurchaseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StoreModel.Instance.Purchase(_shop.id);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPackBuy, source,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "IceBreak");
    }
}