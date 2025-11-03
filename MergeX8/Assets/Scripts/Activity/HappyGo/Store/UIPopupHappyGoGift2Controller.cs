
using DragonPlus;
using DragonPlus.Config.HappyGo;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupHappyGoGift2Controller : UIWindowController
{
    private Button _closeButton;
    private LocalizeTextMeshProUGUI _timeText;
    private Transform _rewardItem;
    private LocalizeTextMeshProUGUI _tag1;
    private LocalizeTextMeshProUGUI _tag2;
    private Button _buttonBuy;
    private Text _buttonBuyText;

    private HGVDBundle _bundle;
    public override void PrivateAwake()
    {
        _closeButton = GetItem<Button>("Root/CloseButton");
        _closeButton.onClick.AddListener(OnBtnClose);
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _rewardItem = transform.Find("Root/RewardGroup/Item");
        _rewardItem.gameObject.SetActive(false);
        _tag1 = GetItem<LocalizeTextMeshProUGUI>("Root/TagGroup/Text1");
        _tag2 = GetItem<LocalizeTextMeshProUGUI>("Root/TagGroup/Text2");
        _buttonBuy=GetItem<Button>("Root/BuyButton");
        _buttonBuy.onClick.AddListener(OnClickBuy);
        _buttonBuyText = GetItem<Text>("Root/BuyButton/Text");
        EventDispatcher.Instance.AddEventListener(EventEnum.HAPPYGOBUNDLE_PURCHASE_REFRESH, PurchseRefresh);

    }
   
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        StorageHGBundleItem bundle=objs[0] as StorageHGBundleItem;
        var bundleConfig= HappyGoModel.Instance.GetTableHgBundleById(bundle.PackId);
        Init(bundleConfig);
        InvokeRepeating("UpdateTimeText",0,1);
    }   
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HAPPYGOBUNDLE_PURCHASE_REFRESH, PurchseRefresh);
        CancelInvoke("UpdateTimeText");
    }

    private void PurchseRefresh(BaseEvent obj)
    {
        AnimCloseWindow();
    }

    public void UpdateTimeText()
    {
        if (_bundle == null)
            return;

        _timeText.SetText(HappyGoPackModel.Instance.GetPackLeftTimeStr());
    }
    public void Init(HGVDBundle bundle)
    {
        _bundle = bundle;
        _buttonBuyText.text=(StoreModel.Instance.GetPrice(bundle.shopItemId));
        for (int i = 0; i < bundle.bundleItemList.Length; i++)
        {
            var item= Instantiate(_rewardItem, _rewardItem.parent);
            item.gameObject.SetActive(true);
            InitItem(item,bundle.bundleItemList[i],bundle.bundleItemCountList[i]);
        }
        _tag1.SetText(_bundle.labelNum + "%");
    }
    
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
    protected void OnClickBuy()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdBuypackagePopup,_bundle.id.ToString());
        StoreModel.Instance.Purchase(_bundle.shopItemId, "openSrc");
    }
    
    private void InitItem(Transform item,int itemID,int ItemCount)
    {
        if (UserData.Instance.IsResource(itemID))
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

        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
        bool isRes = UserData.Instance.IsResource(itemID);
        var infoBtn = item.transform.Find("HelpButton");
        if (infoBtn != null)
        {
            infoBtn.gameObject.SetActive(!isRes);
            if (!isRes)
            {
                var itemButton = CommonUtils.GetComponent<Button>(infoBtn.gameObject);
                itemButton.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemID, null,_isShowProbability:true,isShowGetResource:false);
                    UIPopupMergeInformationController controller =
                        UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                    if (controller != null)
                        controller.SetResourcesActive(false);
                });
            }
        }
    }

}
