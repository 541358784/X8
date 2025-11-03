using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIDolphinPackController : UIWindowController
{
    private Text _payPrice;
    private Button _payBtn;

    private Button _close;

    private Image _rewardIcon;
    private LocalizeTextMeshProUGUI _rewardText;

    private TableBundle _tableBundle;

    private Button _helpBtn;

    private LocalizeTextMeshProUGUI _cooldown;

    public override void PrivateAwake()
    {
        _payPrice = GetItem<Text>("Root/BuyButton/Text");
        _payBtn = GetItem<Button>("Root/BuyButton");
        _payBtn.onClick.AddListener(OnBtnPay);
        
        _close = GetItem<Button>("Root/CloseButton");
        _close.onClick.AddListener(OnBtnClose);
        
     
        _tableBundle = DolphinPackModel.Instance.BundleData;

        _cooldown = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        
      
     
        InvokeRepeating("UpdateUI", 0, 1);
    }

    private void Start()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDolphinPackagePop);
        
        if(_tableBundle == null)
            return;
        
        for (int i = 1; i < 4; i++)
        {
            _rewardIcon = GetItem<Image>("Root/RewardGroup/Icon"+i);
            _rewardText = GetItem<LocalizeTextMeshProUGUI>("Root/RewardGroup/Icon"+i+"/Text");
            if(i>1)
                _rewardIcon.sprite = UserData.GetResourceIcon(_tableBundle.bundleItemList[i-1]);
            _rewardText.SetText("X"+_tableBundle.bundleItemCountList[i-1].ToString());
            _payPrice.text = StoreModel.Instance.GetPrice(_tableBundle.shopItemId);
            _helpBtn = GetItem<Button>("Root/RewardGroup/Icon"+i+"/HelpButton");
            var itemConfig = GameConfigManager.Instance.GetItemConfig(_tableBundle.bundleItemList[i - 1]);
            if (itemConfig != null)
            {
                _helpBtn.gameObject.SetActive(true);
                _helpBtn.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemConfig, isShowGetResource:false);
                }); 
            }
               
        }
      
    }

    private void OnBtnPay()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDolphinPackagePurchase, "pack");
        StoreModel.Instance.Purchase(_tableBundle.shopItemId, "pack");
    }
    
    private void OnBtnClose()
    {
        OnCloseClicked(null);
    }
    
    private void OnCloseClicked(Action action)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            if (DolphinPackModel.Instance.storageDolphinPack.LastPopUpTime == 0)
            {
                DolphinPackModel.Instance.storageDolphinPack.LastPopUpTime = (long)APIManager.Instance.GetServerTime();

                ResData res = new ResData(_tableBundle.bundleItemList[0], 0);
                CoroutineManager.Instance.StartCoroutine(FlyGameObjectManager.Instance.ItemFlyLogic(res, null, UIHomeMainController.mainController.ShopTransform));
            }
            
            action?.Invoke();
            CloseWindowWithinUIMgr(true);
        }));
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnCloseClicked(null);
    }

    private void UpdateUI()
    {
        _cooldown.SetText(DolphinPackModel.Instance.GetActiveTime());
    }

    public static string constPlaceId = "DolphinPack";
    public static bool CanShowUI()
    {
        if (DolphinPackModel.Instance.storageDolphinPack.IsFinish)
            return false;

        if (DolphinPackModel.Instance.storageDolphinPack.PopTimes > 7)
            return false;

        if (DolphinPackModel.Instance.IsActiveTimeEnd())
            return false;
        
        if (!MainOrderManager.Instance.IsDolphinActiveTask() || MainOrderManager.Instance.IsFinishDolphinTask())
            return false;

        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
            return false;

        DolphinPackModel.Instance.storageDolphinPack.PopTimes++;
        if (DolphinPackModel.Instance.storageDolphinPack.FinishTime == 0)
        {
            DolphinPackModel.Instance.storageDolphinPack.FinishTime = CommonUtils.GetTomorrow((long)APIManager.Instance.GetServerTime(), 7, 0);
        }
        
        UIManager.Instance.OpenUI(UINameConst.UIDolphinPack);
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,CommonUtils.GetTimeStamp());

        return true;
    }
}