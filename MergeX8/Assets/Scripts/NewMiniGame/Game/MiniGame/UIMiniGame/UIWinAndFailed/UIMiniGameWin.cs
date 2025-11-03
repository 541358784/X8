using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Framework.UI;
using Framework.Utils;
using MiniGame;
using UnityEngine;

namespace Scripts.UI
{
    public class UIMiniGameWin : UIView
    {
        public class Data : UIData
        {
            public int levelId;
            public ResData data;

            public Data(int levelId, ResData data)
            {
                this.levelId = levelId;
                this.data = data;
            }
        }

        //先注释
        //private UIRewardElement _reward;
        private Data _data;

        public static void Open(int chapterId, int levelId, ResData data)
        {
            EventBus.Send<EventMiniGameLevelWin>();
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFinish, chapterId.ToString(), levelId.ToString());

            CommonRewardManager.Instance.PopCommonReward(new List<ResData>(){data}, CurrencyGroupManager.Instance.currencyController, false, new GameBIManager.ItemChangeReasonArgs(){},
                animEndCall: () =>
                {
                    CurrencyGroupManager.Instance.currencyController.CheckLevelUp(() =>
                    {
                        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, "Click_BackHome", "Click_BackHome");
                        EventBus.Send<EventMiniGameLevelWinClaimed>();
                        MiniGameModel.Instance.MiniGameHandle();
                    });
                    if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
                    {
                        NoAdsGiftBagModel.Instance.TryCreate();
                        AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b =>
                        {
                            NoAdsGiftBagModel.Instance.TryShow();
                        });
                    }  
                },clickGetCall:() =>
                {
                });

            //Framework.UI.UIManager.Instance.Open<UIMiniGameWin>("NewMiniGame/UIMiniGame/Prefab/UIPopupVictory", new Data(levelId, data));
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            //先注释
            //_reward = BindElement<UIRewardElement>("Root/MiddleGroup/RewardGroup");
            BindButtonEvent("Root/BottomGroup/ButtonContinue", OnClaimClicked);
            BindButtonEvent("Root/PopupCommonItem/CommonGroup/ButtonClose", OnClaimClicked);
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            _data = data as Data;

            ShowRewards();
            FlyCoin();
        }

        protected internal override void OnRemove()
        {
            base.OnRemove();
             EventBus.Send<EventMiniGameLevelWinClaimed>();
            //
            // GuideTool.Instance.StartGuide(GuideTriggerType.FinishMinigame, $"{_data.levelId}");
        }

        private void ShowRewards()
        {
            //先注释
           // _reward.Set(_data.data);
        }

        private void FlyCoin()
        {
            var bundle = new GameObject("FlyBundle");
            //先注释
            //bundle.transform.SetParent(_reward.GameObject.transform);
            bundle.transform.localScale = Vector3.one;

            //先注释
            // var data = new UIFlyBundle.Data(GameUtil.CoinItemId, _data.data.count, UIChapter.GetCoinIconPos(), 1f, 0f, 0f, 1f);
            // AddElement<UIFlyBundle>(bundle, null, data);
        }

        private void OnClaimClicked()
        {
            Close();
        }
    }
}