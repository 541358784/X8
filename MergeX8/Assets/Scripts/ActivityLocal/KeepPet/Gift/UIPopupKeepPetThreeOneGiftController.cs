
using System;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKeepPetThreeOneGiftController : UIWindowController
{
    public class ThreeOneStoreItem : MonoBehaviour
    {
        private Button _buyBtn;
        private Text _buyBtnText;
        private List<CommonRewardItem> _items = new List<CommonRewardItem>();
        private Transform DefaultItem;
        private KeepPetThreeOneStoreConfig _threeOneConfig;

        public void Init(KeepPetThreeOneStoreConfig config)
        {
            DefaultItem = transform.Find("Item");
            DefaultItem.gameObject.SetActive(false);
            _threeOneConfig = config;
            var rewards = CommonUtils.FormatReward(_threeOneConfig.RewardId, _threeOneConfig.RewardCount);
            foreach (var reward in rewards)
            {
                var item = Instantiate(DefaultItem, DefaultItem.parent).gameObject.AddComponent<CommonRewardItem>();
                item.gameObject.SetActive(true);
                item.Init(reward);
                _items.Add(item);
            }

            _buyBtn = transform.Find("Button").GetComponent<Button>();
            _buyBtn.onClick.AddListener(() =>
            {
                StoreModel.Instance.Purchase(_threeOneConfig.ShopId);
            });
            _buyBtnText = transform.Find("Button/Text").GetComponent<Text>();
            _buyBtnText.text = StoreModel.Instance.GetPrice(_threeOneConfig.ShopId);
        }
    }
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _timeText;
    private Button _buyBtn;
    private Text _buyBtnText;
    private LocalizeTextMeshProUGUI _buyBtnDescribeText;
    // private Transform DefaultStoreItem;
    private List<ThreeOneStoreItem> _items = new List<ThreeOneStoreItem>();
    
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnBtnClose);
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _buyBtn = GetItem<Button>("Root/Button");
        _buyBtn.onClick.AddListener(OnBtnBuy);
        _buyBtnText = GetItem<Text>("Root/Button/Text");
        _buyBtnDescribeText = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text1");
        _buyBtnDescribeText.SetTermFormats(KeepPetModel.Instance.GlobalConfig.Discount.ToString());
        InvokeRepeating("RefreshTime",0,1);
        EventDispatcher.Instance.AddEventListener(EventEnum.KEEPPET_GIFT_PURCHASE,OnPurchase);
        // DefaultStoreItem = transform.Find("1");
        // DefaultStoreItem.gameObject.SetActive(false);
        Init();
    }

    private void OnPurchase(BaseEvent obj)
    {
        AnimCloseWindow();
    }

    private void OnBtnBuy()
    {
        StoreModel.Instance.Purchase(KeepPetModel.Instance.GlobalConfig.ThreeOneShopId);
    }

    public void Init()
    {
        var i = 0;
        foreach (var config in KeepPetModel.Instance.ThreeOneStoreConfig)
        {
            i++;
            // var item = Instantiate(DefaultStoreItem, DefaultStoreItem.parent).gameObject
            //     .AddComponent<ThreeOneStoreItem>();
            var item = transform.Find("Root/Gift" + i).gameObject.AddComponent<ThreeOneStoreItem>();
            item.gameObject.SetActive(true);
            item.Init(config);
            _items.Add(item);
        }
        _buyBtnText.text = StoreModel.Instance.GetPrice(KeepPetModel.Instance.GlobalConfig.ThreeOneShopId);
    }

    public void RefreshTime()
    {
        _timeText.SetText(KeepPetModel.Instance.GetGiftRestTimeString());
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.KEEPPET_GIFT_PURCHASE,OnPurchase);
    }
}
