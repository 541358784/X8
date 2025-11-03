using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
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
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupScrewWin")]
    public class UIScrewWinPopup:UIWindowController
    {
        [UIBinder("Root")] private RectTransform root;
        [UIBinder("GetButton")] private Button _continueButton;

        [UIBinder("Root/HaveRewardGroup/RewardGroup/GetButton/coinIcon")] private Transform _icon;
        [UIBinder("NumText")] private LocalizeTextMeshProUGUI _levelTxt;
        [UIBinder("CurrencyCoin")] private Transform _currencyCoin;
        
        [UIBinder("CountText")] private LocalizeTextMeshProUGUI _rewardTxt;
        
        [UIBinder("Root/HaveRewardGroup/Progress")] private Slider _progressSlider; 
        [UIBinder("Root/HaveRewardGroup/Progress/FeatureIcon/IconMask/Icon")] private Image _featureIcon; 
        [UIBinder("Root/HaveRewardGroup/Progress/UnlockFeatureProgressText")] private TMP_Text _progressText; 
        [UIBinder("Root/HaveRewardGroup")] public Transform Content; 

        private ScrewGameContext _screwGameContext;
  
        private GameFeatureType _gameFeatureType;

        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);

            RegisterEvent();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _screwGameContext = (ScrewGameContext) objs[0];
            _gameFeatureType = (GameFeatureType) objs[1];
            SoundModule.PlaySfx("sfx_level_complete");

            if (NeedPlaySettleReward())
            {
                //错误注释
                //CreateWidget<CoinWidget>(_currencyCoin.gameObject);
            }
            else
            {
                _currencyCoin?.gameObject.SetActive(false);
            }

            AddReward();
            _screwGameContext.Record.Record();

            var levelTextStr = LocalizationManager.Instance.GetLocalizedStringWithFormats(
                $"&key.UI_return_reward_help_target",
                _screwGameContext.levelIndex);
            _levelTxt.SetText(_screwGameContext.levelIndex.ToString());
            _levelTxt.gameObject.SetActive(_screwGameContext is ScrewGameContext);


            SetUpUnlockFeatureUI();
            UpdateUnlockProgress(1f);
            SendLevelWinBi();
            
            //错误注释
            // if (GameApp.Get<AdSys>().ShowingBanner)
            // {
            //     GameApp.Get<AdSys>().HideBanner();
            // }

            AdaptHeader();
        }

        private void AdaptHeader()
        {
            root.offsetMax = new Vector2(root.offsetMax.x, ScrewUtility.GetSafeAreaOffset());
        }

        private void AddReward()
        {
            int passLevelReward = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].PassLevel;
            _rewardTxt.SetText(passLevelReward.ToString());
            
            UserData.UserData.Instance.AddRes(ResType.Coin, passLevelReward, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ScrewPassLevel,
                data1 = _screwGameContext.levelIndex.ToString(),
            }, false);
        }
        private void SendLevelWinBi()
        {
            if (ScrewGameLogic.Instance.SendBi)
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventScrewLevelEnd,_screwGameContext.levelIndex.ToString(),"pass");
            //错误注释
            // var currentLevel = _screwGameContext.levelIndex;
            // if (currentLevel <= 20)
            // {
            //     var eventType = (BiEventScrewscapes.Types.GameEventType) Enum.Parse(
            //         typeof(BiEventScrewscapes.Types.GameEventType), $"GameEventFunnelLevel{currentLevel}CompleteBoardShow");
            //     BIHelper.SendGameEvent(eventType);
            // }
            //
            // switch (currentLevel)
            // {
            //     case 10:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv10);
            //         break;
            //     case 20:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv20);
            //         break;
            //     case 30:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv30);
            //         break;
            //     case 40:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv40);
            //         break;
            //     case 50:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv50);
            //         break;
            //     case 70:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv70);
            //         break;
            //     case 100:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv100);
            //         break;
            //     case 150:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv150);
            //         break;
            //     case 200:
            //         BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventPassLv200);
            //         break;
            // }
        }

        private void SendCollectBi(float adRatio)
        {
            var currentLevel = _screwGameContext.levelIndex;
            if (currentLevel <= 20)
            {
                //错误注释
                // var eventCollectType = (BiEventScrewscapes.Types.GameEventType) Enum.Parse(
                //     typeof(BiEventScrewscapes.Types.GameEventType), $"GameEventFunnelLevel{currentLevel}CompleteBoardCollect");
                // BIHelper.SendGameEvent(eventCollectType);
            }
            
            _screwGameContext.SendLevelWinBi(adRatio);
        }

        private void SetUpUnlockFeatureUI()
        {
            if (_gameFeatureType == GameFeatureType.None)
            {
                _progressSlider.gameObject.SetActive(false);
                return;
            }

            var currentLevel = _screwGameContext.levelIndex;
            var progress = ScrewGameLogic.Instance.GetUnlockFeatureProgress(currentLevel);
            _progressSlider.value = progress;
            _progressText.SetText(ScrewGameLogic.Instance.GetUnlockFeatureProgressDesc(currentLevel));
            var featureSprite = AssetModule.Instance.GetSprite("ScrewFeatureAtlas", $"ui_feature_icon_{_gameFeatureType}_Small");
            if(featureSprite != null)
                _featureIcon.sprite = featureSprite;
        }

        private void UpdateUnlockProgress(float delayTime)
        {
            if (gameObject != null)
            {
                var currentLevel = ScrewGameLogic.Instance.GetMainLevelIndex();
                var progress = ScrewGameLogic.Instance.GetUnlockFeatureProgress(currentLevel, true);

                if (_progressSlider != null)
                {
                    _progressSlider.DOValue(progress, 0.4f).OnComplete(() =>
                    {
                        SoundModule.PlaySfx("sfx_ui_number_plus");
                        _progressText.SetText(ScrewGameLogic.Instance.GetUnlockFeatureProgressDesc(currentLevel, true));
                    }).SetDelay(delayTime);
                }
            }
        }

        public void RegisterEvent()
        {
            _continueButton.onClick.AddListener(() => { OnContinueButtonClicked(1); });
        }
 
        private async void OnContinueButtonClicked(float adRatio)
        {
            _continueButton.interactable = false;
            SendCollectBi(adRatio);

            if (NeedPlaySettleReward())
            {
                await AddLevelPassReward();
                UIModule.Instance.CloseWindow(typeof(UIScrewWinPopup));
                
                Debug.LogError("-------------------------------------");
                //GameApp.Get<GameStateSys>().ToState<GameStateScrewGame>();
                return;
            }

            AnimCloseWindow();
            
            if (_screwGameContext.afterWinLevelHandlers != null && _screwGameContext.afterWinLevelHandlers.Count > 0)
            {
                for (int i = 0; i < _screwGameContext.afterWinLevelHandlers.Count; i++)
                {
                    await _screwGameContext.afterWinLevelHandlers[i].Invoke(_screwGameContext);
                }
            }

            SceneFsm.mInstance.ChangeState(StatusType.ScrewHome, DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].PassLevel);
        }

        private bool NeedPlaySettleReward()
        {
            return false;
            return _screwGameContext.levelIndex > 1 && _screwGameContext.levelIndex < 10;
        }

        private async UniTask AddLevelPassReward()
        {
            // var unclaimedLevelReward = StorageManager.Instance.GetStorage<StorageGlobal>().UnclaimedLevelReward;
            //
            // var levelRewardItem = SMItemUtility.GenerateDummyItem(ItemType.Coin, unclaimedLevelReward.UnClaimedCoinCount);
            //
            // GameApp.Get<UserProfileSys>().SettleReward(levelRewardItem,
            //     new BIHelper.ItemChangeReasonArgs(BiEventScrewscapes.Types.ItemChangeReason.PlayLevel));
            //
            // _screwGameContext.biLevelInfo.LevelCoinScore = (uint)levelRewardItem.Amount;
            //
            // if (unclaimedLevelReward.UnClaimedCloverCount > 0)
            // {
            //     var levelStarRewardItem = SMItemUtility.GenerateDummyItem(ItemType.Clover, unclaimedLevelReward.UnClaimedCloverCount);
            //     GameApp.Get<UserProfileSys>().SettleReward(levelStarRewardItem,
            //         new BIHelper.ItemChangeReasonArgs(BiEventScrewscapes.Types.ItemChangeReason.PlayLevel));
            // }
            //
            // unclaimedLevelReward.UnClaimedCoinCount = 0;
            // unclaimedLevelReward.UnClaimedCloverCount = 0;
            //
            // await GameApp.Get<FlySys>().FlyItemAsync(levelRewardItem.ItemId,
            //     levelRewardItem.Amount,
            //     _icon.transform.position,
            //     GameApp.Get<FlySys>().GetTargetTransform(levelRewardItem.ItemId).position);
        }

        public void OnDestroy()
        {
        }
    }
}