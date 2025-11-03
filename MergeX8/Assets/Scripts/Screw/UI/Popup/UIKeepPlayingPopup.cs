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
    public interface ILevelFailHandler
    {
        public void OnUserSelectPlayOn();
        public void OnUserSelectGiveUp();
    }

    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupKeepPlaying")]
    public partial class UIKeepPlayingPopup : UIWindow
    {
        [UIBinder("KeepingButton")] private Button _playOnButton;
        [UIBinder("GiveupButton")] private Button _closeButton;
        [UIBinder("CoinNumberText")] private TMP_Text _playOnCostText;
        [UIBinder("coinIconinfinite")] private Transform _infiniteTr;
        [UIBinder("FreeButton")] private Button _freeBtn;

        private ILevelFailHandler _levelFailHandler;

        [UIBinder("Scroll View")] private Transform _failGroups;

        [UIBinder("Mask")] private Transform _failMask;
        
        private ScrewGameContext context;
        private bool _needRecoveryBanner;

        private List<GameObject> _failPopObjects = new List<GameObject>();
        
        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);
            RegisterEvent();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _levelFailHandler = (ILevelFailHandler)objs[0];
            context = (ScrewGameContext)objs[1];
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

            var index = context.failCount;
            var coinPlayOn = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinPlayOn;
            if (context.failCount > coinPlayOn.Count - 1)
            {
                index = coinPlayOn.Count - 1;
            }

            _playOnCostText.text = coinPlayOn[index].GetCommaFormat();
            
            var rebornGroup = transform.Find("Root/Item").gameObject.AddComponent<RebornPackageExtraGroup>();
            rebornGroup.Init(RebornPackageModel.Instance.Configs[0]);
        }


        public void RegisterEvent()
        {
            _playOnButton.onClick.AddListener(OnPlayOnButtonClicked);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            _freeBtn.onClick.AddListener(() => { LevelRelive(0); });

            //错误注释
            EventDispatcher.Instance.AddEvent<EventScrewBuyRebornPackage>(OnBuyRebornPackage);
        }
        public void OnBuyRebornPackage(EventScrewBuyRebornPackage evt)
        {
            LevelRelive(0);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventScrewBuyRebornPackage>(OnBuyRebornPackage);
        }
        private void InitFailPop()
        {
            var failPathName = $"{context.failReason}Group";

            var obj = AssetModule.Instance.LoadAsset<GameObject>("Screw/Prefabs/PopUp/FailReasonGroup_" + failPathName, _failMask.transform);
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

            var index = context.failCount;
            var coinPlayOn = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinPlayOn;
            if (context.failCount > coinPlayOn.Count - 1)
            {
                index = coinPlayOn.Count - 1;
            }


            //错误注释
            // BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventRelive,
            //     coinPlayOn.ToString(),
            //     context.biLevelInfo.NoGridRespawnCount.ToString(), "no_grid", "main");

            var coin = coinPlayOn[index];
            if (UserData.UserData.Instance.CanAfford(ResType.Coin, coin))
            {
                LevelRelive(coin);
            }
            else
            {
                //错误注释
                UIScrewShop.Open(UIScrewShop.ShopViewGroupType.Coin);

                _needRecoveryBanner = true;
            }
        }

        private void LevelRelive(int coinPlayOn)
        {
            _playOnButton.interactable = false;
            _closeButton.interactable = false;
            _freeBtn.interactable = false;

            if (coinPlayOn > 0)
            {
                //错误注释
                //var itemChangeReasonArgs = new BIHelper.ItemChangeReasonArgs(BiEventScrewscapes.Types.ItemChangeReason.SpaceRelive);
                UserData.UserData.Instance.ConsumeRes(ResType.Coin, coinPlayOn, new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ReliveScrew,
                    data1 = ScrewGameLogic.Instance.context.levelIndex.ToString(),
                });
            }

            // AnimCloseWindow();
            CloseWindowWithinUIMgr(true);
            _levelFailHandler?.OnUserSelectPlayOn();
            //错误注释
            // if (_needRecoveryBanner)
            //     GameApp.Get<AdSys>().TryShowCloseBanner();
            // BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventReliveSuccess,
            //     coinPlayOn.ToString(),
            //     context.biLevelInfo.NoGridRespawnCount.ToString(), "no_grid", "main");
        }

        private void OnCloseButtonClicked()
        {
            //错误注释
            // if (failPopGroupContextWidget != null && !failPopGroupContextWidget.QuitAni())
            //     return;

            _playOnButton.interactable = false;
            _closeButton.interactable = false;
            // AnimCloseWindow();
            CloseWindowWithinUIMgr(true);
            _levelFailHandler?.OnUserSelectGiveUp();
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