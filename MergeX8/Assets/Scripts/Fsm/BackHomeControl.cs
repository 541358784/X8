using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using Activity.CrazeOrder.Model;
using Activity.DiamondRewardModel.Model;
using Activity.GardenTreasure.Model;
using Activity.LimitTimeOrder;
using Activity.Matreshkas.Model;
using Activity.SaveTheWhales;
using Activity.TimeOrder;
using Activity.TotalRecharge;
using Activity.TreasureHuntModel;
using Activity.Turntable.Model;
using ActivityLocal.CardCollection.Home;
using Deco.Node;
using Deco.World;
using Decoration;
using Decoration.DaysManager;
using DragonPlus;
using DG.Tweening;
using DragonU3DSDK.Asset;
using DragonPlus.Config;
using DragonPlus.UI;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Storage;
using ExtraEnergy;
using Farm.Model;
using Framework;
using Gameplay;
using Gameplay.UI.EnergyTorrent;
using Gameplay.UI.UpdateRewardManager;
using GamePool;
using Manager;
using Merge.UnlockMergeLine;
using OptionalGift;
using TMatch;
using TotalRecharge_New;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using Random = UnityEngine.Random;


public class BackHomeControl
{
    public static bool isFristPopUI = false;
    
    public static DecoNode doitNode = null;
    public static DecoOperationType opType = DecoOperationType.None;
    public static List<int> doitItems = new List<int>();
    public static Action doitAction = null;
    public static bool EnterFarm;
    public static int miniGameCharpterId;
    public static Action miniGameCallBackAction = null;
    

    
    public static void BackHomeLogic()
    {
        CoroutineManager.Instance.StartCoroutine(BackHome());
    }

    
    private static IEnumerator BackHome()
    {
        UIHomeMainController.UpdateAuixControllers();

        UIRoot.Instance.EnableEventSystem = false;

        bool recoverEvent = true;
        if (opType == DecoOperationType.None  || opType == DecoOperationType.MiniGame)
        {
            yield return new WaitForSeconds(0.3f);

            yield return CoroutineManager.Instance.StartCoroutine(GuideLogic());

            // yield return CoroutineManager.Instance.StartCoroutine(BuildAccountReward()); 
            miniGameCallBackAction?.Invoke();
            yield return CoroutineManager.Instance.StartCoroutine(AutoPopupManager.AutoPopupManager.Instance.HomePopUIViewLogic());

            if(FarmModel.Instance.IsFarmModel())
                yield return CoroutineManager.Instance.StartCoroutine(AutoPopupManager.AutoPopupManager.Instance.FarmPopUIViewLogic());
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            switch (opType)
            {
                case DecoOperationType.Buy:
                {
                    DecoManager.Instance.SelectNode(doitNode);
                    break;
                }
                case DecoOperationType.Install:
                {
                    DecoManager.Instance.InstallItem(doitItems, doitAction);
                    recoverEvent = false;
                    break;
                }
                case DecoOperationType.Preview:
                {
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NODE_PREVIEW, doitItems,doitAction);
                    break;
                }
            }
            
            doitNode = null;
            doitItems = null;
            doitAction = null;
            miniGameCallBackAction = null;
            opType = DecoOperationType.None;
            yield return new WaitForSeconds(1f);
        }

