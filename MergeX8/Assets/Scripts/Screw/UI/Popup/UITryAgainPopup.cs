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
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupScrewTryAgain")]
    public class UITryAgainPopup:UIWindow
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
               ScrewGameLogic.Instance.GetMainLevelIndex().ToString());

           _localizeTextMeshProUGUI?.SetText(levelTextStr);
           if(_localizeTextMeshProUGUIShadow != null)
               _localizeTextMeshProUGUIShadow.SetText(levelTextStr);

           UpdateContent();

           RegisterEvent();
        }
        
        
        private void UpdateContent()
        {
            bool hasInfinity = UserData.UserData.Instance.GetBuffLeftTime(ResType.EnergyInfinity) > 0;
            _lifeIcon.gameObject.SetActive(!hasInfinity);
            _infiniteLifeIcon.gameObject.SetActive(hasInfinity);
        }

        public void RegisterEvent()
        {
            _tryAgainButton.onClick.AddListener(OnTryAgainButtonClicked);
            _buttonClose?.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnTryAgainButtonClicked()
        {
            SoundModule.PlayButtonClick();
            if (EnergyData.Instance.IsEnergyEmpty())
            {
                SceneFsm.mInstance.ChangeState(StatusType.ScrewHome);
                AnimCloseWindow();
                return;
            }
            
            ((SceneFsmScrewGame)SceneFsm.mInstance.GetCurrentState()).RePlay();
            AnimCloseWindow();
        }

        private void OnCloseButtonClicked()
        {
            SceneFsm.mInstance.ChangeState(StatusType.ScrewHome);
           
            AnimCloseWindow();
        }
    }
}