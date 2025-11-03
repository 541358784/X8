using DragonPlus;
using DragonPlus.Config.LuckyGoldenEgg;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.LuckyGoldenEgg
{
    public class LuckyGoldenEggItem : MonoBehaviour
    {
        private Text _price;
        private LocalizeTextMeshProUGUI _numText;
        private Button _buttonBuy;
        private TableLuckyGoldenEggStoreConfig _config;

        private void Awake()
        {
            _buttonBuy = transform.Find("Button").GetComponent<Button>();
            _buttonBuy.onClick.AddListener(OnBtnBuy);
            _price = transform.Find("Button/Text").GetComponent<Text>();
            _numText = transform.Find("Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        private void OnBtnBuy()
        {
            StoreModel.Instance.Purchase(_config.ShopId);
        }

        public void Init(TableLuckyGoldenEggStoreConfig config)
        {
            _config = config;
            _price.text = StoreModel.Instance.GetPrice(_config.ShopId);
            _numText.SetText(config.RewardCount[0].ToString());     
            
            transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(_config.ShopId));

        }
    }
}