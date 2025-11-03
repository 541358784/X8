using System;
using Activity.TreasureHuntModel;
using DragonPlus;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;
public class ButterflyWorkShopGiftItem : MonoBehaviour
{
    private Text _price;
    private Button _buttonBuy;
    private ButterflyWorkShopPackageConfig _config;
    private Transform item;
    private void Awake()
    {
        _buttonBuy=transform.Find("Button").GetComponent<Button>();
        _buttonBuy.onClick.AddListener(OnBtnBuy);
        _price=transform.Find("Button/Text").GetComponent<Text>();
        item = transform.Find("Item");
        item.gameObject.SetActive(false);
    }

    private void OnBtnBuy()
    {
        StoreModel.Instance.Purchase(_config.ShopId);
    }

    public void Init(ButterflyWorkShopPackageConfig config)
    {
        _config = config;
        _price.text = StoreModel.Instance.GetPrice(_config.ShopId);
        for (int i = 0; i < config.RewardId.Count; i++)
        {
            var obj=Instantiate(item, item.parent);
            obj.gameObject.SetActive(true);
            InitItem(obj,config.RewardId[i],config.RewardCount[i]);
        }            
        
        transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(config.ShopId));

    }

    private void InitItem(Transform item, int itemID, int ItemCount)
    {
        LocalizeTextMeshProUGUI text = item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        text.SetText(ItemCount.ToString());

        if (UserData.Instance.IsResource(itemID))
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
            }
        }
    }
    
}
