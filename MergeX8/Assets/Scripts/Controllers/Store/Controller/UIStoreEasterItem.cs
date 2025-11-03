using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreEasterItem : UIStoreBaseItem
{
    public override UIStoreItemType ItemType { get; } = UIStoreItemType.Bundle;

    protected override Button BuyButton
    {
        get { return _btnBuy; }
    }

    private Button _btnBuy;

    private Text _priceText;
    
    private LocalizeTextMeshProUGUI _contentText;

    private Transform _tagGroup;
    // private LocalizeTextMeshProUGUI _tagText;

    private LocalizeTextMeshProUGUI _timeText;

    private Image _rewardIcon;
    private LocalizeTextMeshProUGUI _rewardText;
    
    protected override void PrivateAwake()
    {
        _btnBuy = transform.Find("BuyButton").GetComponent<Button>();
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();
 
        _tagGroup = transform.Find("TagGroup");
        // _tagText = transform.Find("TagGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();

        _rewardIcon = transform.Find("RewardGroup/Icon").GetComponent<Image>();
        _rewardText = transform.Find("RewardGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
    }

    protected override void PrivateStart()
    {
       InvokeRepeating("RefreshTime",0,1);
    }

    public void RefreshTime()
    {
        if (!EasterPackModel.Instance.CanShowInStore())
        {
            gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }
        _timeText.SetText(EasterPackModel.Instance.GetActiveTime());
    }
    public override void Refresh()
    {
        TableBundle cfg = GlobalConfigManager.Instance.GetTableBundleByShopID(ID);
        if (null == cfg)
        {
            DebugUtil.LogError("shopConfig is Null " + ID);
            return;
        }
  
        _priceText.text = (StoreModel.Instance.GetPrice(Cfg.id));
        InitItem(_rewardIcon.transform.parent, cfg.bundleItemList[0], cfg.bundleItemCountList[0],
            cfg.bundleItemType[0]);
        openSrc = "shop";
    }

    private void InitItem(Transform item, int itemID, int ItemCount, int ItemType)
    {
        if (ItemType == 1)
        {
            item.Find("Icon").GetComponent<Image>().sprite =
                UserData.GetResourceIcon(itemID, UserData.ResourceSubType.Reward);
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
        var infoBtn = item.transform.parent.Find("HelpButton");
        if (infoBtn != null)
        {
            infoBtn.gameObject.SetActive(!isRes);
            if (!isRes)
            {
                var itemButton = CommonUtils.GetComponent<Button>(infoBtn.gameObject);
                itemButton.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemID, isShowGetResource:false,_isShowProbability:true);
                });
            }
        }
    }
}