using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Vip.Model;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKapibalaGiftBagController:UIWindowController
{
    public static UIPopupKapibalaGiftBagController Instance;
    public static UIPopupKapibalaGiftBagController Open(StorageKapibala storage,Action callback = null)
    {
        if (storage == null)
            return null;
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupKapibalaGiftBag, storage,callback) as
            UIPopupKapibalaGiftBagController;
        return Instance;
    }

    public StorageKapibala Storage;
    public Button CloseBtn;
    public LocalizeTextMeshProUGUI PercentText;
    public LocalizeTextMeshProUGUI TimeText;
    public List<ProductView> ProductList = new List<ProductView>();
    private Action Callback;
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        PercentText = transform.Find("Root/TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);
        for (var i = 1; transform.Find("Root/Gift"+i); i++)
        {
            ProductList.Add(transform.Find("Root/Gift"+i).gameObject.AddComponent<ProductView>());
        }
    }

    private void OnDestroy()
    {
        if (TMatchSystem.LevelController != null)
            TMatchSystem.LevelController.LevelStateData.pause = false;
        Callback?.Invoke();
    }

    public void UpdateTime()
    {
        TimeText.SetText(Storage.GetLeftTimeText());
        if (Storage.IsTimeOut())
            AnimCloseWindow();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageKapibala;
        if (objs.Length > 1)
            Callback = objs[1] as Action;
        var productList = KapibalaModel.Instance.GiftBagConfig;
        for (var i = 0; i < ProductList.Count; i++)
        {
            if (productList.Count <= i)
            {
                ProductList[i].gameObject.SetActive(false);
            }
            else
            {
                ProductList[i].Init(productList[i],this);
            }
        }
        PercentText.SetText(productList.Last().TagText);
        if (TMatchSystem.LevelController != null)
            TMatchSystem.LevelController.LevelStateData.pause = true;
    }

    public class ProductView : MonoBehaviour
    {
        public Transform DefaultItem;
        public List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
        public Transform FinishGroup;
        public Button BuyBtn;
        public Text PriceText;
        public KapibalaGiftBagConfig ProductConfig;
        public UIPopupKapibalaGiftBagController Controller;
        
        private void Awake()
        {
            DefaultItem = transform.Find("Item");
            DefaultItem.gameObject.SetActive(false);
            FinishGroup = transform.Find("ButtonFinish");
            BuyBtn = transform.Find("Button").GetComponent<Button>();
            BuyBtn.onClick.AddListener(() =>
            {
                var buyState = false;
                if (!buyState)
                    StoreModel.Instance.Purchase(ProductConfig.ShopId);
            });
            PriceText = transform.Find("Button/Root/Text").GetComponent<Text>();
        }

        public void Init(KapibalaGiftBagConfig config,UIPopupKapibalaGiftBagController controller)
        {
            gameObject.SetActive(true);
            ProductConfig = config;
            Controller = controller;
            PriceText.text = StoreModel.Instance.GetPrice(ProductConfig.ShopId);
            var buyState = false;
            BuyBtn.gameObject.SetActive(!buyState);
            FinishGroup.gameObject.SetActive(buyState);
            foreach (var rewardItem in RewardItemList)
            {
                DestroyImmediate(rewardItem.gameObject);
            }
            RewardItemList.Clear();
            var rewards = CommonUtils.FormatReward(ProductConfig.RewardId, ProductConfig.RewardNum);
            foreach (var reward in rewards)
            {
                var rewardItem = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                    .AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(reward);
                RewardItemList.Add(rewardItem);
            }        
            
            transform.Find("Button/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(ProductConfig.ShopId));

        }
    }
}