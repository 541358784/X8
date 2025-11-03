
using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBuyItem : MonoBehaviour
{ 
    private Button _buyBtn;
    private Text _buyBtnText;
    private LocalizeTextMeshProUGUI _tagText1;
    private Transform _rewardItem;

    private TableEnergyPackage _energyPackage;
    private void Awake()
    {
        _buyBtn =transform.Find("Button").GetComponent<Button>();
        _buyBtn.onClick.AddListener(OnBtnBuy);
        _buyBtnText = transform.Find("Button/Text").GetComponent<Text>();        
        _tagText1 = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardItem = transform.Find("Item");
        _rewardItem.gameObject.SetActive(false);
    }

    public void Init(TableEnergyPackage energyPackage)
    {
        _energyPackage = energyPackage;
        _buyBtnText.text = StoreModel.Instance.GetPrice(energyPackage.shopId);
        _tagText1.SetText(energyPackage.discount);
        _tagText1.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(energyPackage.discount));
        for (int i = 0; i < energyPackage.itemId.Length; i++)
        {
            var trans = Instantiate(_rewardItem, _rewardItem.parent);
            trans.gameObject.SetActive(true);
            InitItem(trans, energyPackage.itemId[i], energyPackage.ItemNum[i]);
        }
        
        transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(energyPackage.shopId));
    }
    
    private void InitItem(Transform item, int itemID, int ItemCount)
    {
        if (UserData.Instance.IsResource(itemID))
        {
            item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID, UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
            if (itemConfig != null)
            {
                item.Find("Icon").GetComponent<Image>().sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else
            {
                Debug.LogError("Get MergeItemConfig---null " + itemID);
            }
        }

        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
    }
    private void OnBtnBuy()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyPackagePurchase,_energyPackage.shopId.ToString(),EnergyPackageModel.Instance.StorageEnergyPackage.PopTimes.ToString());

        StoreModel.Instance.Purchase(_energyPackage.shopId);
    }
}