        if(recoverEvent)
            UIRoot.Instance.EnableEventSystem = true;
        yield break;
    }
    

    public static IEnumerator GuideLogic(bool isEnterHome = false)
    {
        if (CheckMainGuide(isEnterHome))
        {
            UIManager.Instance.CloseUI(UINameConst.UIPopupUnlockRoom, true);
            yield return new WaitForSeconds(0.3f);
            UIRoot.Instance.EnableEventSystem = true;
            yield break;
        }

        EventDispatcher.Instance.DispatchEvent(EventEnum.BackHomeStep, "guide");
    }

    public static IEnumerator NewEventLogic()
    {
        if (!NewEventModel.Instance.GetOpenState() || NewEventModel.Instance.wwwImage == null)
            yield break;

        UIManager.Instance.OpenUI(UINameConst.UIPopupNewEvent);
        while(true)
        {
            var dlg = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupNewEvent);
            if (dlg == null)
                yield break;
            
            if(!dlg.isActiveAndEnabled)
                yield break;
            
            yield return new WaitForEndOfFrame();
        }
    }


    public static bool CheckMainGuide(bool isEnterHome = false, bool checkLevel = true, int nodeId = -1)
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.EnterAsmr))
        {
            if (Makeover.Utils.IsOpen && GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnterAsmr, nodeId.ToString()))
                return true;
        }
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.EnterHome))
        {
            if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
            {
                if (GuideSubSystem.Instance.isFinished(203) && !GuideSubSystem.Instance.isFinished(204))
                    GuideSubSystem.Instance.ForceFinished(204);
                
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, "Click_MiniGame", "Click_MiniGame"))
                    return true;
            }
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnterHome, null))
                return true;
            
            if (checkLevel && GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnterHome, ExperenceModel.Instance.GetLevel().ToString()))
                return true;
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnterHome, TMatch.TMatchModel.Instance.IsUnlock() ? "TMatch" : "None"))
                return true;
        }
        
        
        if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
        {
            if (!GuideSubSystem.Instance.isFinished(260))
            {
                int value = UnlockManager.GetUnlockParam(UnlockManager.MergeUnlockType.MiniGame_Deo);
                int level = ExperenceModel.Instance.GetLevel();
                level = level >= value ? value : level;
                
                string key = $"Click_Task_{level}";
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, key, key);
            }
        }
        
        if (DecoManager.Instance.CurrentWorld != null && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TouchBubble))
        {
            var areaNodes = DecoManager.Instance.CurrentWorld.GetUnlockAndNotOwnedNodes();
            if (areaNodes != null && areaNodes.Count > 0)
            {
                foreach (var area in areaNodes)
                {
                    for(int i = 0; i < area.Value.Count; i++)
                    {
                        if (!area.Value[i].IsOwned && !UserData.Instance.CanAford((UserData.ResourceId)area.Value[i]._data._config.costId, area.Value[i]._data._config.price))
                            continue;
                        
                        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TouchBubble, area.Value[i].Config.id.ToString()))
                            return true;
                        
                        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TouchBubble, area.Value[i].Config.costId.ToString()))
                            return true;
                    }
                }
            }
        }
        
        //海豹
        if (DecoWorld.NodeLib.ContainsKey(101099))
        {
            if (!DecoWorld.NodeLib[101099].IsFinish && DecoWorld.NodeLib[101099].UnLocked)
            {
                if (StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.FinishNode, "101017"))
                    return true;
            }
        }
        
        //海豚
        if (StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.FinishNode, "101099"))
            return true;
        
        if (isEnterHome && StorageManager.Instance.GetStorage<StorageHome>().IsBackGame)
        {
            if (UIHomeMainController.mainController != null)
            {
                UIHomeMainController.mainController.OnButtonPlay();
                return true;
            }
        }

        FarmModel.Instance.TriggerFarmGuide();
        return false;
    }

    public static IEnumerator BuildAccountReward()
    {
        if (GuideSubSystem.Instance.IsShowingGuide())
        {
            yield break;
        }

        StorageHome home = StorageManager.Instance.GetStorage<StorageHome>();
        if (home == null)
            yield break;

        // if(GlobalConfigManager.Instance.GetNumValue("buildAccountRewardLevel") > LevelGroupSystem.Instance.GetPassedLevelCount())
        //     yield break;

        bool isGetReward = false;
        if (AccountManager.Instance.HasBindApple())
        {
            if (home.BuildReward_Apple)
                yield break;

            home.BuildReward_Apple = true;
            isGetReward = true;
            var appleRewardCfg =
                GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.bind_apple_reward);
            var appleRewards = ResData.ParseList(appleRewardCfg);

            if (appleRewards != null && appleRewards.Count > 0)
            {
                UIRoot.Instance.EnableEventSystem = true;
                CommonRewardManager.Instance.PopCommonReward(appleRewards,
                    CurrencyGroupManager.Instance.currencyController, true,
                    new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.BindFacebook),
                    () => { isGetReward = false; });
            }
        }

        if (AccountManager.Instance.HasBindFacebook())
        {
            if (home.BuildReward_FB)
                yield break;

            home.BuildReward_FB = true;
            isGetReward = true;
            var fbRewardCfg =
                GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.link_fb_reward);
            var fbRewards = ResData.ParseList(fbRewardCfg);

            if (fbRewards != null && fbRewards.Count > 0)
            {
                UIRoot.Instance.EnableEventSystem = true;
                CommonRewardManager.Instance.PopCommonReward(fbRewards,
                    CurrencyGroupManager.Instance.currencyController, true,
                    new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.BindFacebook),
                    () => { isGetReward = false; });
            }
        }

        while (isGetReward)
        {
            yield return new WaitForEndOfFrame();
        }

        UIRoot.Instance.EnableEventSystem = false;
        yield break;
    }

    public static void BackButtonQuickApp()
    {
        if (UIPopupChooseProgressController.IsOpenWindow)
            return;

        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
        {
            DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_quit_game_tips_text"),
            OKCallback = () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                //NotificationManager.Instance.PushLocalNotifications ();
                UnityEngine.Application.Quit ();
#endif
            },
            OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok"),
            CancelButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_cancel"),
            HasCancelButton = true,
        });
    }
}