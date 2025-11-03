using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIMaterialPackItem : MonoBehaviour
{
    private Transform _rewardItem;
    private Transform _rewardParent;

    private LocalizeTextMeshProUGUI _titleText;

    // private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _tagText;
    private Transform _tagGroup;
    private Text _priceText;
    private TableShop _shop;
    private Button _buyButton;
    private string source = null;
    private StorageDailyPackItem _pack;
    private void Awake()
    {
        _titleText = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        // _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _tagGroup = transform.Find("TagGroup");
        _tagText = transform.Find("TagGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();

        // _tagGroup.gameObject.SetActive(false);
        _buyButton = transform.Find("BuyButton").GetComponent<Button>();
        _buyButton.onClick.AddListener(OnPurchaseClick);
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();


        EventDispatcher.Instance.AddEventListener(EventEnum.Daily_Pack_REFRESH, UpDateUI);
    }

    protected void Start()
    {
        CommonUtils.SetShieldButUnEnable(_buyButton.gameObject);
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

    public void Init(StorageDailyPackItem pack, string name, string source)
    {
        _rewardParent = transform.Find("RewardGroup");
        this.source = source;
        _pack = pack;
        _shop = GlobalConfigManager.Instance.GetTableShopByID(pack.Shopid);
        _priceText.text = StoreModel.Instance.GetPrice(_shop.id);
        _tagText.SetText(pack.Discount + "% \n" + LocalizationManager.Instance.GetLocalizedString("ui_common_off"));
        _titleText.SetTerm(name);

        _rewardItem = _rewardParent.Find("Item1");
        _rewardItem.gameObject.SetActive(false);
        if (pack != null)
        {
            for (int i = 0; i < pack.Id.Count; i++)
            {
                CreateRewardItem(pack.Id[i], pack.Count[i]);
            }
        }

        UpDateUI(null);
    }

    private void CreateRewardItem(int resID, int resCount)
    {
        var item = Instantiate(_rewardItem, _rewardItem.parent);
        item.gameObject.SetActive(true);
        if (UserData.Instance.IsResource(resID))
        {
            item.transform.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(resID, UserData.ResourceSubType.Reward);
            item.transform.Find("TipsBtn").gameObject.SetActive(false);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(resID);
            item.transform.Find("Icon").GetComponent<Image>().sprite =
                MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            item.transform.Find("TipsBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                MergeInfoView.Instance.OpenMergeInfo(itemConfig,_isShowProbability:true);
                UIPopupMergeInformationController controller =
                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                if (controller != null)
                    controller.SetResourcesActive(false);
            }); 
        }
        item.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(resCount.ToString());
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.Daily_Pack_REFRESH, UpDateUI);
    }

    private void OnPurchaseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StoreModel.Instance.Purchase(_shop.id);
        var extras2 = new Dictionary<string, string>();
        extras2.Clear();
        extras2.Add("PackID", _pack?.PackId.ToString());
        for (int i = 0; i < _pack.Id.Count; i++)
        {
            if(!extras2.ContainsKey(_pack.Id[i].ToString()))
                extras2.Add(_pack.Id[i].ToString(),_pack.Count[i].ToString());
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDailyDealsPurchase, source,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString(),"Daily");
    }
}