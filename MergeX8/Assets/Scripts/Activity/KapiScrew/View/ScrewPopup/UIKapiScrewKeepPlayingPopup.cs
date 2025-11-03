using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Screw.GameLogic;
using Screw.Module;
using Screw.UI;
using Screw.UIBinder;
using Screw.UserData;
using TMatch;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{

    [Window(UIWindowLayer.Normal, "Prefabs/Activity/KapiScrew/UIPopupKapibalaKeepPlaying")]
    public partial class UIKapiScrewKeepPlayingPopup : UIWindow
    {
        [UIBinder("KeepingButton")] private Button _playOnButton;
        [UIBinder("GiveupButton")] private Button _closeButton;
        [UIBinder("ButtonClose")] private Button _btnClose;
        [UIBinder("CoinNumberText")] private TMP_Text _playOnCostText;
        [UIBinder("coinIconinfinite")] private Transform _infiniteTr;
        [UIBinder("FreeButton")] private Button _freeBtn;


        [UIBinder("Scroll View")] private Transform _failGroups;

        [UIBinder("Mask")] private Transform _failMask;
        
        private ScrewGameContext context;
        private bool _needRecoveryBanner;

        private List<GameObject> _failPopObjects = new List<GameObject>();
        
        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);
            RegisterEvent();
            transform.Find("Root/KeepingButton/CoinIcon").GetComponent<Image>().sprite =
                Gameplay.UserData.GetResourceIcon(Gameplay.UserData.ResourceId.KapiScrewReborn,
                    Gameplay.UserData.ResourceSubType.Big);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            // _levelFailHandler = (ILevelFailHandler)objs[0];
            context = (ScrewGameContext)objs[0];
            context.failCount++;

            _freeBtn.gameObject.SetActive(false);

            SoundModule.PlaySfx("sfx_level_fail");
            
            //错误注释
            // CreateWidgetByPath<FailPopPurchaseWidget>(gameObject.transform, "FailOtherGroup", true, context);
            // failPopGroupContextWidget = CreateWidget<FailPopGroupContextWidget>(_failGroups.gameObject, true, context);

            _infiniteTr.gameObject.SetActive(UserData.EnergyData.Instance.EnterGameEnergyInfiniteState);

            InitMolePopup();
            InitFailPop();
            
            
            //错误注释
            // if (_needRecoveryBanner)
            //     GameApp.Get<AdSys>().HideBanner();

            // var index = context.failCount;
            // var coinPlayOn = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinPlayOn;
            // if (context.failCount > coinPlayOn.Count - 1)
            // {
            //     index = coinPlayOn.Count - 1;
            // }
            //
            // _playOnCostText.text = coinPlayOn[index].GetCommaFormat();

            _playOnCostText.text = KapiScrewModel.Instance.GetRebornCount() + "/1";
            
            var rebornGroup = transform.Find("Root/Item").gameObject.AddComponent<KapiScrewRebornPackageExtraGroup>();
            rebornGroup.Init(KapiScrewModel.Instance.GetOptionalGiftActivityConfig());
        }


        public void RegisterEvent()
        {
            _playOnButton.onClick.AddListener(OnPlayOnButtonClicked);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            _btnClose.onClick.AddListener(OnCloseButtonClicked);
            // _freeBtn.onClick.AddListener(() => { LevelRelive(0); });

            //错误注释
            // EventDispatcher.Instance.AddEvent<EventScrewBuyRebornPackage>(OnBuyRebornPackage);
            EventDispatcher.Instance.AddEvent<EventKapiScrewRebornCountChange>(OnRebornCountChange);
        }
        // public void OnBuyRebornPackage(EventScrewBuyRebornPackage evt)
        // {
        //     LevelRelive(0);
        // }
        public void OnRebornCountChange(EventKapiScrewRebornCountChange evt)
        {
            _playOnCostText.text = KapiScrewModel.Instance.GetRebornCount() + "/1";
        }

        private void OnDestroy()
        {
            // EventDispatcher.Instance.RemoveEvent<EventScrewBuyRebornPackage>(OnBuyRebornPackage);
            EventDispatcher.Instance.RemoveEvent<EventKapiScrewRebornCountChange>(OnRebornCountChange);
        }
        private void InitFailPop()
        {
            var failPathName = $"{context.failReason}Group";

            var obj = AssetModule.Instance.LoadAsset<GameObject>("Prefabs/Activity/KapiScrew/FailReasonGroup_" + failPathName, _failMask.transform);
            if(obj == null)
                return;
            
            _failPopObjects.Add(obj);
        }
        //错误注释
        // private void OnEventBuyGoldenPassSuccess(EventBuyGoldenPassSuccess evt)
        // {
        //     _playOnButton.gameObject.SetActive(false);
        //     _freeBtn.gameObject.SetActive(true);
        // }

        private void OnPlayOnButtonClicked()
        {
            //错误注释
            // if (GameApp.Get<IAPSys>().IsInPaying())
            // {
            //     return;
            // }

            // var index = context.failCount;
            // var coinPlayOn = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinPlayOn;
            // if (context.failCount > coinPlayOn.Count - 1)
            // {
            //     index = coinPlayOn.Count - 1;
            // }


            //错误注释
            // BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventRelive,
            //     coinPlayOn.ToString(),
            //     context.biLevelInfo.NoGridRespawnCount.ToString(), "no_grid", "main");

            // var coin = coinPlayOn[index];
            // if (UserData.UserData.Instance.CanAfford(ResType.Coin, coin))
            // {
            //     LevelRelive(coin);
            // }
            // else
            // {
            //     //错误注释
            //     UIScrewShop.Open(UIScrewShop.ShopViewGroupType.Coin);
            //
            //     _needRecoveryBanner = true;
            // }
            if (KapiScrewModel.Instance.GetRebornCount() > 0)
            {
                LevelRelive();
            }
            else
            {
                UIPopupKapiScrewShopController.Open();
            }
        }

        private void LevelRelive()
        {
            _playOnButton.interactable = false;
            _closeButton.interactable = false;
            _btnClose.interactable = false;
            _freeBtn.interactable = false;

            KapiScrewModel.Instance.AddRebornItem(-1,"FullFail");

            // AnimCloseWindow();
            CloseWindowWithinUIMgr(true);
            context?.OnUserSelectPlayOn();
        }

        private void OnCloseButtonClicked()
        {
            //错误注释
            // if (failPopGroupContextWidget != null && !failPopGroupContextWidget.QuitAni())
            //     return;

            _playOnButton.interactable = false;
            _closeButton.interactable = false;
            _btnClose.interactable = false;
            // AnimCloseWindow();
            CloseWindowWithinUIMgr(true);
            context?.OnUserSelectGiveUp();
        }

        private void InitMolePopup()
        {
            //错误注释
            // var activity = GameApp.Get<ActivitySys>().GetActivity<Activity_MoleSprint>(ActivityType.MoleSprint);
            // if (activity != null && activity.IsActivityOpened() && activity.IsInChallenge())
            // {
            //     var widget = CreateWidgetByPath<Mole_InfoPopup>(gameObject.transform, "UISealSprintInPopUp");
            //     var obj = new GameObject("MoleContent");
            //     var rect = obj.AddComponent<RectTransform>();
            //     obj.transform.SetParent(rectTransform);
            //     rect.sizeDelta = widget.rectTransform.sizeDelta;
            //     _needRecoveryBanner = true;
            // }
        }
    }
}