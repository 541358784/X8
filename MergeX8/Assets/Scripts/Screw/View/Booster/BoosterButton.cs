using DragonPlus;
using Screw.Module;
using Screw.UIBinder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    public class BoosterButton : Entity
    {
        [UIBinder("Lock")] private Transform lockContainer;
        [UIBinder("UnLock")] private Transform unLockContainer;
        [UIBinder("Count")] private TMP_Text countText;
        [UIBinder("UnLockTip")] private LocalizeTextMeshProUGUI unLockTip;
        [UIBinder("Plus")] private Transform plusTrans;
        [UIBinder("BoosterCount")] private Transform boosterCountGroup;
        [UIBinder("DisableIcon")] private Transform disableGroup;

        [UIBinder("UnLockBk")] private Transform unLockBg;
        [UIBinder("Icon")] private Transform icon;
        [UIBinder("Text")] private LocalizeTextMeshProUGUI tipTxt;

        
        private BoosterType _boosterType;

        private bool isActive = false;
        private int guideParams = 0;

        private BoosterHandler _boosterHandler;

        private Button _button;

        public BoosterButton(Transform root, ScrewGameContext context, BoosterType boosterType)
        {
            Bind(root, context);
            _boosterType = boosterType;
            _button = root.GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);

            _boosterHandler = context.boostersHandler.GetHandler(boosterType);
        }
       
        protected bool BoosterCountIsEnough()
        {
            return _boosterHandler.GetBoosterFreeCount() > 0 || _boosterHandler.GetBoosterCount() > 0;
        }
        
        private void OnButtonClicked()
        {
            if (context.actionController.HasActionInExecute())
                return;
            
            if (_boosterHandler.IsBoosterLocked())
                return;
          
            //再检查是否可用
            if (!_boosterHandler.CanUse())
                return;
            
            //先检查数量
            if (!BoosterCountIsEnough())
            {
                //迁移报错注释
                // BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventBuyItemPop,
                //     _boosterType.ToString());

                BoostersHandler.ShowPurchaseBoosterPopup(_boosterHandler);
                return;
            }
            //使用道具
            context.boostersHandler.OnBoosterClicked(_boosterType);
        }
 
        public void UpdateLockState()
        {
            var boosterHandler = context.boostersHandler.GetHandler(_boosterType);
            var locked = boosterHandler.IsBoosterLocked();

            lockContainer.gameObject.SetActive(locked);
            unLockContainer.gameObject.SetActive(!locked);

            if (locked)
                unLockTip.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_unlock_tips",
                    boosterHandler.GetUnlockLevel().ToString()));

            if (unLockBg && unLockBg.gameObject.activeSelf != !locked)
                unLockBg.gameObject.SetActive(!locked);
            if (icon && icon.gameObject.activeSelf != !locked)
                icon.gameObject.SetActive(!locked);

            _button.interactable = !locked;

            if (boosterHandler.GetUnlockLevel() == context.levelIndex && !boosterHandler.GetShowBoosterUnLockPop())
            {
                UIModule.Instance.ShowUI(typeof(UIBoosterUnlockPopup) ,_boosterType, context);
                boosterHandler.SetShowBoosterUnLockPop();
            }
        }

        public async void OnPurchaseBooster(BoosterType boosterType, int count)
        {
            if (boosterType == _boosterType)
            {
                var boosterHandler = context.boostersHandler.GetHandler(_boosterType);
                var boosterCount = boosterHandler.GetBoosterCount();
                var startCount = boosterCount - count;
                await ScrewUtility.WaitSeconds(1.1f, false);
                boosterCountGroup.gameObject.SetActive(true);
                while (startCount < boosterCount)
                {
                    startCount++;
                    // buttonAnimator.Play("Collect",0,0);
                    countText.SetText(startCount.ToString());
                    await ScrewUtility.WaitSeconds(0.3f, false);
                }
                
                UpdateBoosterCount();
            }
        }

        public void UpdateBoosterCount()
        {
            var boosterHandler = context.boostersHandler.GetHandler(_boosterType);
            var boosterCount = boosterHandler.GetBoosterCount();
            if (boosterCount > 0)
                countText.SetText(boosterCount.ToString());

            if (disableGroup)
            {
                disableGroup.gameObject.SetActive(!boosterHandler.CanUse());
                plusTrans.gameObject.SetActive(boosterCount <= 0 && !disableGroup.gameObject.activeSelf);
            }
            else
            {
                plusTrans.gameObject.SetActive(boosterCount <= 0);
            }

            boosterCountGroup.gameObject.SetActive(boosterCount > 0);
        }

        public void UpdateText()
        {
            if (tipTxt)
            {
                tipTxt.SetTerm("&key.UI_item_hammer_use");
            }
            
            var boosterHandler = context.boostersHandler.GetHandler(_boosterType);
            var locked = boosterHandler.IsBoosterLocked();
            if (locked)
                unLockTip.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_unlock_tips",
                    boosterHandler.GetUnlockLevel().ToString()));
        }
    }
}