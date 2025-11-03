using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;
using ABTest;
using Cysharp.Threading.Tasks;
using Decoration;
using Decoration.DaysManager;
using DragonPlus;
using DG.Tweening;
using Difference;
using Ditch.Model;
using DragonU3DSDK.Asset;
using DragonPlus.Config;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.Ditch;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Filthy.Game;
using Filthy.Model;
using Framework;
using Game;
using Gameplay;
using Gameplay.UI.Capybara;
using Gameplay.UI.MiniGame;
using GamePool;
using Makeover;
using Manager;
using Merge.Order;
using Psychology;
using Screw;
using Utils = DragonU3DSDK.Utils;

public class SceneFsmHome : IFsmState
{
    public StatusType Type => StatusType.Home;

    public void Enter(params object[] objs)
    {
        EnterLogic(objs);
    }

    private async UniTask EnterLogic(params object[] objs)
    {
        string data1 = "1";
        if (AccountManager.Instance.HasBindApple())
        {
            data1 = "3";
        }
        else if (AccountManager.Instance.HasBindFacebook())
        {
            data1 = "2";
        }

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueLobby, data1);

        UIManager.Instance.CloseUI(UINameConst.UISaveProgress);

        AudioManager.Instance.PlayMusic(1, true);
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, "GAME_EVENT_ENTER_MAIN_UI"))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, "GAME_EVENT_ENTER_MAIN_UI", CommonUtils.GetTimeStamp());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnterMainUi);
        }

        SceneFsm.mInstance.ClientLogin = true;

        UIRoot.Instance.EnableEventSystem = false;

        UIHomeMainController.ShowUI();

        if (FarmModel.Instance.IsFarmModel())
        {
            await UniTask.WaitForEndOfFrame();
            await UniTask.WaitForEndOfFrame();

            SceneFsmEnterFarm.Prepare();
            AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, false, true, isForce: true);
            SceneFsmEnterFarm.EnterSuccess(true);
        }

        StoreModel.Instance.CheckVerifyUnfulfilledPayments();
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.EnterHome, objs);

        UIPopupRVRewardController.InitRvReward();
        UserGroupManager.Instance.UpdateSubUserGroup();
        MailDataModel.Instance.RequestMailList();

        MainOrderCreatorRecommend.RefreshRecommend();

        PlayerManager.Instance.UpdatePlayersState(false);

        UIPopupMysteryGiftController.MysteryGiftShowTime = (int)Time.realtimeSinceStartup;

        StorageHome home = StorageManager.Instance.GetStorage<StorageHome>();


        DaysManager.Instance.CanShowRetrieveReward();

        if (home.IsFirstLogin)
        {
            DifferenceManager.Instance.Init();
        }

        if (!home.IsFirstLogin)
        {
            home.AvatarData.UserName = "Player " + DragonU3DSDK.Utils.PlayerIdToString(StorageManager.Instance.GetStorage<StorageCommon>().PlayerId);
            ;

            CapybaraManager.Instance.Init();

            if (!home.RcoveryRecord.ContainsKey("1.0.36"))
                home.RcoveryRecord.Add("1.0.36", true);

            if (!home.RcoveryRecord.ContainsKey("1.0.46"))
                home.RcoveryRecord.Add("1.0.46", true);

            if (!home.RcoveryRecord.ContainsKey("1.0.60"))
                home.RcoveryRecord.Add("1.0.60", true);

            if (!home.RcoveryRecord.ContainsKey("1.0.63"))
                home.RcoveryRecord.Add("1.0.63", true);

            if (!home.RcoveryRecord.ContainsKey("1.0.65"))
                home.RcoveryRecord.Add("1.0.65", true);

            if (!home.RcoveryRecord.ContainsKey("1.0.68"))
                home.RcoveryRecord.Add("1.0.68", true);

            if (!home.RcoveryRecord.ContainsKey("1.0.69"))
                home.RcoveryRecord.Add("1.0.69", true);

            if (Makeover.Utils.IsOn(UIPopupMiniGameController.MiniGameType.Psychology))
                Makeover.Utils.InitMiniGameGroup(MiniGroup.Puzzle);
            else if (Makeover.Utils.IsOn(UIPopupMiniGameController.MiniGameType.DigTrench))
                Makeover.Utils.InitMiniGameGroup(MiniGroup.DigTrench);

            if (!home.AbTestConfig.ContainsKey("BattlePass"))
                home.AbTestConfig.Add("BattlePass", "BattlePass");

            if (!home.AbTestConfig.ContainsKey("MergeMainUIAB"))
                home.AbTestConfig.Add("MergeMainUIAB", "MergeMainUIAB");

            if (!home.AbTestConfig.ContainsKey("GuideAB"))
                home.AbTestConfig.Add("GuideAB", "GuideAB");

            if (!home.AbTestConfig.ContainsKey("SecondGuideAB"))
                home.AbTestConfig.Add("SecondGuideAB", "SecondGuideAB");

            if (!home.AbTestConfig.ContainsKey("GuidePlanC"))
                home.AbTestConfig.Add("GuidePlanC", "GuidePlanC");

            if (!home.AbTestConfig.ContainsKey("Tmatch"))
                home.AbTestConfig.Add("Tmatch", "Tmatch");

            if (!home.AbTestConfig.ContainsKey("CloseTM"))
                home.AbTestConfig.Add("CloseTM", "CloseTM");

            if (!home.AbTestConfig.ContainsKey("DitchPlanD"))
                home.AbTestConfig.Add("DitchPlanD", "DitchPlanD");

            if (!home.AbTestConfig.ContainsKey("AB_TEST_AD_LOCAL_CONFIG_PAY_NEW_KEY"))
                home.AbTestConfig.Add("AB_TEST_AD_LOCAL_CONFIG_PAY_NEW_KEY", "AB_TEST_AD_LOCAL_CONFIG_PAY_NEW_KEY");

            if (!home.AbTestConfig.ContainsKey(MainOrderManager.Instance.AbKey))
                home.AbTestConfig.Add(MainOrderManager.Instance.AbKey, MainOrderManager.Instance.AbKey);

            AdLocalConfigHandle.Instance.Storage.IsNewUser = true;

            NewNewIceBreakPackModel.Instance.Storage.IsNewUser = true;

            DifferenceManager.Instance.Init();
            if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
            {
                home.IsFirstLogin = true;
                
                DecoSceneRoot.Instance.mSceneCamera.gameObject.transform.position = new Vector3(1.67999995f, -19f, -15.9948339f);
                DecoManager.Instance.CurrentWorld.PinchMap.transform.Find("Surface/Movie_Ship").transform.position = new Vector3(1.67999995f, -19f, -15.9948339f);
            }
            else
            {
                StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.FirstEnterGame, null, b => { });

                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueMinigameWater6);

                Action startLogic = () =>
                {
                    AudioManager.Instance.ResumeAllMusic();
                    GameBIManager.Instance.SendGameEvent(
                        BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueMinigameWater7);
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtue3);
                    StoryMovieSubSystem.Instance.StopMovie();
                    var bag = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find(
                        "Surface/Movie_Ship/ripple/SkeletonUtility-SkeletonRoot/root/chuan/bag");
                    if (bag)
                    {
                        bag.gameObject.SetActive(true);
                        bag.GetComponent<Animator>().PlayAnimation("normal");
                    }

                    Action storyMovieAction = () =>
                    {
                        StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.CGEnd, null, b =>
                        {
                            Action cb = () =>
                            {
                                StorySubSystem.Instance.Trigger(StoryTrigger.StoryMovieEnd, "101001", (b) =>
                                {
                                    if (b)
                                        home.IsFirstLogin = true;
                                 
                                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
                                });
                            };
                            //表演包掉落动画
                            if (bag)
                            {
                                bag.GetComponent<Animator>().PlayAnimation("appear", cb);
                            }
                            else
                            {
                                cb();
                            }
                        });
                    };
                    
                    storyMovieAction();
                };
                
                CGVideoManager.Instance.TryStartCG(null, () => { startLogic(); });
            }
        }
        else
        {
            Makeover.Utils.InitMiniGameGroup(MiniGroup.None);

            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.5f,
                () => { UIRoot.Instance.EnableEventSystem = true; }));
            CoroutineManager.Instance.StartCoroutine(
                EnterHomeLogic());
            StorySubSystem.Instance.TriggerStoryEndGuide();

            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
        }

        home.IsFirstLogin = true;

        AdLocalConfigHandle.Instance.InitGroup();

        Input.multiTouchEnabled = true;
        EventDispatcher.Instance.DispatchEvent(EventEnum.LOGIN_SUCCESS);
    }

    private IEnumerator EnterHomeLogic()
    {
        yield return MatchGameManager.MatchGameManager.Instance.MatchGame();

        yield return BackHomeControl.NewEventLogic();

        // yield return BackHomeControl.DayRetrieve();

        yield return BackHomeControl.GuideLogic(true);

        yield return AutoPopupManager.AutoPopupManager.Instance.FirstPopUIViewLogic();

        BackHomeControl.isFristPopUI = true;
    }

    public void Update(float deltaTime)
    {
        DecoManager.Instance?.Update(deltaTime);
    }

    public void LateUpdate(float deltaTime)
    {
    }

    public void Exit()
    {
        //UIHomeMainController.HideUI();
        Input.multiTouchEnabled = false;
    }

    private void checkAreaUnlock()
    {
        foreach (var area in Deco.World.DecoWorld.AreaLib.Values)
        {
            area.TryFinish();
        }
    }
}