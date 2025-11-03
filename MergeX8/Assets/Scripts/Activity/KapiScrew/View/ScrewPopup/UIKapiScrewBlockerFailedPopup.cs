using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
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
    [Window(UIWindowLayer.Normal, "Prefabs/Activity/KapiScrew/UIPopupKapibalaBlockerFail")]
    public partial class UIKapiScrewBlockerFailedPopup:UIWindowController
    {
        [UIBinder("KeepingButton")] private Button _playOnButton;
        [UIBinder("CoinNumberText")] private TMP_Text _playOnCostText;
        [UIBinder("GiveupButton")] private Button _buttonClose;
        [UIBinder("ButtonClose")] private Button _btnClose;
        [UIBinder("FreeButton")] private Button _freeBtn;
        [UIBinder("Scroll View")] private Transform _failGroups;
        [UIBinder("coinIconinfinite")] private Transform _infiniteTr;

        [UIBinder("Mask")] private Transform _failMask;
        private ScrewGameContext _context;
        
        private bool _needRecoveryBanner;

        
        
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
            _context = (ScrewGameContext) objs[0];
            _context.failCount++;

            _playOnCostText.text = KapiScrewModel.Instance.GetRebornCount() + "/1";
            SetUpContent();
            InitFailPop();
            
            SoundModule.PlaySfx("sfx_level_fail");
            _infiniteTr.gameObject.SetActive(false);
            
            //错误注释
            // if (_needRecoveryBanner)
            //     GameApp.Get<AdSys>().HideBanner();
            var rebornGroup = transform.Find("Root/Item").gameObject.AddComponent<KapiScrewRebornPackageExtraGroup>();
            rebornGroup.Init(KapiScrewModel.Instance.GetOptionalGiftActivityConfig());
        }


        private void InitFailPop()
        {
            var failPathName = $"{_context.failReason}Group";

            AssetModule.Instance.LoadAsset<GameObject>("Prefabs/Activity/KapiScrew/FailReasonGroup_" + failPathName, _failMask.transform);
        }
        
        private void SetUpContent()
        {
            // if (_context.failReason == LevelFailReason.BombFailed)
            // {
            //     var index = _context.failCount;
            //     var CoinBomb = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinBomb;
            //     if (_context.failCount > CoinBomb.Count - 1)
            //     {
            //         index = CoinBomb.Count - 1;
            //     }
            //     _playOnCostText.text = CoinBomb[index].GetCommaFormat();
            // }
            // else if (_context.failReason == LevelFailReason.IceFailed)
            // {
            //     var index = _context.failCount;
            //     var CoinIce = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinIce;
            //     if (_context.failCount > CoinIce.Count - 1)
            //     {
            //         index = CoinIce.Count - 1;
            //     }
            //     _playOnCostText.text = CoinIce[index].GetCommaFormat();
            // }
            // else if (_context.failReason == LevelFailReason.ShutterFailed)
            // {
            //     var index = _context.failCount;
            //     var CoinShutter = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinShutter;
            //     if (_context.failCount > CoinShutter.Count - 1)
            //     {
            //         index = CoinShutter.Count - 1;
            //     }
            //     _playOnCostText.text = CoinShutter[index].GetCommaFormat();
            // }
            
            _freeBtn.gameObject.SetActive(false);
        }

        public void RegisterEvent()
        {
            _playOnButton.onClick.AddListener(OnPlayButtonClicked);
            // _freeBtn.onClick.AddListener(() =>
            // {
            //     LevelRelive(0);
            // });
            _buttonClose.onClick.AddListener(OnCloseButtonClicked);
            _btnClose.onClick.AddListener(OnCloseButtonClicked);
            //错误注释
            // EventDispatcher.Instance.AddEvent<EventScrewBuyRebornPackage>(OnBuyRebornPackage);
            EventDispatcher.Instance.AddEvent<EventKapiScrewRebornCountChange>(OnRebornCountChange);
        }
        public void OnRebornCountChange(EventKapiScrewRebornCountChange evt)
        {
            _playOnCostText.text = KapiScrewModel.Instance.GetRebornCount() + "/1";
        }

        //错误注释
        // public void OnBuyRebornPackage(EventScrewBuyRebornPackage evt)
        // {
        //     LevelRelive(0);
        // }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventKapiScrewRebornCountChange>(OnRebornCountChange);
            // EventDispatcher.Instance.RemoveEvent<EventScrewBuyRebornPackage>(OnBuyRebornPackage);
        }
        // private void OnEventBuyGoldenPassSuccess(EventBuyGoldenPassSuccess evt)
        // {
        //     _playOnButton.gameObject.SetActive(false);
        //     _freeBtn.gameObject.SetActive(true);
        // }

        private void OnPlayButtonClicked()
        {
            // var coin = 0;
            // if (_context.failReason == LevelFailReason.BombFailed)
            // {
            //     var index = _context.failCount;
            //     var CoinBomb = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinBomb;
            //     if (_context.failCount > CoinBomb.Count - 1)
            //     {
            //         index = CoinBomb.Count - 1;
            //     }
            //     coin = CoinBomb[index];
            // }
            // else if (_context.failReason == LevelFailReason.IceFailed)
            // {
            //     var index = _context.failCount;
            //     var CoinIce = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinIce;
            //     if (_context.failCount > CoinIce.Count - 1)
            //     {
            //         index = CoinIce.Count - 1;
            //     }
            //     coin = CoinIce[index];
            // }
            // else if (_context.failReason == LevelFailReason.ShutterFailed)
            // {
            //     var index = _context.failCount;
            //     var CoinShutter = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].CoinShutter;
            //     if (_context.failCount > CoinShutter.Count - 1)
            //     {
            //         index = CoinShutter.Count - 1;
            //     }
            //     coin = CoinShutter[index];
            // }
            //
            // if (_context.failReason == LevelFailReason.BombFailed)
            // {
            //     //错误注释
            //     // BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventRelive,
            //     //     coin.ToString(),
            //     //     _context.biLevelInfo.BombRespawnCount.ToString(), "bomb", "main");
            // }
            // else
            // {
            //     // BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventRelive,
            //     //     coin.ToString(),
            //     //     _context.biLevelInfo.NoNoilRespawnCount.ToString(), "no_noil", "main");
            // }
            //
            // if (UserData.UserData.Instance.CanAfford(ResType.Coin, coin))
            // {
            //     LevelRelive(coin);
            // }
            // else
            // {
            //     UIScrewShop.Open(UIScrewShop.ShopViewGroupType.Coin);
            //     // _needRecoveryBanner = true;
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
            _buttonClose.interactable = false;
            _btnClose.interactable = false;
            _freeBtn.interactable = false;

            //错误注释
            // BiEventScrewscapes.Types.ItemChangeReason reason = BiEventScrewscapes.Types.ItemChangeReason.BoomRelive;
            // if (_context.failReason == LevelFailReason.BombFailed)
            // {
            //     _context.biLevelInfo.BombRespawnCount++;
            //     BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventReliveSuccess,
            //         coin.ToString(),
            //         _context.biLevelInfo.BombRespawnCount.ToString(), "bomb", "main");
            // }
            // else
            // {
            //     reason = BiEventScrewscapes.Types.ItemChangeReason.SpecialItemRelive;
            //     _context.biLevelInfo.NoNoilRespawnCount++;
            //     BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventReliveSuccess,
            //         coin.ToString(),
            //         _context.biLevelInfo.NoNoilRespawnCount.ToString(), "no_noil", "main");
            // }
            
            KapiScrewModel.Instance.AddRebornItem(-1,"BlockerFail");

            _context.SetFirstRespawnBi();

            var extraSlotBoosterHandler =
                _context.boostersHandler.GetHandler<ExtraSlotBoosterHandler>(BoosterType.ExtraSlot);
            _context.LevelRevived(extraSlotBoosterHandler);

            // 这里要等弹窗关闭才能检测是否失败，否则Fail页面还在Close就Open了导致卡死
            CloseWindowWithinUIMgr(true);
   
            //错误注释
            // if (_needRecoveryBanner)
            //     GameApp.Get<AdSys>().TryShowCloseBanner();
            _context.hookContext.OnLogicEvent(LogicEvent.BlockCheckFail, null);
            _context.hookContext.OnLogicEvent(LogicEvent.CheckTask, null);
        }

        private async void OnCloseButtonClicked()
        {
            _playOnButton.interactable = false;
            _buttonClose.interactable = false;
            _btnClose.interactable = false;
            _context.SendLevelFailBi();

            // AnimCloseWindow();
            CloseWindowWithinUIMgr(true);

            if (_context.afterFailLevelHandlers != null && _context.afterFailLevelHandlers.Count > 0)
            {
                for (int i = 0; i < _context.afterFailLevelHandlers.Count; i++)
                {
                    await _context.afterFailLevelHandlers[i].Invoke(_context);
                }
            }

            UIModule.Instance.ShowUI(typeof(UIKapiScrewTryAgainPopup), this, true);
        }
    }
}