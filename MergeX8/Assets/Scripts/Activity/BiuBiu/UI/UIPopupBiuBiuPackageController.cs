using DragonPlus;
using DragonPlus.Config.BiuBiu;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupBiuBiuPackageController : UIWindowController
{

    public static UIPopupBiuBiuPackageController Instance;
    public static UIPopupBiuBiuPackageController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupBiuBiuPackage) as UIPopupBiuBiuPackageController;
        return Instance;
    }
    
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _timeText;

    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnBtnCLose);
        var packageConfigs = BiuBiuModel.Instance.BiuBiuPackageConfigList;
        for (int i = 1; i <= 3; i++)
        {
            var item=  transform.Find("Root/Gift" + i).gameObject.AddComponent<BiuBiuPackageItem>();
            var config = packageConfigs.Count >= i ? packageConfigs[i - 1] : null;
            if (config != null)
                item.Init(config);
        }
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("RefreshTime",0,1);

    }
    public void RefreshTime()
    {
        _timeText.SetText(BiuBiuModel.Instance.GetActivityLeftTimeString());
    }
    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }
    public class BiuBiuPackageItem : MonoBehaviour
    {
        private Text _price;
        private Button _buttonBuy;
        private BiuBiuPackageConfig _config;
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
    
        public void Init(BiuBiuPackageConfig config)
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
}