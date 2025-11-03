using Cysharp.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Screw;
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
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupTimesUpFail")]
    public class UITimesUpFail:UIWindowController
    {
        [UIBinder("Root")] private Transform root;
        [UIBinder("GiveupButton")] private Button _buttonClose;
        
        [UIBinder("KeepingButton")] private Button _playButton;
        [UIBinder("CoinNumberText")] private TMP_Text _costText;
        [UIBinder("coinIconinfinite")] private Transform _infiniteTr;
        [UIBinder("FreeButton")] private Button _freeBtn;

        private ScrewGameContext _context;
        [UIBinder("Scroll View")] private Transform _failGroups;
        
        [UIBinder("Mask")] private Transform _failMask;
        private bool _needRecoveryBanner;


        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);

            RegisterEvent();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _context = (ScrewGameContext) objs[0];
            _context.failCount++;
            _freeBtn.gameObject.SetActive(false);

            UpdateText().Forget();
            
            SoundModule.PlaySfx("sfx_level_fail");        
            _infiniteTr.gameObject.SetActive(EnergyData.Instance.EnterGameEnergyInfiniteState);
            
            InitMolePopup();
            InitFailPop();
            //错误注释
            // if (_needRecoveryBanner)
            //     GameApp.Get<AdSys>().HideBanner();
        }

        private void InitFailPop()
        {
            var failPathName = $"TimerGroup";

            AssetModule.Instance.LoadAsset<GameObject>("Screw/Prefabs/PopUp/FailReasonGroup_" + failPathName, _failMask.transform);
        }
        
        private async UniTaskVoid UpdateText()
        {
            await ScrewUtility.WaitNFrame(1);

            var index = _context.failCount;
            var timeLevel = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinLimitTimeLevel;
            if (_context.failCount > timeLevel.Count - 1)
            {
                index = timeLevel.Count - 1;
            }
            _costText.text = timeLevel[index].ToString();
        }

        public void RegisterEvent()
        {
            _playButton.onClick.AddListener(OnPlayOnButtonClicked);
            _freeBtn.onClick.AddListener(() =>
            {
                LevelRelive(0);
            });
            _buttonClose.onClick.AddListener(OnCloseButtonClicked);
            
            //错误注释
            //SubscribeEvent<EventBuyGoldenPassSuccess>(OnEventBuyGoldenPassSuccess);
        }
        
        //错误注释
        // private void OnEventBuyGoldenPassSuccess(EventBuyGoldenPassSuccess evt)
        // {
        //     _playButton.gameObject.SetActive(false);
        //     _freeBtn.gameObject.SetActive(true);
        // }

        private void OnPlayOnButtonClicked()
        {
            //错误注释
            // if (GameApp.Get<IAPSys>().IsInPaying())
            // {
            //     return;
            // }
            
            var index = _context.failCount;
            var timeLevel = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinLimitTimeLevel;
            if (_context.failCount > timeLevel.Count - 1)
            {
                index = timeLevel.Count - 1;
            }

            var coinLimitTimeLevel = timeLevel[index];
            
            //错误注释
            // BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventReliveSuccess,
            //     coinLimitTimeLevel.ToString(),
            //     _context.biLevelInfo.TimeRespawnCoinCount.ToString(), "time", "main");

            if (UserData.UserData.Instance.CanAfford(ResType.Coin, coinLimitTimeLevel))
            {
                LevelRelive(coinLimitTimeLevel);
            }
            else
            {
                UIScrewShop.Open(UIScrewShop.ShopViewGroupType.Coin);
                _needRecoveryBanner = true;
            }
        }

        private void LevelRelive(int coinLimitTimeLevel)
        {
            _playButton.interactable = false;
            _buttonClose.interactable = false;
            _freeBtn.interactable = false;

            if (coinLimitTimeLevel > 0)
            {
                UserData.UserData.Instance.ConsumeRes(ResType.Coin, coinLimitTimeLevel, new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ReliveScrew,
                    data1 = _context.levelIndex.ToString(),
                });
            }

            var timeLevel = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].TimeLimitTimeLevel;
            _context.gameTimer.AddExtraTime(timeLevel);

            //错误注释
            //_context.biLevelInfo.TimeRespawnCoinCount++;
            _context.SetFirstRespawnBi();

            _context.gameState = ScrewGameState.InProgress;
            _context.failReason = LevelFailReason.None;

            //错误注释
            // if (_needRecoveryBanner)
            //     GameApp.Get<AdSys>().TryShowCloseBanner();
            AnimCloseWindow();
        }

        private async void OnCloseButtonClicked()
        {
            //错误注释
            // if (!failPopGroupContextWidget.QuitAni())
            //     return;

            _playButton.interactable = false;
            _buttonClose.interactable = false;
            _context.SendLevelFailBi();

            AnimCloseWindow();

            if (_context.afterFailLevelHandlers != null && _context.afterFailLevelHandlers.Count > 0)
            {
                for (int i = 0; i < _context.afterFailLevelHandlers.Count; i++)
                {
                    await _context.afterFailLevelHandlers[i].Invoke(_context);
                }
            }

            if (_context is ScrewGameSpecialContext)
                SceneFsm.mInstance.ChangeState(StatusType.ScrewHome);
            else
                UIModule.Instance.ShowUI(typeof(UITryAgainPopup), _context, true);
        }

        private void InitMolePopup()
        {
        }
    }
}