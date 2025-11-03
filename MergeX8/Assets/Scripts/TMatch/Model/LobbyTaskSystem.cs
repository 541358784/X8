using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASMR;
using DragonPlus;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
// using OutsideGuide;
using UnityEngine;

namespace TMatch
{


    public class LobbyTaskParam
    {
        public TMatchLevelData MatchLevelData;
        public List<ItemData> MakeoverLevelData;
        public List<ItemData> ASMRLevelData;
    }

    public class LobbyTaskSystem : GlobalSystem<LobbyTaskSystem>
    {
        public struct TaskResult
        {
            public bool result;
            public bool isWindow;

            //没有进入task
            public static TaskResult False => new TaskResult() {result = false, isWindow = false};

            //进入task有弹窗
            public static TaskResult Window => new TaskResult() {result = true, isWindow = true};

            //进入task没有弹窗
            public static TaskResult NoWindow => new TaskResult() {result = true, isWindow = false};
        }

        private bool inAnyTask;
        private bool breakTaskList;

        private LobbyTaskParam param;

        public async void Star(LobbyTaskParam param)
        {
            this.param = param;
            await CheckList();
        }

        private async Task CheckList()
        {
            //任务链
            var tasks = new List<Func<TaskResult>>()
            {
                AddTouchBlock,
                // CheckTMatchResultStar,
                CheckTMatchResultWeeklyChallenge,
                CheckTMatchResultLast,
                // CheckMakeoverResultLast,
                // CheckASMRResultLast,
                CheckShowGoldenHatter,
                RemoveTouchBlock,
                CheckIceBreakingPack,
                // CheckAfterAddResourcesGuide,
                // CheckAfterAddResourcesAndASMREnableGuide,
                // CheckToSendBindReward,          // 检查登录奖励
                CheckRemoveAdPopup,             // 去广告礼包
                CheckBPUnCollect,
                StartBPGuide,
                CheckBpBuy,             //bp购买
                CheckBpView,
                CheckWeeklyChallenge, // 周挑战
                CheckTMGiftLinkView,
                CheckCrocodileView,
                CheckCrocodileWin,
                // CheckCoinCollect,               // 金币收集
                // CheckDiamondCollect,            // 钻石收集
                // CheckSignInPopup,               // 签到
                // CheckRateLikeBind,              // RateUs
                // CheckFBLikeUs,                  // FBLikeUs
                // CheckStarChallenge,             // 明星挑战赛
                // CheckCollectGuild,              // 公会收集
                // CheckPiggyBank,                 // 小猪存钱罐
                // CheckTeamGuidePopup,            // 公会引导
                StartGuide, // 开始引导
            };

            inAnyTask = false;
            breakTaskList = false;

            foreach (var p in tasks)
            {
                while (inAnyTask)
                {
                    if (breakTaskList) return;
                    await Task.Yield();
                }

                var checkResult = p.Invoke();
                inAnyTask = checkResult.result;
            }
        }

        //完成当前任务
        public void FinishCurrentTask()
        {
            inAnyTask = false;
        }

        //中断本次任务链
        public void BreakTaskList()
        {
            breakTaskList = true;
        }

        //各模块任务
        //-------------------------------------------------------------//
        //开启全屏阻断
        private TaskResult AddTouchBlock()
        {
            UIViewSystem.Instance.Open<NormalLayerBlock>();
            return TaskResult.False;
        }
        //-------------------------------------------------------------//

        //结果-星星
        private TaskResult CheckTMatchResultStar()
        {
            if (null == param.MatchLevelData) return TaskResult.False;
            if (PlayerPrefs.GetInt("TMatchResultExecuteStar", 1) == 1) return TaskResult.False;
            PlayerPrefs.SetInt("TMatchResultExecuteStar", 1);
            EventDispatcher.Instance.DispatchEvent(new TMatchResultExecuteEvent(param.MatchLevelData,
                TMatchResultExecuteType.Star));
            return TaskResult.NoWindow;
        }
        //-------------------------------------------------------------//

