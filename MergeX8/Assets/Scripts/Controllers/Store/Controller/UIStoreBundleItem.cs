using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreBundleItem : UIStoreBaseItem
{
    public override UIStoreItemType ItemType { get; } = UIStoreItemType.Bundle;

    protected override Button BuyButton
    {
        get { return _btnBuy; }
    }

    private Button _btnBuy;

    private Text _priceText;
    
    private LocalizeTextMeshProUGUI _contentTextY;
    private LocalizeTextMeshProUGUI _contentTextP;

    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;

    private Transform _item1;
    private Transform _item2;
    private Transform _item3;

    private Transform _rewardGroup;
    private Transform _rewardGroup4;
    private Transform _rewardGrid;
    private Transform _bgYellow;    
    private Transform _bgPurple;
    private Image _icon;
    private bool isHot = false;
    protected override void PrivateAwake()
    {
        _btnBuy = transform.Find("BuyButton").GetComponent<Button>();
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();
        _bgYellow = transform.Find("BGYellow");
        _bgPurple = transform.Find("BGPurple");
        _contentTextY = _bgYellow.Find("BundleGroup/BundleText").GetComponent<LocalizeTextMeshProUGUI>();
        _contentTextP = _bgPurple.Find("BundleGroup/BundleText").GetComponent<LocalizeTextMeshProUGUI>();
        _item1=transform.Find("Item1");
        _item2=transform.Find("Item2");
        _item3=transform.Find("Item3");
        _item1.gameObject.SetActive(false);
        _item2.gameObject.SetActive(false);
        _item3.gameObject.SetActive(false);
        _rewardGroup = transform.Find("RewardGroup");
        _rewardGroup4 = transform.Find("MoreRewardGroup");
        _rewardGrid = transform.Find("RewardGroup/Grid");
        _rewardGrid.gameObject.SetActive(false);
        _tagGroup = transform.Find("TagGroup");
        _icon = transform.Find("DiamondImage").GetComponent<Image>();
        _tagText = transform.Find("TagGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    protected override void PrivateStart()
    {
       
    }

    public override void Refresh()
    {
        isHot = (!string.IsNullOrEmpty(Cfg.best_deal.ToString()))&& Cfg.best_deal == 2;
        _tagGroup.gameObject.SetActive(isHot);
        _bgPurple.gameObject.SetActive(isHot);
        _bgYellow.gameObject.SetActive(!isHot);
        CommonUtils.ClearChildGameObject(_rewardGrid);
        CommonUtils.ClearChildGameObject(_rewardGroup4);
        for (int i = 0; i < _rewardGroup.childCount; i++)
        {
            _rewardGroup.GetChild(i).gameObject.SetActive(false);
        }
        TableBundle cfg = GlobalConfigManager.Instance.GetTableBundleByShopID(ID);
        if (null == cfg)
        {
            DebugUtil.LogError("shopConfig is Null " + ID);
            return;
        }
        _icon.sprite= ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, cfg.icon);
        string iconBg = isHot ? "Shop_BG18_Purple_Item" : "Shop_BG18_Yellow_Item";
        var iconBgSprite=_rewardGrid.GetComponent<Image>();
        if (iconBgSprite!=null)
        {
            iconBgSprite.sprite=ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, iconBg);
        }

        _priceText.text = (StoreModel.Instance.GetPrice(Cfg.id));
        _contentTextY.SetTerm(cfg.name);
        _contentTextP.SetTerm(cfg.name);
          
        List<int> itemIds = new List<int>(cfg.bundleItemList);
        List<int> itemCounts = new List<int>(cfg.bundleItemCountList);
        List<int> itemIdsType = new List<int>(cfg.bundleItemType); // 1 items 2 merge
        if (itemIds.Count == 3)
        {
     
            _rewardGroup.gameObject.SetActive(true);
            _rewardGroup4.gameObject.SetActive(false);
            _rewardGrid.gameObject.SetActive(false);
            for (int i = 0; i < itemIds.Count; i++)
            {
               var item= Instantiate(_item1.gameObject, _rewardGroup);
               InitItem(item.transform, itemIds[i], itemCounts[i], itemIdsType[i]);
            }
        }
        else if(itemIds.Count==4)
        {
            _rewardGroup.gameObject.SetActive(false);
            _rewardGroup4.gameObject.SetActive(true);
            _rewardGrid.gameObject.SetActive(false);
    
            for (int i = 0; i < itemIds.Count; i++)
            {
                var item= Instantiate(_item2.gameObject, _rewardGroup4);
                InitItem(item.transform, itemIds[i], itemCounts[i], itemIdsType[i]);
            }
        }
        else
        {
            _rewardGroup.gameObject.SetActive(true);
            _rewardGroup4.gameObject.SetActive(false);
 
            for (int i = 0; i < itemIds.Count; i++)
            {
                if (i < 2)
                {
                    var item= Instantiate(_item2.gameObject, _rewardGroup);
                    item.transform.SetSiblingIndex(i);
                    InitItem(item.transform, itemIds[i], itemCounts[i], itemIdsType[i]);
                }
                else
                {
                    var item= Instantiate(_item3.gameObject, _rewardGrid);
                    InitItem(item.transform, itemIds[i], itemCounts[i], itemIdsType[i]);
                }
            }
            _rewardGrid.gameObject.SetActive(true);
        }
    }

    private void InitItem(Transform item,int itemID,int ItemCount,int ItemType)
    {
        if (ItemType == 1)
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

        string iconBg = isHot ? "Shop_BG18_Purple_Item" : "Shop_BG18_Yellow_Item";
        var iconBgSprite=item.gameObject.GetComponent<Image>();
        if (iconBgSprite!=null)
        {
            iconBgSprite.sprite=ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, iconBg);
        }
        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
        bool isRes = UserData.Instance.IsResource(itemID);
        var infoBtn = item.transform.Find("Button");
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
}