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

public class UISealPackController : UIWindowController
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
        
        _rewardIcon = GetItem<Image>("Root/RewardGroup/Icon");
        _rewardText = GetItem<LocalizeTextMeshProUGUI>("Root/RewardGroup/Text");
        
        _tableBundle = SealPackModel.Instance.BundleData;

        _cooldown = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        
        _helpBtn = GetItem<Button>("Root/RewardGroup/HelpButton");
        _helpBtn.onClick.AddListener(() =>
        {
            MergeInfoView.Instance.OpenMergeInfo(_tableBundle.bundleItemList[0], isShowGetResource:false);
        });
        
        InvokeRepeating("UpdateUI", 0, 1);
    }

    private void Start()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSealPackagePop);
        
        if(_tableBundle == null)
            return;
        
        _rewardIcon.sprite = UserData.GetResourceIcon(_tableBundle.bundleItemList[0]);
        _rewardText.SetText(_tableBundle.bundleItemCountList[0].ToString());
        _payPrice.text = StoreModel.Instance.GetPrice(_tableBundle.shopItemId);
    }

    private void OnBtnPay()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSealPackagePurchase, "pack");
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
            if (SealPackModel.Instance.storageSealPack.LastPopUpTime == 0)
            {
                SealPackModel.Instance.storageSealPack.LastPopUpTime = (long)APIManager.Instance.GetServerTime();

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
        _cooldown.SetText(SealPackModel.Instance.GetActiveTime());
    }

    public static string constPlaceId = "sealPack";
    public static bool CanShowUI()
    {
        if (SealPackModel.Instance.storageSealPack.IsFinish)
            return false;

        if (SealPackModel.Instance.storageSealPack.PopTimes > 7)
            return false;

        if (SealPackModel.Instance.IsActiveTimeEnd())
            return false;
        
        if (!MainOrderManager.Instance.IsSealActiveTask() || MainOrderManager.Instance.IsFinishSealTask())
            return false;

        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
            return false;

        SealPackModel.Instance.storageSealPack.PopTimes++;
        if (SealPackModel.Instance.storageSealPack.FinishTime == 0)
        {
            SealPackModel.Instance.storageSealPack.FinishTime = CommonUtils.GetTomorrow((long)APIManager.Instance.GetServerTime(), 7, 0);
        }
        
        UIManager.Instance.OpenUI(UINameConst.UISealPack);
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,CommonUtils.GetTimeStamp());

        return true;
    }
}