using System;
using DragonPlus;
using DragonPlus.Config.ClimbTower;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

namespace ActivityLocal.ClimbTower.Model
{
    public class UIPopupClimbTowerPayController: UIWindowController
    {
        private Button _close;
        private Button _yes;
        private Animator _animator;
        private bool _canClick = false;
        private LocalizeTextMeshProUGUI _vipText;
        
        public override void PrivateAwake()
        {
            _animator = transform.GetComponent<Animator>();
            
            _close = transform.Find("Root/CloseButton").GetComponent<Button>();
            _close.onClick.AddListener(() =>
            {
                if(!_canClick)
                    return;
                
                UIManager.Instance.CloseUI(UINameConst.UIClimbTowerMain, true);
                UIManager.Instance.CloseUI(UINameConst.UIClimbTowerMainPay, true);
                AnimCloseWindow();
            });
                
            _yes = transform.Find("Root/ButtonYse").GetComponent<Button>();
            _yes.onClick.AddListener(() =>
            {
                if(!_canClick)
                    return;
                
                StoreModel.Instance.Purchase(GetShopId(), "ClimbTower");
            });

            var text = transform.Find("Root/ButtonYse/Text").GetComponent<Text>();
            text.text = StoreModel.Instance.GetPrice(GetShopId());

            var payText = transform.Find("Root/ButtonYse/Price/Text").GetComponent<Text>();
            payText.text = StoreModel.Instance.GetPrice(ClimbTowerConfigManager.Instance.GetSettingConfig().PayShopId);
            
            _vipText = gameObject.transform.Find("Root/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(GetShopId()));
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            StartCoroutine(CommonUtils.PlayAnimation(_animator, ClimbTowerModel.Instance.ClimbTower.IsPay ? "appear_2" : "appear_1", null,() =>
            {
                _canClick = true;
            }));
        }

        private int GetShopId()
        {
            int shopId = ClimbTowerConfigManager.Instance.GetSettingConfig().FirstPayShopId;
            if(ClimbTowerModel.Instance.ClimbTower.IsPay)
                shopId = ClimbTowerConfigManager.Instance.GetSettingConfig().PayShopId;

            return shopId;
        }
    }
}