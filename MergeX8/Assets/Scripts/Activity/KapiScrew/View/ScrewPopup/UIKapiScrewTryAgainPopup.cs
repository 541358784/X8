using DragonPlus;
using Screw.GameLogic;
using Screw.UI;
using Screw.UIBinder;
using Screw.UserData;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    [Window(UIWindowLayer.Normal, "Prefabs/Activity/KapiScrew/UIPopupKapibalaScrewTryAgain")]
    public class UIKapiScrewTryAgainPopup:UIWindow
    {
        [UIBinder("ExitButton")] private Button _tryAgainButton;
        [UIBinder("TextTitle")] private LocalizeTextMeshProUGUI _localizeTextMeshProUGUI;
        [UIBinder("TextShadow")] private LocalizeTextMeshProUGUI _localizeTextMeshProUGUIShadow;
        [UIBinder("ButtonClose")] private Button _buttonClose;

        [UIBinder("LoseLifeGroup")] private Transform _lifeIcon;
        [UIBinder("InfiniteLifeGroup")] private Transform _infiniteLifeIcon;


        public override void PrivateAwake()
        {
           ComponentBinderUI.BindingComponent(this, transform);
           
           
           var levelTextStr = LocalizationManager.Instance.GetLocalizedStringWithFormats(
               $"&key.UI_return_reward_help_target",
               (KapiScrewModel.Instance.Storage.BigLevel+1)+"_"+(KapiScrewModel.Instance.Storage.PlayingSmallLevel+1));

           _localizeTextMeshProUGUI?.SetText(levelTextStr);
           if(_localizeTextMeshProUGUIShadow != null)
               _localizeTextMeshProUGUIShadow.SetText(levelTextStr);

           UpdateContent();

           RegisterEvent();
        }
        
        
        private void UpdateContent()
        {
            bool hasInfinity = false;
            _lifeIcon.gameObject.SetActive(!hasInfinity);
            _infiniteLifeIcon.gameObject.SetActive(hasInfinity);
        }

        public void RegisterEvent()
        {
            _tryAgainButton.onClick.AddListener(OnCloseButtonClicked);
            _buttonClose?.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnCloseButtonClicked()
        {
            UIKapiScrewMainController.Instance?.ExitGameOnFail();
            AnimCloseWindow();
        }
    }
}