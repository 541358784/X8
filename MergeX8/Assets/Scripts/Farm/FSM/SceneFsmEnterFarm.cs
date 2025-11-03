using System;
using System.Collections;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X9;
using Decoration;
using Decoration.Bubble;
using Decoration.DynamicMap;
using Decoration.WorldFogManager;
using Farm.Model;
using Farm.Order;
using Farm.View;
using Framework;
using Manager;
using MiniGame;
using Screw.GameLogic;
using Screw.Module;
using Scripts.UI;
using UnityEngine;

namespace Screw
{
    public class SceneFsmEnterFarm: IFsmState
    {
        public StatusType Type => StatusType.EnterFarm;

        public void Enter(params object[] objs)
        {
            UIFarmLoadingController.ShowLoading(() =>
            {
                Prepare();
                AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, false, true);

                FarmModel.Instance.LoadWorld(FarmModel.worldId, () =>
                {
                    EnterSuccess(false);
                });
            });

            DecoManager.Instance.CurrentWorldId = FarmModel.worldId;
        }

        public static void Prepare()
        {
            FarmOrderManager.Instance.TryCreateOrder();
            if (GameModeManager.Instance.GetCurrenGameMode() == GameModeManager.CurrentGameMode.MiniGame)
            {
                Framework.UI.UIManager.Instance.Close<UIChapter>();
            }
        }

        public static void EnterSuccess(bool isFirst)
        {
            UIRoot.Instance.EnableEventSystem = true;
            UIManager.Instance.OpenUI(UINameConst.UIFarmMain);
            UIFarmLoadingController.HideLoading();

            if (!StorySubSystem.Instance.IsStoryFinished(20000))
            {
                StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.Initiative, "10008");
            }
            else
            {
                FarmModel.Instance.TriggerFarmGuide();
            }

            if (!isFirst)
            {
                CoroutineManager.Instance.StartCoroutine(AutoPopupManager.AutoPopupManager.Instance.FarmPopUIViewLogic());
            }
            FarmModel.Instance.AdaptMainUI(true);
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
            FarmModel.Instance.AnimShow(false);
        }
    }
}