        //结果-周挑战
        private TaskResult CheckTMatchResultWeeklyChallenge()
        {
            if (null == param.MatchLevelData) return TaskResult.False;
            if (PlayerPrefs.GetInt("TMatchResultExecuteWeeklyChallenge", 1) == 1) return TaskResult.False;
            PlayerPrefs.SetInt("TMatchResultExecuteWeeklyChallenge", 1);
            EventDispatcher.Instance.DispatchEvent(new TMatchResultExecuteEvent(param.MatchLevelData,
                TMatchResultExecuteType.WeeklyChallenge));
            if (!global::WeeklyChallengeModel.Instance.IsOpened()) return TaskResult.False;
            return TaskResult.NoWindow;
        }
        //-------------------------------------------------------------//

        //结果-剩余
        private TaskResult CheckTMatchResultLast()
        {
            if (null == param.MatchLevelData) return TaskResult.False;
            if (PlayerPrefs.GetInt("TMatchResultExecuteLast", 1) == 1) return TaskResult.False;
            PlayerPrefs.SetInt("TMatchResultExecuteLast", 1);
            EventDispatcher.Instance.DispatchEvent(new TMatchResultExecuteEvent(param.MatchLevelData,
                TMatchResultExecuteType.Last));
            CheckResultLastFinsih();
            return TaskResult.NoWindow;
        }

        // 小游戏奖励
        private TaskResult CheckMakeoverResultLast()
        {
            // if(null == param.MakeoverLevelData) return TaskResult.False;
            // if(PlayerPrefs.GetInt("MakeoverResultExecuteLast", 1) == 1) return TaskResult.False;
            // PlayerPrefs.SetInt("MakeoverResultExecuteLast", 1);
            // EventDispatcher.Instance.DispatchEvent(new MakeoverResultExecuteEvent(param.MakeoverLevelData));
            // CheckResultLastFinsih();
            return TaskResult.NoWindow;
        }

        private TaskResult CheckASMRResultLast()
        {
            // if(null == param.ASMRLevelData) return TaskResult.False;
            // if(PlayerPrefs.GetInt("ASMRResultExecuteLast", 1) == 1) return TaskResult.False;
            // PlayerPrefs.SetInt("ASMRResultExecuteLast", 1);
            // EventDispatcher.Instance.DispatchEvent(new ASMRResultExecuteEvent(param.ASMRLevelData));
            // CheckResultLastFinsih();
            return TaskResult.NoWindow;
        }

        private async void CheckResultLastFinsih()
        {
            await Task.Delay(1000);
            FinishCurrentTask();
        }
        //-------------------------------------------------------------//

        //黄金帽
        private TaskResult CheckShowGoldenHatter()
        {
            UILobbyView mainView = UIViewSystem.Instance.Get<UILobbyView>();
            if (mainView != null && mainView.MainView.LevelButtonView.TryShowGoldenHatter())
                return TaskResult.NoWindow;
            return TaskResult.False;
        }
        //-------------------------------------------------------------//

        //去掉全屏阻断
        private TaskResult RemoveTouchBlock()
        {
            UIViewSystem.Instance.Close<NormalLayerBlock>();
            return TaskResult.False;
        }
        //-------------------------------------------------------------//

        // 破冰礼包
        private TaskResult CheckIceBreakingPack()
        {
            if (IceBreakingPackModel.Instance.TryToShowPopup())
                return TaskResult.Window;
            return TaskResult.False;
        }
        private TaskResult CheckAfterAddResourcesGuide()
        {
            // GuideSubSystem.Instance.Trigger(GuideTrigger.AfterAddResources, TMatchModel.Instance.GetMainLevel().ToString());
            return TaskResult.False;
        }

