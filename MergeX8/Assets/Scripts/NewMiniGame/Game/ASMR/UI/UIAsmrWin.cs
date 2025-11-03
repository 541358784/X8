using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Framework.UI;
using Framework.Utils;
using MiniGame;
using Scripts.UI;

namespace ASMR
{
    public class UIAsmrWin : UIPopupView
    {
        private LocalizeTextMeshProUGUI _countText;

        public static void Open(ResData rewardData)
        {
            CommonRewardManager.Instance.PopCommonReward(new List<ResData>(){rewardData}, CurrencyGroupManager.Instance.currencyController, false, new GameBIManager.ItemChangeReasonArgs(){},
                animEndCall: () =>
                {
                    CurrencyGroupManager.Instance.currencyController.CheckLevelUp(() =>
                    {
                        EventBus.Send<EventMiniGameLevelWinClaimed>();
                        EventDispatcher.Instance.DispatchEvent(EventMiniGame.MINIGAME_SETSHOWSTATUS, true, true, true);
                    });
                },clickGetCall:() =>
                {
                    Framework.UI.UIManager.Instance.Close<UIAsmr>();
                    UIChapter.Open(ASMRModel.Instance.AttData.chapterId);
                    ASMRModel.Instance.UnLoadCurrentLevel();
                    EventDispatcher.Instance.DispatchEventImmediately(EventMiniGame.MINIGAME_SETSHOWSTATUS, true, true, false);

                    var view = Framework.UI.UIManager.Instance.GetView<UIChapter>();
                    if (view != null)
                    {
                        view.ChapterContent?.ShowBubbles(false);
                    }
                });
            
            // var ui = Framework.UI.UIManager.Instance.Open<UIAsmrWin>("NewMiniGame/UIMiniGame/Prefab/UIPopupVictory");
            // ui.SetRewardData(rewardData);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _countText = BindItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/RewardGroup/NormalReward/TextCount");

            BindButtonEvent("Root/BottomGroup/ButtonContinue", OnContinueBtnClick);
        }

        protected override void OnCloseBtnClick()
        {
            // base.OnCloseBtnClick();

            OnContinueBtnClick();
        }

        private void OnContinueBtnClick()
        {
            // ASMRModel.Instance.UnLoadCurrentLevel();
            //
            // Close();
            // Framework.UI.UIManager.Instance.Close<UIAsmr>();
            //
            // UIChapter.Open(ASMRModel.Instance.AttData.chapterId);
        }

        private void SetRewardData(ResData rewardData)
        {
            _countText.SetText($"{rewardData.count}");
        }
    }
}