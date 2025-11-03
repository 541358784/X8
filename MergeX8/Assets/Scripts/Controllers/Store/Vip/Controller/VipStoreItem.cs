using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay.UI.Store.Vip.Model;
using GamePool;
using UnityEngine;

namespace Gameplay.UI.Store.Vip.Controller
{
    public class VipStoreItem : MonoBehaviour
    {
        private TableVipStoreSetting _settingConfig;
        private int _index;

        private LocalizeTextMeshProUGUI _titleText;
        private LocalizeTextMeshProUGUI _countDownText;
        private GameObject _timeGroup;
        private Transform _normalContent;

        private List<VipStoreCell> _vipStoreCells = new List<VipStoreCell>();

        private GameObject _lockObj;
        private LocalizeTextMeshProUGUI _lockText;


        private string[] _vipLevelKey = new string[]
        {
            "ui_vipshop_viplevel1",
            "ui_vipshop_viplevel2",
            "ui_vipshop_viplevel3",
        };
        
        private string[] _vipLockKey = new string[]
        {
            "ui_vipshop_vipdesc2",
            "ui_vipshop_vipdesc2",
            "ui_vipshop_vipdesc3",
        };
        
        public void Init(TableVipStoreSetting config, int index)
        {
            _settingConfig = config;
            _index = index;

            InitUIComponent();
            InitStoreContent();
            
            _countDownText.SetText(VipStoreModel.Instance.GetRefreshTime());

            Refresh();
        }

        private void Start()
        {
            //InvokeRepeating("InvokeUpdate", 0, 1);
        }

        private void InvokeUpdate()
        {
            //_countDownText.SetText(VipStoreModel.Instance.GetRefreshTime());
        }
        
        public void Refresh()
        {
            if(!VipStoreModel.Instance.IsOpenVipStore())
                return;
            
            _vipStoreCells.ForEach(a=>a.Refresh());
            
            _lockObj.gameObject.SetActive(VipStoreModel.Instance.GetCurrentConfig().id < _settingConfig.id);
            RefreshLockText();
        }
        
        private void InitUIComponent()
        {
            transform.Find("TitleBg_Vip").gameObject.SetActive(true);
            transform.Find("TitleBg").gameObject.SetActive(false);
            
            _titleText = transform.Find("TitleBg_Vip/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
            transform.Find("TagBG").gameObject.SetActive(false);
            transform.Find("UIShopRefresh").gameObject.SetActive(false);
            _timeGroup = transform.Find("TimeGroup").gameObject; 
            _timeGroup.gameObject.SetActive(false);
            _countDownText = transform.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            _normalContent = transform.Find("NormalContent");
            transform.Find("SpecialContent").gameObject.SetActive(false);

            _lockObj = transform.Find("Lock").gameObject;
            _lockText = transform.Find("Lock/Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            _titleText.SetTerm(_vipLevelKey[_index]);
            RefreshLockText();
        }

        private void RefreshLockText()
        {
            int price = _settingConfig.vipPrice;
            var frontConfig = VipStoreModel.Instance.GetFrontConfig(_settingConfig.id);
            var currentConfig = VipStoreModel.Instance.GetCurrentConfig();
            if (frontConfig != null && frontConfig.id != _settingConfig.id && currentConfig != null && currentConfig.id < frontConfig.id)
                price += frontConfig.vipPrice;
            
            _lockText.SetText(FormatString(_vipLockKey[_index], price));
        }
        
        private string FormatString(string key, object value)
        {
            string localizedString = LocalizationManager.Instance.GetLocalizedString(key);
            return string.Format(localizedString, value);
        }
        
        private void InitStoreContent()
        {
            var vipStoreInfo = GlobalConfigManager.Instance.GetVipStoreInfo(_settingConfig.id);
            foreach (var config in vipStoreInfo)
            {
                Transform item = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ShopItemFlashPath).transform;
                item.SetParent(_normalContent);
                item.localScale = Vector3.one;
                item.localPosition = Vector3.zero;

                var mono = item.gameObject.GetComponentDefault<VipStoreCell>();
                mono.Init(config, _settingConfig);
                _vipStoreCells.Add(mono);
            }
        }

        private void OnDestroy()
        {
            foreach (var mono in _vipStoreCells)
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemFlashPath, mono.gameObject);
                DestroyImmediate(mono);
            }
            
            _lockObj.gameObject.SetActive(false);
        }
    }
}