        private TaskResult CheckAfterAddResourcesAndASMREnableGuide()
        {
            // if (Model.Instance.CanShowEntry())
            // {
            //     GuideSubSystem.Instance.Trigger(GuideTrigger.AfterAddResourcesAndASMREnable, TMatchModel.Instance.GetMainLevel().ToString());
            // }
            return TaskResult.False;
        }

        //RateUs
        private TaskResult CheckRateLikeBind()
        {
            // if (RateUsModel.Instance.ShouldPopUpRateUs())
            // {
            //     RateUsModel.Instance.PopUpRateUs();
            //     return TaskResult.Window;
            // }

            return TaskResult.False;
        }
        //-------------------------------------------------------------//

        // Like Us
        private TaskResult CheckFBLikeUs()
        {
            // if (FBModel.Instance.TryAutoOpenFBLike())
            //     return TaskResult.Window;
            // else
            return TaskResult.False;
        }

        // 签到
        private TaskResult CheckSignInPopup()
        {
            // if (SignInModel.Instance.TryToAutoShowSignInPopup())
            //     return TaskResult.Window;
            // else
            return TaskResult.False;
        }

        // 公会引导
        private TaskResult CheckTeamGuidePopup()
        {
            // if (TeamManager.Instance.TryOpenTeamGuideUI())
            //     return TaskResult.Window;
            // else
            return TaskResult.False;
        }

        // 去广告礼包
        private TaskResult CheckRemoveAdPopup()
        {
            if (RemoveAdModel.Instance.TryToAutoOpen())
                return TaskResult.Window;
            else
            return TaskResult.False;
        }

        // 绑定奖励
        private TaskResult CheckToSendBindReward()
        {
//         //新用户如果绑定了Facebook，直接将奖励发放到账户
//         if (AccountManager.Instance.HasBindFacebook())
//         {
//             var globalStorage = StorageManager.Instance.GetStorage<StorageGlobal>();
//             if (!globalStorage.SocialBind.FacebookBindRewardReceived && globalStorage.SocialBind.FacebookFirstBindPendingReward)
//             {
//                 var rewardCfg = TMatch.GameConfigManager.Instance.GetGlobalReward("FBBind");
//                 List<ItemData> rewards = new List<ItemData>();
//                 for (var i = 0; i < rewardCfg.RewardID.Count; i ++)
//                 {
//                     rewards.Add(new ItemData(){id = rewardCfg.RewardID[i], cnt = rewardCfg.RewardCnt[i]});
//                 }
//                 UIGetRewardParam rewardViewParam = new UIGetRewardParam();
//                 rewardViewParam.itemDatas = rewards;
//                 rewardViewParam.closeAction = () =>
//                 {
//                     globalStorage.SocialBind.FacebookBindRewardReceived = true;
//                     globalStorage.SocialBind.FacebookFirstBindPendingReward = false;
//                     FinishCurrentTask();
//                 };
//                 rewardViewParam.itemChangeReasonArgs = new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventMatchFrenzy.Types.ItemChangeReason.FbBind);
//                 UIViewSystem.Instance.Open<UIGetBindRewardView>(rewardViewParam);
//                 return TaskResult.Window;
//             }
//         }
//         
// #if UNITY_IOS
//         // 苹果绑定奖励
//         if (AccountManager.Instance.HasBindApple())
//         {
//             var globalStorage = StorageManager.Instance.GetStorage<StorageGlobal>();
//             if (!globalStorage.SocialBind.AppleBindRewardReceived && globalStorage.SocialBind.AppleFirstBindPendingReward)
//             {
//                 var rewardCfg = TMatch.GameConfigManager.Instance.GetGlobalReward("APPLEBind");
//                 List<ItemData> rewards = new List<ItemData>();
//                 for (var i = 0; i < rewardCfg.RewardID.Count; i ++)
//                 {
//                     rewards.Add(new ItemData(){id = rewardCfg.RewardID[i], cnt = rewardCfg.RewardCnt[i]});
//                 }
//                 UIGetRewardParam rewardViewParam = new UIGetRewardParam();
//                 rewardViewParam.itemDatas = rewards;
//                 rewardViewParam.closeAction = () =>
//                 {
//                     globalStorage.SocialBind.AppleBindRewardReceived = true;
//                     globalStorage.SocialBind.AppleFirstBindPendingReward = false;
//                     FinishCurrentTask();
//                 };
//                 rewardViewParam.itemChangeReasonArgs = new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventMatchFrenzy.Types.ItemChangeReason.AppleBind);
//                 UIViewSystem.Instance.Open<UIGetBindRewardView>(rewardViewParam);
//                 return TaskResult.Window;
//             }
//         }
// #endif

            return TaskResult.False;
        }

