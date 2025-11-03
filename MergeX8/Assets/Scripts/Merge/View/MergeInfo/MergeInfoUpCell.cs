using System;
using System.Collections.Generic;
using Activity.BalloonRacing;
using Activity.RabbitRacing.Dynamic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class MergeInfoUpCell : MonoBehaviour
{
    private GameObject _normal;
    private GameObject _current;
    private GameObject _question;
    private LocalizeTextMeshProUGUI _text;
    private Image _image;
    private Button _button;
    private LocalizeTextMeshProUGUI _gemText;
    private TableMergeItem _mergeItem;
    private StorageStoreItem _storeItem;
    private void Awake()
    {
        _normal = transform.Find("Normal").gameObject;
        _current = transform.Find("Current").gameObject;
        _question = transform.Find("QuestionMark").gameObject;
        _image = transform.Find("Icon").GetComponent<Image>();
        _text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _gemText = transform.Find("Button/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        _button = transform.Find("Button").GetComponent<Button>();
        _button.onClick.AddListener(OnClickBuy);
        EventDispatcher.Instance.AddEventListener(EventEnum.FLASH_SALE_REFRESH,FlashRefresh);
    }

 
    public void Init(TableMergeItem mergeItem, bool isCurrent)
    {
        _mergeItem = mergeItem;
        _normal.gameObject.SetActive(!isCurrent);
        _current.gameObject.SetActive(isCurrent);
        
        bool isUnlock = MergeManager.Instance.IsShowedItemId(mergeItem.id);
        isUnlock = isCurrent ? true : isUnlock;
        
        _question.gameObject.SetActive(!isUnlock);
        _image.gameObject.SetActive(isUnlock);
        
        _text.SetText(mergeItem.level.ToString());
        if(!isUnlock)
            return;
        
        _image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image);
      
        RefreshBuyInfo();
    }
    private void FlashRefresh(BaseEvent obj)
    {
        RefreshBuyInfo();
    }

    public void RefreshBuyInfo()
    {
        _storeItem= StoreModel.Instance.GetStorageStoreItem(_mergeItem.id);
        if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.FlashSale) && _storeItem != null && _storeItem.BuyCount<_storeItem.CanBuyCount)
        {
            _button.gameObject.SetActive(true);
            _gemText.SetText(StoreModel.Instance.GetFlashSaleItemPrice(_storeItem).ToString());
        }
        else
        {
            _button.gameObject.SetActive(false);
        }
    }
    private  void OnClickBuy()
    {
        if (_storeItem == null)
            return;
        var price = StoreModel.Instance.GetFlashSaleItemPrice(_storeItem);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventShopFlashsaleBuy, _storeItem.ItemId.ToString(),"2");
        if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
        {
            var reason = new GameBIManager.ItemChangeReasonArgs
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopFlashSale,
                data1 = _storeItem.ItemId.ToString(),
                data2 = "2"
            };
            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, price, reason);
            AddRewardItem();
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventShopFlashsaleGet,
                _storeItem.ItemId.ToString(),"2");
            
            BalloonRacingModel.Instance.AddFlashBuyScore(price, _button.transform.position);
            RabbitRacingModel.Instance.AddFlashBuyScore(price, _button.transform.position);
        }
        else
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_info_text14"),
                HasCloseButton = true,
            });
        }
          
    }

    private void AddRewardItem()
    {

        StoreModel.Instance.BuyFlashSaleItem(_storeItem);
        RefreshBuyInfo();
        var mergeItem = MergeManager.Instance.GetEmptyItem();

        mergeItem.Id = _storeItem.ItemId;
        mergeItem.State = 1;
        MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);

        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeShopFlashSale,
            itemAId = mergeItem.Id,
            ItemALevel = _mergeItem.level,
            isChange = true,
         
        });
        Vector3 endPos = Vector3.zero;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            endPos = MergeMainController.Instance.rewardBtnPos;
        }
        else
        {
            endPos = UIHomeMainController.mainController.MainPlayTransform.position;
        }

        FlyGameObjectManager.Instance.FlyObject(mergeItem.Id, transform.position, endPos, 1.2f, 2.0f, 1f,
            () => { EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH); });
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FLASH_SALE_REFRESH,FlashRefresh);

    }
}