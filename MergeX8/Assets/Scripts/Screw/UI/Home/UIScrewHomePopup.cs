using System;
using System.Collections.Generic;
using Decoration;
using DragonPlus;
using Farm.Model;
using Framework;
using Screw.GameLogic;
using Screw.Module;
using Screw.UI;
using Screw.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/Main/UIScrewHome")]
    public partial class UIScrewHomePopup : UIWindowController
    {
        private ResBarMono _energyResBar;
        private ResBarMono _coinResBar;
        private Button _shopButton;

        private LocalizeTextMeshProUGUI _levelText;

        private Button _screwButton;
        private Button _mergeButton;
        
        public override void PrivateAwake()
        {
            _energyResBar = transform.Find("Root/UIScrewResourcesGroup/Root/ResourcesGroup/ResourcesBarGroup1").gameObject.AddComponent<EnergyResBar>();
            _coinResBar = transform.Find("Root/UIScrewResourcesGroup/Root/ResourcesGroup/ResourcesBarGroup2").gameObject.AddComponent<CoinResBar>();

            _shopButton = transform.Find("Root/UIScrewResourcesGroup/Root/ResourcesGroup/ButtonShop").GetComponent<Button>();
            _shopButton.onClick.AddListener(() =>
            {
                UIScrewShop.Open(UIScrewShop.ShopViewGroupType.None);
            });

            _levelText = transform.Find("Root/LevelGroup/NumText").GetComponent<LocalizeTextMeshProUGUI>();

            _screwButton = transform.Find("Root/ButtonGroup/ButtonStart").GetComponent<Button>();
            _screwButton.onClick.AddListener(OnClickScrew);
            ResBarModule.Instance.RegisterResBar(ResBarType.MainPlay, _screwButton.transform);
            
            _mergeButton = transform.Find("Root/ButtonGroup/ButtonSynthesis").GetComponent<Button>();
            _mergeButton.onClick.AddListener(OnClickMerge);
            
            CommonUtils.NotchAdapte(transform.Find("Root/UIScrewResourcesGroup"));

            RefreshLevelText();
            InitSideView();
            
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_mergeButton.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ScrewToMerge, _mergeButton.transform as RectTransform, topLayer: topLayer);
        }

        private void OnClickScrew()
        {
            if (EnergyData.Instance.GetEnergy() > 0 || EnergyData.Instance.IsInfiniteEnergy())
            {
                SceneFsm.mInstance.EnterScrewGame(-1);   
            }
            else
            {
                UIModule.Instance.ShowUI(typeof(UILifePurchasePopup));
            }
        }

        private void OnClickMerge()
        {
            SceneFsm.mInstance.ChangeState(StatusType.BackHome);
            FarmModel.Instance.AnimShow(true);
            UIHomeMainController.ShowUI();
            PlayerManager.Instance.RecoverPlayer();
            DecoManager.Instance.CurrentWorld.ShowByPosition();
            UIRoot.Instance.EnableEventSystem = true;

            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.1f, () =>
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
            }));

            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ScrewToMerge);
        }

        private void RefreshLevelText()
        {
            _levelText.SetText(ScrewGameLogic.Instance.GetMainLevelIndex().ToString());
        }

        public void InitSideView()
        {
            InitDailyPackageEntrance();
        }
        private void OnDestroy()
        {
            ResBarModule.Instance.UnRegisterResBar(ResBarType.MainPlay, _screwButton.transform);
        }
    }
}