        //金币搜集
        private TaskResult CheckCoinCollect()
        {
            // if (CollectCoinController.Instance.TryOpenView())
            //     return TaskResult.Window;
            // else
            return TaskResult.False;
        }

        //钻石收集
        private TaskResult CheckDiamondCollect()
        {
            // if (CollectDiamondController.Instance.TryOpenView())
            //     return TaskResult.Window;
            // else
            return TaskResult.False;
        }

        private TaskResult CheckStarChallenge()
        {
            // if (StarChallengeController.Instance.TryOpenView())
            //     return TaskResult.Window;
            return TaskResult.False;
        }

        private TaskResult CheckCollectGuild()
        {
            // if (CollectGuildController.Instance.TryOpenView())
            //     return TaskResult.Window;
            return TaskResult.False;
        }

        private TaskResult CheckPiggyBank()
        {
            // if (PiggyBankActivityModel.Instance.Check())
            //     return TaskResult.Window;
            return TaskResult.False;
        }

        //周挑战
        private TaskResult CheckWeeklyChallenge()
        {
            if (WeeklyChallengeController.Instance.TryOpenView())
                return TaskResult.Window;
            else
                return TaskResult.False;
        }

        /// <summary>
        /// 开始引导
        /// </summary>
        /// <returns></returns>
        private TaskResult StartGuide()
        {
            if (TMatchModel.Instance.TryGuideLevel())
                return TaskResult.False;

            if (TMatchModel.Instance.TryGuideFinishLevel())
                return TaskResult.False;

            return TaskResult.False;
        }

        public TaskResult CheckBPUnCollect()
        {
            if (TMBPModel.CanShowUnCollectRewardsUI())
                return TaskResult.Window;
            return TaskResult.False;
        }

        /// <summary>
        /// 开启bp引导
        /// </summary>
        /// <returns></returns>
        private TaskResult StartBPGuide()
        {
            if (TMBPModel.Instance.StartBpEntranceGuide())
                return TaskResult.NoWindow;
            return TaskResult.False;
        }

        /// <summary>
        /// 检测bp购买
        /// </summary>
        /// <returns></returns>
        private TaskResult CheckBpBuy()
        {
            if (TMBPModel.Instance.PopBpBuy())
            {
                return TaskResult.Window;
            }
            return TaskResult.False;
        }

        /// <summary>
        /// 检测bp
        /// </summary>
        /// <returns></returns>
        private TaskResult CheckBpView()
        {
            if (TMBPModel.Instance.PopBpView())
            {
                return TaskResult.Window;
            }
            return TaskResult.False;
        }
        
        private TaskResult CheckTMGiftLinkView()
        {
            if (UIGiftBagLinkController.CanShowUI())
            {
                return TaskResult.Window;
            }
            return TaskResult.False;
        }

        private TaskResult CheckCrocodileView()
        {
            if (CrocodileActivityModel.Instance.TryToShowPopup())
            {
                return TaskResult.Window;
            }
            return TaskResult.False;
        }
        private TaskResult CheckCrocodileWin()
        {
            if (CrocodileActivityModel.Instance.TryToPopupWinInGame())
            {
                return TaskResult.Window;
            }
            return TaskResult.False;
        }
    }
}