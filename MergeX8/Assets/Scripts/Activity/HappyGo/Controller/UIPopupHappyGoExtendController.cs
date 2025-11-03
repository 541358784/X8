
using System;
using Deco.Item;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupHappyGoExtendController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;

    private Button _buyButton;
    private Text _buyText;
    private Button _buttonClose;
    private Transform _rewardItem;
    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _buyButton = GetItem<Button>("Root/BuyButton");
        _buyText = GetItem<Text>("Root/BuyButton/Text");
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _rewardItem = transform.Find("Root/RewardGroup/Item");
        _rewardItem.gameObject.SetActive(false);
        EventDispatcher.Instance.AddEventListener(EventEnum.HAPPYGO_EXTEND_PURCHASE_SUCCESS,PurchaseSuccess);
    }



    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        InvokeRepeating("UpdateTimeText",0,1);
        UpdateTimeText();

        var config = HappyGoModel.Instance.HappyGoConfig;
        _buyText.text = StoreModel.Instance.GetPrice(config.extendBuyProductId);
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buyButton.onClick.AddListener(OnBtnBuy);
        var bundle=HappyGoModel.Instance.GetTableHgBundle(config.extendBuyProductId);
        for (int i = 0; i < bundle.bundleItemList.Length; i++)
        {
            var item= Instantiate(_rewardItem, _rewardItem.parent);
            item.gameObject.SetActive(true);
            InitItem(item,bundle.bundleItemList[i],bundle.bundleItemCountList[i]);
        }
    }

    private void PurchaseSuccess(BaseEvent obj)
    {
        AnimCloseWindow();
    }
    private void OnBtnBuy()
    {
        StoreModel.Instance.Purchase(HappyGoModel.Instance.HappyGoConfig.extendBuyProductId);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdExtend);

    }

   

    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void UpdateTimeText()
    {
        if (HappyGoModel.Instance.GetActivityExtendBuyWaitLeftTime() <= 0)
        {
            AnimCloseWindow();
        }
        else
        {
            _timeText.SetText(HappyGoModel.Instance.GetActivityExtendBuyWaitLeftTimeString());
        }
    }
    private void InitItem(Transform item, int itemID, int ItemCount)
    {
        LocalizeTextMeshProUGUI text =  item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        text.SetText(ItemCount.ToString());
        
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
              
            }
        }

        item.gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HAPPYGO_EXTEND_PURCHASE_SUCCESS,PurchaseSuccess);
    }
}
