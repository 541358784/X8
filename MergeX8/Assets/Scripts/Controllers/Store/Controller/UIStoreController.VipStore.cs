using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMBP;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Controller;
using Gameplay.UI.Store.Vip.Controller;
using Gameplay.UI.Store.Vip.Model;
using GamePool;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStoreController : UIWindow
{
    // private GameObject _bottomButtons;

    private Transform _redPoint;
    
    private Transform _vipContent;
    private ScrollRect _vipScrollRect;

    private List<VipStoreItem> _vipStoreItems = new List<VipStoreItem>();

    private LocalizeTextMeshProUGUI _currentStarText;
    private LocalizeTextMeshProUGUI _vipDaysText;
    private LocalizeTextMeshProUGUI _keepVipText;
    private LocalizeTextMeshProUGUI _validTimeText;
    private LocalizeTextMeshProUGUI _countDownText;
    private LocalizeTextMeshProUGUI _vipUpgradeText;
    private Image _vipIcon;

    private string[] vipIconNames = new[]
    {
        "VipShopIcon_1",
        "VipShopIcon_2",
        "VipShopIcon_3",
    };
    
    private void AwakeVipStore()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.VipStore_PurchaseRefresh, OnPurchaseRefresh);
        EventDispatcher.Instance.AddEventListener(EventEnum.VipStore_RefreshTime, OnRefreshTime);
        EventDispatcher.Instance.AddEventListener(EventEnum.VipStore_RefreshCycleTime, OnRefreshCycleTime);
        
        _currentStarText = transform.Find("Root/LevelState1/Viewport/Content/Top/Image (2)/currentStar").GetComponent<LocalizeTextMeshProUGUI>();
        _vipDaysText = transform.Find("Root/LevelState1/Viewport/Content/Top/vipDays").GetComponent<LocalizeTextMeshProUGUI>();
        _keepVipText = transform.Find("Root/LevelState1/Viewport/Content/Top/keepVip").GetComponent<LocalizeTextMeshProUGUI>();
        _validTimeText = transform.Find("Root/LevelState1/Viewport/Content/Top/validTime").GetComponent<LocalizeTextMeshProUGUI>();
        _countDownText = transform.Find("Root/LevelState1/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _vipUpgradeText = transform.Find("Root/LevelState1/Viewport/Content/Top/text").GetComponent<LocalizeTextMeshProUGUI>();
        
        transform.Find("Root/LevelState1/CloseButton").GetComponent<Button>().onClick.AddListener(OnClickClose);

        _vipIcon = transform.Find("Root/LevelState1/Viewport/Content/Top/Icon").GetComponent<Image>();
        
        InvokeRepeating("VipInvokeUpdate", 0, 1);
        
        // _bottomButtons = transform.Find("Root/LabelGroup").gameObject;
        
        _redPoint = transform.Find("Root/LabelGroup/LevelState1/RedPoint");
        _redPoint.gameObject.SetActive(false);
        
        _vipContent = transform.Find("Root/LevelState1/Viewport/Content");
        _vipScrollRect = transform.Find("Root/LevelState1").GetComponent<ScrollRect>();

        InitVipStore();
        
        if (!VipStoreModel.Instance.IsOpenVipStore())
        {
            // _bottomButtons.gameObject.SetActive(false);
            return;
        }
        
        // _bottomButtons.gameObject.SetActive(true);
        
        
        RefreshVipIcon();
        
    }

    public void InitVipTab()
    {
        var vipStorePage = transform.Find("Root/LevelState1").gameObject.AddComponent<VipStorePage>();
        vipStorePage.Init();
        InitTab(TabState.VipStore,"LevelState1",vipStorePage);
    }

    public class VipStorePage : MonoBehaviour, ITabContent
    {
        public void Show()
        {
            Animator.PlayAnimation("Appear");
            Instance.CheckVipStoreGuide();
        }
        public void Hide()
        {
            Animator.PlayAnimation("Disappear");
        }
        private Transform Root;
        Animator Animator;

        public void Init()
        {
            Animator = transform.GetComponent<Animator>();
        }
    }

    public void CheckVipStoreGuide()
    {
        if (!GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.VipStoreDesc))
        {
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.VipStoreDesc, null))
            {
                var closeBtn = transform.Find("Root/LevelState1/CloseButton").GetComponent<Button>();
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(() =>
                {
                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.VipStoreClose);
                    AnimCloseWindow(() =>
                    {
                        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.VipStoreHeadIcon))
                        {
                            var item = CurrencyGroupManager.Instance.currencyController.expTrans;
                            CurrencyGroupManager.Instance.currencyController._experience.expButton.onClick.AddListener(
                                () =>
                                {
                                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.VipStoreHeadIcon);
                                });
                            List<Transform> topLayer = new List<Transform>();
                            topLayer.Add(item);
                            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.VipStoreHeadIcon, item.transform as RectTransform,
                                topLayer: topLayer);
                            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.VipStoreHeadIcon, null);
                        }
                    });
                });
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(closeBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.VipStoreClose, closeBtn.transform as RectTransform,
                    topLayer: topLayer);
            }
        }
    }
    //
    // private int AnimCount = 0;
    // private bool IsVip = false;
    // private async void SwitchStore(bool isVip, bool quick = false)
    // {
    //     if (!SwitchAnimator)
    //         SwitchAnimator = transform.Find("Root").GetComponent<Animator>();
    //     if (quick)
    //         IsVip = isVip;
    //     else
    //     {
    //         if (IsVip == isVip)
    //             return;
    //         IsVip = isVip;
    //     }
    //     _normalStore.SetSelect(!isVip);
    //     _vipStore.SetSelect(isVip);
    //     AnimCount++;
    //     var animCount = AnimCount;
    //     if (quick)
    //     {
    //         _vipScrollRect.gameObject.SetActive(isVip);
    //         _scrollRect.gameObject.SetActive(!isVip);   
    //         SwitchAnimator.PlayAnimation(isVip?"LevelState2_normal":"LevelState1_normal");
    //     }
    //     else
    //     {
    //         _vipScrollRect.gameObject.SetActive(true);
    //         _scrollRect.gameObject.SetActive(true); 
    //         await SwitchAnimator.PlayAnimationAsync(isVip?"LevelState2":"LevelState1");
    //         if (animCount == AnimCount)
    //         {
    //             _vipScrollRect.gameObject.SetActive(isVip);
    //             _scrollRect.gameObject.SetActive(!isVip);  
    //             SwitchAnimator.PlayAnimation(isVip?"LevelState2_normal":"LevelState1_normal");   
    //         }
    //     }
    //
    //     if (isVip)
    //     {
    //         LayoutRebuilder.ForceRebuildLayoutImmediate(_vipScrollRect.content);
    //         RefreshVipIcon();
    //     }
    // }

    private void InitVipStore()
    {
        int index = 0;
        foreach (var storeSetting in GlobalConfigManager.Instance._vipStoreSettings)
        {
            if(storeSetting.id <= 2)
                continue;
            
            var item = Instantiate(_giftTitle, _vipContent);
            var monoScript = item.gameObject.AddComponent<VipStoreItem>();
            monoScript.Init(storeSetting, index++);
            _vipStoreItems.Add(monoScript);
        }

        RefreshUI();
    }

    private void VipInvokeUpdate()
    {
        RefreshUI();
    }
    private void RefreshUI()
    {
        if (!VipStoreModel.Instance.IsOpenVipStore())
            return;

        RefreshRedPoint();
        
        string cycleTime = VipStoreModel.Instance.GetCycleTime();
        _currentStarText.SetText(FormatString("ui_vipshop_vipstar", VipStoreModel.Instance.GetCurrentStarNum()));
        
        _validTimeText.SetText(FormatString("ui_vipshop_vipstar_expired", cycleTime));
        
        _vipDaysText.SetText(FormatString("ui_vipshop_vipdesc1",VipStoreModel.Instance.GetVipDayNum()));

        _countDownText.SetText(VipStoreModel.Instance.GetRefreshTime());
        
        
        _validTimeText.gameObject.SetActive(true);
        _keepVipText.SetText(FormatString("ui_vipshop_vipkeepLevel", cycleTime, VipStoreModel.Instance.GetCurrentConfig().vipCyclePrice));
        
        int vipLevel = VipStoreModel.Instance.VipLevel();
        TableVipStoreSetting firstConfig = VipStoreModel.Instance.GetFirstConfig();
        if (vipLevel == 0)
        {
            _vipUpgradeText.SetText(FormatString("ui_vipsystem_VipUpgrade", firstConfig.vipPrice-VipStoreModel.Instance.vipStore.PurchasePrice));
            _keepVipText.SetText("");
        }
        else if(vipLevel == VipStoreModel.Instance.GetLastConfig().id)
        {
            _vipUpgradeText.SetText("");
                
            _keepVipText.SetText(FormatString("ui_vipsystem_VipKeepLevel", VipStoreModel.Instance.GetCurrentConfig().vipCyclePrice));
        }
        else
        {
            var nextConfig = VipStoreModel.Instance.GetNextConfig(vipLevel);
                
            _vipUpgradeText.SetText(FormatString("ui_vipsystem_VipUpgrade", nextConfig.vipPrice-VipStoreModel.Instance.vipStore.PurchasePrice));
            _keepVipText.SetText(FormatString("ui_vipsystem_VipKeepLevel", VipStoreModel.Instance.GetCurrentConfig().vipCyclePrice));
        }
        
    }

    private void RefreshRedPoint()
    {
        _redPoint.gameObject.SetActive(false);
        foreach (var kv in GlobalConfigManager.Instance._vipStoreMap)
        {
            if (VipStoreModel.Instance.VipLevel() < kv.Key)
                continue;
            
            foreach (var config in kv.Value)
            {
                if(config.buyCost != 0)
                    continue;
                
                int buyCount = VipStoreModel.Instance.GetBuyCount(config.storeid, config.buyItem);
                int countLeft = config.buyCount - buyCount;
                countLeft = Math.Max(countLeft, 0);
                if(countLeft == 0)
                    continue;
                
                _redPoint.gameObject.SetActive(true);
                return;
            }
            
            if(_redPoint.gameObject.activeSelf)
                return;
        }
    }

    private void RefreshVipIcon()
    {
        int index = GlobalConfigManager.Instance._vipStoreSettings.FindIndex(a => a.id == VipStoreModel.Instance.vipStore.VipLevel);
        index -= 2;
        if(index < 0 || index >= vipIconNames.Length)
            return;

        if(_vipIcon.sprite == null || _vipIcon.sprite.name != vipIconNames[index])
            _vipIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, vipIconNames[index]);
    }
    private string FormatString(string key, object value)
    {
        string localizedString = LocalizationManager.Instance.GetLocalizedString(key);
        return string.Format(localizedString, value);
    }
    
    private string FormatString(string key, object value, object value1)
    {
        string localizedString = LocalizationManager.Instance.GetLocalizedString(key);
        return string.Format(localizedString, value, value1);
    }
    
    private void DestroyVipStore()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.VipStore_PurchaseRefresh, OnPurchaseRefresh);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.VipStore_RefreshTime, OnRefreshTime);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.VipStore_RefreshCycleTime, OnRefreshCycleTime);
    }

    private void OnPurchaseRefresh(BaseEvent e)
    {
        if (!VipStoreModel.Instance.IsOpenVipStore())
            return;
            
        // _bottomButtons.gameObject.SetActive(true);
        _vipStoreItems.ForEach(a=>a.Refresh());
        RefreshVipIcon();
    }
    
    private void OnRefreshTime(BaseEvent e)
    {
        _vipStoreItems.ForEach(a=>a.Refresh());
        RefreshVipIcon();
    }
    private void OnRefreshCycleTime(BaseEvent e)
    {
        _vipStoreItems.ForEach(a=>a.Refresh());
        RefreshVipIcon();
    }
    
}