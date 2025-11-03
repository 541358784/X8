using System;
using DragonPlus;
using Screw;
using Screw.GameLogic;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine.UI;

namespace Screw
{
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupBoosterFeatureUnlock")]
    public class UIBoosterFeatureUnlockPopup : UIWindowController
    {
        [UIBinder("FeatureIcon")] private Image icon;
        [UIBinder("ContinueBtn")] private Button playBtn;

        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);
                
            playBtn.onClick.AddListener(OnPlayBtnClicked);
            
            var feature = ScrewGameLogic.Instance.IsNextLevelUnlockFeature();
            switch (feature)
            {
                case GameFeatureType.StarShape:
                    icon.sprite = AssetModule.Instance.GetSprite("ScrewFeatureAtlas", "ui_feature_icon_StarShape_Big");
                    break;
                case GameFeatureType.ConnectBlocker:
                    icon.sprite = AssetModule.Instance.GetSprite("ScrewFeatureAtlas", "ui_feature_icon_ConnectBlocker_Big");
                    break;
                case GameFeatureType.IceBlocker:
                    icon.sprite = AssetModule.Instance.GetSprite("ScrewFeatureAtlas", "ui_feature_icon_IceBlocker_Small");
                    break;
                case GameFeatureType.ShutterBlocker:
                    icon.sprite = AssetModule.Instance.GetSprite("ScrewFeatureAtlas", "ui_feature_icon_ShutterBlocker_Small");
                    break;
                case GameFeatureType.BombBlocker:
                    icon.sprite = AssetModule.Instance.GetSprite("ScrewFeatureAtlas", "ui_feature_icon_BombBlocker_Small");
                    break;
                case GameFeatureType.TieBlocker:
                    icon.sprite = AssetModule.Instance.GetSprite("ScrewFeatureAtlas", "ui_feature_icon_TieBlocker_Big");
                    break;
                case GameFeatureType.LockBlocker:
                    icon.sprite = AssetModule.Instance.GetSprite("ScrewFeatureAtlas", "ui_feature_icon_LockBlocker_Small");
                    break;
            }
        }
        

        private void OnPlayBtnClicked()
        {
            playBtn.interactable = false;
            AnimCloseWindow();
        }
    }
}