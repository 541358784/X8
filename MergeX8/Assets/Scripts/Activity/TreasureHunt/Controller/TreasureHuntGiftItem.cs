using System;
using Activity.TreasureHuntModel;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
public class TreasureHuntGiftItem : MonoBehaviour
{
    private Text _price;
    private LocalizeTextMeshProUGUI _numText;
    private Button _buttonBuy;
    private TreasureHuntStoreConfig _config;
    private void Awake()
    {
        _buttonBuy=transform.Find("Button").GetComponent<Button>();
        _buttonBuy.onClick.AddListener(OnBtnBuy);
        _price=transform.Find("Button/Text").GetComponent<Text>();
        _numText=transform.Find("Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    private void OnBtnBuy()
    {
        StoreModel.Instance.Purchase(_config.ShopId);
    }

    public void Init(TreasureHuntStoreConfig config)
    {
        _config = config;
        _price.text = StoreModel.Instance.GetPrice(_config.ShopId);
        _numText.SetText(config.RewardCount[0].ToString());
    }
}
