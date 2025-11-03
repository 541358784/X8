using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Setting
{
    public class UIPopupSetVipController : UIWindowController
    {
        public static UIPopupSetVipController Open(Action callback = null)
        {
            return UIManager.Instance.OpenUI(UINameConst.UIPopupSetVip, callback) as UIPopupSetVipController;
        }

        private Action Callback;
        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            Callback = objs.Length > 0 ? objs[0] as Action : null;
            XUtility.WaitFrames(1, CheckVipStoreGuide);
        }

        public void CheckVipStoreGuide()
        {
            if (!GuideSubSystem.Instance.IsShowingGuide() &&
                !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.VipStoreDetailDesc))
            {
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.VipStoreDetailDesc, null))
                {
                }
            }
        }

        private Button _closeButton;

        private List<Transform> _currentLevels = new List<Transform>();
        private List<Transform> _nextLevels= new List<Transform>();

        private LocalizeTextMeshProUGUI _sliderText;
        private Slider _vipSlider;
        private LocalizeTextMeshProUGUI _vipText;

        private LocalizeTextMeshProUGUI _vipDayText;
        private LocalizeTextMeshProUGUI _vipKeepText;
        private LocalizeTextMeshProUGUI _vipExpiredText;
        
        
        private class VipItem
        {
            private Transform _normal;
            private Transform _lock;
            private Transform _current;

            private List<Transform> _icons = new List<Transform>();

            private Transform _root;

            private int _level;

            private LocalizeTextMeshProUGUI _vipText;
            
            public VipItem(Transform root, int level)
            {
                _level = level;
                _root = root;

                _normal = _root.Find("Normal");
                _lock = _root.Find("Lock");
                _current = _root.Find("Current");

                string vipLevel = LocalizationManager.Instance.GetLocalizedString("ui_vipsystem_VipLv");
                string vipLockLevel = LocalizationManager.Instance.GetLocalizedString("ui_vipsystem_VipUnlock");
                _normal.transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(string.Format(vipLevel, level));
                _lock.transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(string.Format(vipLevel, level));
                _current.transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(string.Format(vipLevel, level));
                
                _lock.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(string.Format(vipLockLevel, level));
                
                for (int i = 1; i <= 3; i++)
                {
                    _icons.Add(root.Find("GameObject/"+i));
                }

                _vipText = root.Find("GameObject/Text").GetComponent<LocalizeTextMeshProUGUI>();
                
                RefreshUI();
            }

            private void RefreshUI()
            {
                int vipLevel = VipStoreModel.Instance.VipLevel();

                _lock.gameObject.SetActive(vipLevel < _level);
                _normal.gameObject.SetActive(vipLevel > _level);
                _current.gameObject.SetActive(vipLevel == _level);
                
                for (var i = 0; i < _icons.Count; i++)
                {
                    if(_icons[i] == null)
                        continue;

                    var localVipLevel = vipLevel <= 3 ? 3 : vipLevel;
                    _icons[i].gameObject.SetActive(localVipLevel-3 == i);
                }

                switch (_level)
                {
                    case 2:
                    {
                        _vipText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_vipsystem_VipBenefits2"), GlobalConfigManager.Instance.GetNumValue("vip_daily_claim_diamond")));
                        break;
                    }
                    case 3:
                    {
                        if (vipLevel >= 3)
                        {
                            _vipText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_vipsystem_VipBenefits3"), vipLevel-2));
                        }
                        else
                        {
                            _vipText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_vipsystem_VipBenefits3"), 1));
                        }
                        break;
                    }
                }
            }
        }

        private List<VipItem> _vipItems = new List<VipItem>();
        
        public override void PrivateAwake()
        {
            _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _closeButton.onClick.AddListener(()=>AnimCloseWindow(() =>
            {
                Callback?.Invoke();
            }));

            for (int i = 0; i <= 5; i++)
            {
                _currentLevels.Add(transform.Find("Root/TopGroup/Vip/" + i));
            }
            
            for (int i = 1; i <= 5; i++)
            {
                _nextLevels.Add(transform.Find("Root/TopGroup/Slider/NextVip/" + i));
                _vipItems.Add(new VipItem(transform.Find("Root/Vip/Viewport/Content/"+i), i));
            }
            
            _sliderText = transform.Find("Root/TopGroup/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _vipText = transform.Find("Root/TopGroup/UpgradeText").GetComponent<LocalizeTextMeshProUGUI>();
            _vipSlider = transform.Find("Root/TopGroup/Slider").GetComponent<Slider>();
            _vipDayText  = transform.Find("Root/TopGroup/DayText").GetComponent<LocalizeTextMeshProUGUI>();
            _vipKeepText = transform.Find("Root/TopGroup/KeepLevelText").GetComponent<LocalizeTextMeshProUGUI>();
            _vipExpiredText= transform.Find("Root/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            
            InvokeRepeating("InvokeUpdate", 0, 1);
        }

        private void Start()
        {
            int vipLevel = VipStoreModel.Instance.VipLevel();
            
            for (var i = 0; i < _currentLevels.Count; i++)
            {
                _currentLevels[i].gameObject.SetActive(i == vipLevel);
            }

            for (var i = 0; i < _nextLevels.Count; i++)
            {
                _nextLevels[i].gameObject.SetActive(i == vipLevel);
            }

            TableVipStoreSetting firstConfig = VipStoreModel.Instance.GetFirstConfig();
            if (vipLevel == 0)
            {
                _vipSlider.value = 0;
                _sliderText.SetText("0/"+firstConfig.vipPrice);
                _vipText.SetText(FormatString("ui_vipsystem_VipUpgrade", firstConfig.vipPrice-VipStoreModel.Instance.vipStore.PurchasePrice));
                _vipKeepText.SetText("");
            }
            else if(vipLevel == VipStoreModel.Instance.GetLastConfig().id)
            {
                _vipSlider.value = 1;
                _sliderText.SetTerm("UI_max");
                _vipText.SetText("");
                
                _vipKeepText.SetText(FormatString("ui_vipsystem_VipKeepLevel", VipStoreModel.Instance.GetCurrentConfig().vipCyclePrice));
            }
            else
            {
                var nextConfig = VipStoreModel.Instance.GetNextConfig(vipLevel);
                
                _vipSlider.value = 1.0f * VipStoreModel.Instance.vipStore.PurchasePrice / nextConfig.vipPrice;
                _sliderText.SetText(VipStoreModel.Instance.vipStore.PurchasePrice +"/"+nextConfig.vipPrice);
                _vipText.SetText(FormatString("ui_vipsystem_VipUpgrade", nextConfig.vipPrice-VipStoreModel.Instance.vipStore.PurchasePrice));
                
                _vipKeepText.SetText(FormatString("ui_vipsystem_VipKeepLevel", VipStoreModel.Instance.GetCurrentConfig().vipCyclePrice));
            }
            
            _vipDayText.SetText(FormatString("ui_vipshop_vipdesc1",VipStoreModel.Instance.GetVipDayNum()));
        }

        private void InvokeUpdate()
        {
            int vipLevel = VipStoreModel.Instance.VipLevel();
            if (vipLevel == 0)
            {
                _vipExpiredText.SetText("");
            }
            else
            {                
                string cycleTime = VipStoreModel.Instance.GetCycleTime();
                _vipExpiredText.SetText(FormatString("ui_vipsystem_VipExpired", cycleTime));
            }
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
    }
}