using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreDolphinItem : UIStoreBaseItem
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

    
    protected override void PrivateAwake()
    {
        _btnBuy = transform.Find("BuyButton").GetComponent<Button>();
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();
 
        _tagGroup = transform.Find("TagGroup");
        // _tagText = transform.Find("TagGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();

        _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
    }

    protected override void PrivateStart()
    {
       InvokeRepeating("RefreshTime",0,1);
    }

    public void RefreshTime()
    {
        if (!DolphinPackModel.Instance.CanShowInStore())
        {
            gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }
        _timeText.SetText(DolphinPackModel.Instance.GetActiveTime());
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
        for (int i = 1; i < 4; i++)
        {
          var   reward = transform.Find("RewardGroup/Icon"+i);
          InitItem(reward, cfg.bundleItemList[i-1], cfg.bundleItemCountList[i-1],
              cfg.bundleItemType[i-1]);
        }
     
        openSrc = "shop";
    }

    private void InitItem(Transform item, int itemID, int ItemCount, int ItemType)
    {
        if (ItemType == 1)
        {
            item.GetComponent<Image>().sprite =
                UserData.GetResourceIcon(itemID, UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
            if (itemConfig != null)
            {
                item.GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                ;
            }
            else
            {
                Debug.LogError("Get MergeItemConfig---null " + itemID);
            }
        }

        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText("x"+ItemCount.ToString());
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